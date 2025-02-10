using K9.Base.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using K9.WebApplication.Services.Stripe;
using NLog;
using System;
using System.Web.Mvc;
using StripeConfiguration = K9.WebApplication.Config.StripeConfiguration;

namespace K9.WebApplication.Controllers
{
    public class PaymentsController : BaseNineStarKiController
    {
        private readonly ILogger _logger;
        private readonly IStripeService _stripeService;
        private readonly IContactService _contactService;
        private readonly IUserService _userService;
        private readonly StripeConfiguration _stripeConfig;

        public PaymentsController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IStripeService stripeService, IOptions<StripeConfiguration> stripeConfig, IMembershipService membershipService, IContactService contactService, IUserService userService, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService, rolesRepository, userRolesRepository)
        {
            _logger = logger;
            _stripeService = stripeService;
            _contactService = contactService;
            _userService = userService;
            _stripeConfig = stripeConfig.Value;
        }

        [Route("payments/start")]
        [HttpPost]
        public ActionResult GetPaymentIntent(double amount, string description)
        {
            var intent = _stripeService.GetPaymentIntent(new StripeModel
            {
                Amount = amount,
                Description = description
            });

            return Json(new
            {
                _stripeConfig.PublishableKey,
                intent.ClientSecret
            });
        }

        [HttpPost]
        [Route("payments/process")]
        public ActionResult ProcessPayment(int id, int quantity, string paymentIntentId, string fullName, string emailAddress, string phoneNumber = "")
        {
            try
            {
                var result = _stripeService.GetPaymentIntentById(paymentIntentId);
                var contact = _contactService.GetOrCreateContact("", fullName, emailAddress);
                _userService.UpdateActiveUserEmailAddressIfFromFacebook(contact);

                return Json(new
                {
                    success = true,
                    purchaseModel = new PurchaseModel
                    {
                        ItemId = id,
                        Quantity = quantity,
                        ContactId = contact.Id,
                        CustomerName = fullName,
                        CustomerEmailAddress = emailAddress,
                        Amount = result.Amount > 0 ? (double)result.Amount / 100 : 0,
                        Description = result.Description,
                        Currency = result.Currency?.ToUpper() ?? "USD",
                        Status = result.Status
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.Error($"PaymentController => ProcessPayment => Error: {ex.GetFullErrorMessage()}");
                return Json(new
                {
                    success = false,
                    errorMsg = ex.Message
                });
            }
        }

        public override string GetObjectName()
        {
            throw new NotImplementedException();
        }
    }
}
