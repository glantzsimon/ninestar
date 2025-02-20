using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.WebApplication.Packages;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    public class EmailTemplatesController : BaseNineStarKiController<EmailTemplate>
    {

        public EmailTemplatesController(IControllerPackage<EmailTemplate> controllerPackage, INineStarKiPackage nineStarKiPackage)
            : base(controllerPackage, nineStarKiPackage)
        {
        }

    }
}