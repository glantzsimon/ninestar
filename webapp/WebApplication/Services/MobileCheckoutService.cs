using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services.Stripe;
using K9.WebApplication.ViewModels.MobileApi;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace K9.WebApplication.Services
{
    // Coordinates existing pricing, Stripe, membership and booking services.
    // Checkout records use existing private UserPreference storage: no new schema.
    public class MobileCheckoutService : BaseService, IMobileCheckoutService
    {
        private readonly INineStarKiPackage _package;
        private readonly IStripeService _stripe;
        private readonly IPromotionService _promotions;
        private readonly IConsultationService _consultations;
        private readonly IDonationService _donations;
        private readonly IRepository<UserPreference> _records;
        private readonly DbContext _database;
        private const string Prefix = "MobileCheckout:";

        public MobileCheckoutService(INineStarKiPackage package, IStripeService stripe, IPromotionService promotions,
            IConsultationService consultations, IDonationService donations, IRepository<UserPreference> records, DbContext database) : base(package)
        { _package = package; _stripe = stripe; _promotions = promotions; _consultations = consultations; _donations = donations; _records = records; _database = database; }

        public MobileCheckoutIntent Start(int userId, MobileCheckoutRequest request)
        {
            CheckIdentity(userId);
            if (request == null || request.RequestId == Guid.Empty || !Enum.IsDefined(typeof(MobileCheckoutKind), request.Kind))
                throw new MobileCheckoutException("invalid_checkout_request", 400);
            using (var transaction = _database.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                var record = Find(userId, request.RequestId);
                if (record != null)
                {
                    var saved = Read(record, userId, request.RequestId);
                    if (saved.Kind != request.Kind || saved.MembershipOptionId != request.MembershipOptionId ||
                        saved.Duration != request.Duration || saved.DonationAmount != request.DonationAmount || saved.PromoCode != request.PromoCode)
                        throw new MobileCheckoutException("checkout_request_id_reused");
                    transaction.Commit();
                    return saved.Intent;
                }
                var state = new CheckoutState
                {
                    UserId = userId, Kind = request.Kind, MembershipOptionId = request.MembershipOptionId,
                    Duration = request.Duration, DonationAmount = request.DonationAmount, PromoCode = request.PromoCode,
                    Intent = new MobileCheckoutIntent { RequestId = request.RequestId, Currency = StripeModel.GetSystemCurrencyCode() }
                };
                double amount;
                string description;
                switch (request.Kind)
                {
                    case MobileCheckoutKind.Membership:
                        ValidatePromotion(userId, request.MembershipOptionId, request.PromoCode);
                        // Passing a promotion to the old model constructor mutates
                        // MembershipOption.Price. Quote without mutating that entity.
                        var model = _package.MembershipService.GetPurchaseMembershipModel(request.MembershipOptionId);
                        if (model.MembershipOption == null || model.MembershipOption.IsDeleted || model.MembershipOption.IsFree)
                            throw new MobileCheckoutException("invalid_membership_option", 400);
                        amount = string.IsNullOrEmpty(request.PromoCode) ? model.SubscriptionPrice : _promotions.Find(request.PromoCode).SpecialPrice;
                        description = model.MembershipOption.SubscriptionTypeNameLocal;
                        break;
                    case MobileCheckoutKind.Consultation:
                        if (!Enum.IsDefined(typeof(K9.DataAccessLayer.Enums.EConsultationDuration), request.Duration))
                            throw new MobileCheckoutException("invalid_consultation_duration", 400);
                        var consultation = new Consultation { ConsultationDuration = request.Duration };
                        amount = consultation.Price;
                        description = consultation.ConsultationDescription;
                        break;
                    default:
                        amount = request.DonationAmount;
                        description = K9.Globalisation.Dictionary.Donation;
                        break;
                }
                if (double.IsNaN(amount) || double.IsInfinity(amount) || amount < 0 || amount > 100000)
                    throw new MobileCheckoutException("invalid_checkout_amount", 400);
                state.Intent.AmountInMinorUnits = checked((long)Math.Round(amount * 100, MidpointRounding.AwayFromZero));
                if (state.Intent.AmountInMinorUnits == 0)
                {
                    // Free promotions must go through the existing promotion path.
                    throw new MobileCheckoutException("use_promotion_redemption", 400);
                }
                var intent = _stripe.GetPaymentIntent(new StripeModel
                {
                    Amount = state.Intent.AmountInMinorUnits / 100d, Description = description
                }, "mobile:" + userId.ToString(CultureInfo.InvariantCulture) + ":" + request.RequestId.ToString("N"));
                state.Intent.PaymentIntentId = intent.Id;
                state.Intent.ClientSecret = intent.ClientSecret;
                state.Description = description;
                _records.Create(new UserPreference
                {
                    UserId = userId, Key = Prefix + request.RequestId.ToString("N"),
                    Value = MobileCheckoutProtection.Protect(JsonConvert.SerializeObject(state), userId, request.RequestId),
                    ValueType = typeof(string).AssemblyQualifiedName
                });
                transaction.Commit();
                return state.Intent;
            }
        }

        public MobileCheckoutIntent Complete(int userId, Guid requestId)
        {
            CheckIdentity(userId);
            if (requestId == Guid.Empty) throw new MobileCheckoutException("invalid_checkout_request", 400);
            // Serialize completion across sessions/workers using the existing EF
            // context and SQL transaction, not a process-local lock or session flag.
            using (var transaction = _database.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                var record = Find(userId, requestId);
                if (record == null) throw new MobileCheckoutException("checkout_not_found", 404);
                var state = Read(record, userId, requestId);
                if (state.UserId != userId) throw new MobileCheckoutException("checkout_not_found", 404);
                if (state.Intent.Completed) { transaction.Commit(); return state.Intent; }
                var payment = _stripe.GetPaymentIntentById(state.Intent.PaymentIntentId);
                if (payment == null || payment.Status != "succeeded") throw new MobileCheckoutException("payment_not_completed");
                if (payment.Amount != state.Intent.AmountInMinorUnits ||
                    !string.Equals(payment.Currency, state.Intent.Currency, StringComparison.OrdinalIgnoreCase))
                    throw new MobileCheckoutException("payment_quote_mismatch");
                var user = _package.UserService.Find(userId);
                if (user == null) throw new MobileCheckoutException("account_not_found", 404);
                var contact = _package.ContactService.GetOrCreateContact(payment.CustomerId, user.FullName, user.EmailAddress, user.PhoneNumber, userId);
                switch (state.Kind)
                {
                    case MobileCheckoutKind.Membership:
                        ValidatePromotion(userId, state.MembershipOptionId, state.PromoCode);
                        _package.MembershipService.ProcessPurchase(new PurchaseModel
                        {
                            ItemId = state.MembershipOptionId, Quantity = 1, ContactId = contact.Id,
                            CustomerName = user.FullName, CustomerEmailAddress = user.EmailAddress,
                            Amount = payment.Amount / 100d, Currency = payment.Currency, Status = payment.Status, Description = state.Description
                        });
                        if (!string.IsNullOrEmpty(state.PromoCode)) _promotions.UsePromotion(userId, state.PromoCode);
                        break;
                    case MobileCheckoutKind.Consultation:
                        var consultationId = _consultations.CreateConsultation(new Consultation
                        {
                            ConsultationDuration = state.Duration, ContactId = contact.Id
                        }, contact, userId);
                        // The legacy service catches some repository failures.
                        // Verify both records before marking checkout complete.
                        if (consultationId <= 0 || _consultations.FindUserConsultation(consultationId, userId) == null)
                            throw new MobileCheckoutException("consultation_creation_failed", 422);
                        state.Intent.ConsultationId = consultationId;
                        break;
                    default:
                        _donations.CreateDonation(new Donation
                        {
                            Customer = user.FullName, CustomerEmail = user.EmailAddress,
                            DonationAmount = payment.Amount / 100d, Currency = payment.Currency,
                            DonationDescription = state.Description, DonatedOn = DateTime.UtcNow
                        }, contact);
                        break;
                }
                state.Intent.Completed = true;
                state.Intent.ClientSecret = null;
                record.Value = MobileCheckoutProtection.Protect(JsonConvert.SerializeObject(state), userId, requestId);
                _records.Update(record);
                transaction.Commit();
                return state.Intent;
            }
        }

        private UserPreference Find(int userId, Guid requestId)
        {
            var key = Prefix + requestId.ToString("N");
            return _records.Find(e => e.UserId == userId && e.Key == key).SingleOrDefault();
        }

        private static CheckoutState Read(UserPreference record, int userId, Guid requestId)
        {
            var json = MobileCheckoutProtection.Unprotect(record.Value, userId, requestId);
            if (json == null) throw new MobileCheckoutException("invalid_checkout_state");
            var state = JsonConvert.DeserializeObject<CheckoutState>(json,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
            if (state?.Intent == null || state.UserId != userId || state.Intent.RequestId != requestId)
                throw new MobileCheckoutException("invalid_checkout_state");
            return state;
        }

        private static void CheckIdentity(int userId)
        {
            if (userId <= 0 || userId != Current.UserId || Current.IsImpersonating())
                throw new MobileCheckoutException("checkout_identity_mismatch", 403);
        }

        private void ValidatePromotion(int userId, int optionId, string code)
        {
            if (string.IsNullOrEmpty(code)) return;
            var promotion = _promotions.Find(code);
            if (promotion == null || promotion.MembershipOptionId != optionId ||
                (!promotion.IsReusable && _promotions.FindForUser(code, userId) == null) || _promotions.IsPromotionAlreadyUsed(code, userId))
                throw new MobileCheckoutException("invalid_promotion", 400);
        }

        private class CheckoutState
        {
            public CheckoutState() { }
            public int UserId { get; set; }
            public MobileCheckoutKind Kind { get; set; }
            public int MembershipOptionId { get; set; }
            public K9.DataAccessLayer.Enums.EConsultationDuration Duration { get; set; }
            public double DonationAmount { get; set; }
            public string PromoCode { get; set; }
            public string Description { get; set; }
            public MobileCheckoutIntent Intent { get; set; }
        }
    }
}
