using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Attributes;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [LimitByUserId]
    public class UserRelationshipCompatibilityReadingsController : BaseController<UserRelationshipCompatibilityReading>
    {
        public UserRelationshipCompatibilityReadingsController(IControllerPackage<UserRelationshipCompatibilityReading> controllerPackage)
            : base(controllerPackage)
        {
        }

        [Route("my-saved-compatibility-readings")]
        public ActionResult MySavedReadings()
        {
            return View("Index");
        }

        [Route("my-saved-compatibility-readings/view")]
        public ActionResult ViewReading(int id)
        {
            return RedirectToAction("ViewCompatibility", "NineStarKi", new { id });
        }
    }
}
