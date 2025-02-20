using System.Web.Mvc;
using K9.WebApplication.Helpers;

namespace K9.WebApplication.Controllers
{
    public partial class AccountController
    {
        [Route("account/is-username-available")]
        public JsonResult IsUserNameAvailable(string username)
        {
            return Json(Package.UsersRepository.Exists(u => u.Username == username), JsonRequestBehavior.AllowGet);
        }

        [Route("account/is-email-available")]
        public JsonResult IsEmailAddressAvailable(string emailAddress)
        {
            return Json(Package.UsersRepository.Exists(u => u.EmailAddress == emailAddress), JsonRequestBehavior.AllowGet);
        }

        [Route("account/set-user-timezone")]
        public JsonResult SetCurrentUserTimeZone(string value)
        {
            SessionHelper.SetCurrentUserTimeZone(value);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [Route("account/resend-activation-code")]
        [HttpPost]
        public JsonResult ResendActivationCode(int userId)
        {
            var user = Package.UserService.Find(userId);
            if (user == null)
            {
                return Json(new { success = false, error = "User not found" }, JsonRequestBehavior.AllowGet);
            }

            if (user.Id != Current.UserId)
            {
                return Json(new { success = false, error = "Invalid UserId" }, JsonRequestBehavior.AllowGet);
            }

            var otp = Package.AccountService.CreateAccountActivationOTP(userId, true);
            return Json(new
            {
                success = true
            }, JsonRequestBehavior.AllowGet);
        }
    }
}