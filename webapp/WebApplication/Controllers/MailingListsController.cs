using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    public class MailingListsController : BaseController<MailingList>
    {

        public MailingListsController(IControllerPackage<MailingList> controllerPackage)
            : base(controllerPackage)
        {
        }

    }
}