using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Attributes;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [LimitByUserId]
    public class UserProfileReadingsController : BaseController<UserProfileReading>
    {
        public UserProfileReadingsController(IControllerPackage<UserProfileReading> controllerPackage)
            : base(controllerPackage)
        {
        }

        [Route("my-saved-charts")]
        public ActionResult MySavedProfiles()
        {
            return View("Index");
        }

        [Route("my-saved-charts/view")]
        public ActionResult ViewProfile(int id)
        {
            return RedirectToAction("ViewProfile", "NineStarKi", new { id });
        }
    }
}
