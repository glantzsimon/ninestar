using K9.Base.DataAccessLayer.Config;
using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.EventArgs;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;
using WebMatrix.WebData;
using LogLevel = NLog.LogLevel;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RequirePermissions(Role = RoleNames.Administrators)]
    public class UsersController : BaseNineStarKiController<User>
    {
        private readonly IOptions<DatabaseConfiguration> _dataConfig;
        private readonly IPromoCodeService _promoCodeService;

        public UsersController(IControllerPackage<User> controllerPackage, INineStarKiPackage nineStarKiPackage, IOptions<DatabaseConfiguration> dataConfig, IPromoCodeService promoCodeService)
            : base(controllerPackage, nineStarKiPackage)
        {
            _dataConfig = dataConfig;
            _promoCodeService = promoCodeService;

            RecordCreated += UsersController_RecordCreated;
            RecordBeforeDeleted += UsersController_RecordBeforeDeleted;
            RecordUpdated += UsersController_RecordUpdated;
        }

        private void UsersController_RecordUpdated(object sender, CrudEventArgs e)
        {
            var user = (User)e.Item;
            var contact = Package.ContactsRepository.Find(c => c.EmailAddress == user.EmailAddress).FirstOrDefault();

            if (contact != null && user.IsUnsubscribed != contact.IsUnsubscribed)
            {
                contact.IsUnsubscribed = user.IsUnsubscribed;

                try
                {
                    Package.ContactsRepository.Update(contact);
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error,
                        $"UsersController => UsersController_RecordUpdated => Could not update contact => ContactId: {contact.Id} => Error: {ex.GetFullErrorMessage()}");
                    throw;
                }
            }
        }

        private void UsersController_RecordBeforeDeleted(object sender, CrudEventArgs e)
        {
            var user = e.Item as User;
            try
            {
                user.SetToDeleted();
                Repository.Update(user);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        void UsersController_RecordCreated(object sender, CrudEventArgs e)
        {
            var user = e.Item as User;
            WebSecurity.CreateAccount(user.Username, _dataConfig.Value.DefaultUserPassword);
            Roles.AddUserToRole(user.Username, RoleNames.DefaultUsers);
        }

        [Route("users/assign-membership/start")]
        public ActionResult AssignMembershipStart(int Id)
        {
            ViewBag.UserId = Id;
            return View(new AssignMembershipViewModel
            {
                UserId = Id
            });
        }

        [Route("users/assign-membership")]
        [HttpPost]
        public ActionResult AssignMembership(AssignMembershipViewModel model)
        {
            try
            {
                Package.MembershipService.AssignMembershipToUser(model.MembershipOptionId, model.UserId);
                return RedirectToAction("AssignMembershipSuccess");
            }
            catch (Exception ex)
            {
                Logger.Error($"UsersController => AssignMembership => Error: {ex.GetFullErrorMessage()}");
                throw;
            }
        }

        [Route("users/assign-membership/success")]
        public ActionResult AssignMembershipSuccess()
        {
            return View();
        }

        [Route("users/assign-consultation/start")]
        public ActionResult AssignFreeConsultationStart(int id)
        {
            ViewBag.UserId = id;
            return View(new AssignConsultationModel
            {
                UserId = id
            });
        }

        [Route("users/assign-consultation")]
        [HttpPost]
        public ActionResult AssignFreeConsultation(AssignConsultationModel model)
        {
            try
            {
                Package.MembershipService.CreateComplementaryUserConsultation(model.UserId, model.Duration);
                return RedirectToAction("AssignFreeConsultationSuccess");
            }
            catch (Exception ex)
            {
                Logger.Error($"UsersController => AssignFreeConsultation => Error: {ex.GetFullErrorMessage()}");
                throw;
            }
        }

        [Route("users/assign-consultation/success")]
        public ActionResult AssignFreeConsultationSuccess()
        {
            return View();
        }

        [Route("users/assign-promocode/start")]
        public ActionResult AssignPromoCodeStart(int Id)
        {
            ViewBag.UserId = Id;
            return View(new AssignPromoCodeViewModel
            {
                UserId = Id
            });
        }

        [Route("users/assign-promocode")]
        [HttpPost]
        public ActionResult AssignPromoCode(AssignPromoCodeViewModel model)
        {
            try
            {
                if (_promoCodeService.IsPromoCodeAlreadyUsed(model.PromoCode))
                {
                    ModelState.AddModelError("PromoCode", Globalisation.Dictionary.PromoCodeInUse);
                    return RedirectToAction("AssignPromoCodeStart", "Users", new { Id = model.UserId });
                };
            }
            catch (Exception e)
            {
                ModelState.AddModelError("PromoCode", e.Message);
                return RedirectToAction("AssignPromoCodeStart", "Users", new { Id = model.UserId });
            }

            try
            {
                Package.MembershipService.CreateMembershipFromPromoCode(model.UserId, model.PromoCode);
                return RedirectToAction("AssignPromoCodeSuccess");
            }
            catch (Exception ex)
            {
                Logger.Error($"UsersController => AssignPromoCode => Error: {ex.GetFullErrorMessage()}");
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction("AssignPromoCodeStart", "Users", new { Id = model.UserId });
            }
        }

        [Route("users/assign-promocode/success")]
        public ActionResult AssignPromoCodeSuccess()
        {
            return View();
        }

        [Authorize]
        public ActionResult ViewAccount(int Id)
        {
            return RedirectToAction("ViewAccount", "Account", new { userId = Id });
        }
    }
}
