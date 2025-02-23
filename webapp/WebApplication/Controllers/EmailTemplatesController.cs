using System;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RequirePermissions(Role = RoleNames.Administrators)]
    [Authorize]
    [RoutePrefix("emailtemplates")]
    public class EmailTemplatesController : BaseNineStarKiController<EmailTemplate>
    {
        private readonly IMailingListService _mailingListService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IPromotionService _promotionService;
        private readonly IRepository<MembershipOption> _membershipOptionsRepository;

        public EmailTemplatesController(IControllerPackage<EmailTemplate> controllerPackage, INineStarKiPackage nineStarKiPackage, IMailingListService mailingListService, IEmailTemplateService emailTemplateService, IPromotionService promotionService, IRepository<MembershipOption> membershipOptionsRepository)
            : base(controllerPackage, nineStarKiPackage)
        {
            _mailingListService = mailingListService;
            _emailTemplateService = emailTemplateService;
            _promotionService = promotionService;
            _membershipOptionsRepository = membershipOptionsRepository;
        }

        [Route("send-to-user")]
        public ActionResult SendToUser(int id)
        {
            var emailTemplate = Repository.Find(id);
            if (emailTemplate == null)
            {
                return HttpNotFound();
            }

            return View(emailTemplate);
        }

        public ActionResult SendToMailingList(int id)
        {
            var emailTemplate = Repository.Find(id);
            if (emailTemplate == null)
            {
                return HttpNotFound();
            }

            ViewData["MailingListListItems"] = _mailingListService.GetMailingListListItems();
            return View(emailTemplate);
        }

        [Route("test")]
        public ActionResult TestEmailTemplate(int id)
        {
            try
            {
                var systemUser = My.UsersRepository.Find(e => e.Username == "SYSTEM").FirstOrDefault();
                var emailTemplate = Repository.Find(id);
                if (emailTemplate == null)
                {
                    return HttpNotFound();
                }

                if (emailTemplate.PromotionId.HasValue)
                {
                    var promotion = _promotionService.Find(emailTemplate.PromotionId.Value);
                    _promotionService.SendPromotionFromTemplateToUser(systemUser.Id, emailTemplate, promotion);
                }
                else
                {
                    MembershipOption option = null;
                    if (emailTemplate.MembershipOptionId.HasValue)
                    {
                        option = _membershipOptionsRepository.Find(emailTemplate.MembershipOptionId.Value);
                    }

                    object data = null;
                    if (option != null)
                    {
                        data = new
                        {
                            DiscountPercent = 0,
                            FormattedFullPrice = option.FormattedPrice,
                            FormattedSpecialPrice = option.FormattedPrice,
                            MembershipName = option.SubscriptionTypeNameLocal,
                            PromoLink = "https://9starkiastrology.com"
                        };
                    }

                    var parsedTemplate = _emailTemplateService.Parse(
                        emailTemplate.Id,
                        systemUser.FirstName,
                        My.UrlHelper.AbsoluteAction("UnsubscribeUser", "UsersController", new { externalId = systemUser.Name }), data);
                }
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.GetFullErrorMessage();
                return View("TestEmailTemplateFailure");
            }

            return View("TestEmailTemplateSuccess");
        }
    }
}