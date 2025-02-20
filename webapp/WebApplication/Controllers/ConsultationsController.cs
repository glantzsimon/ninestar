using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.WebApplication.Packages;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RequirePermissions(Role = RoleNames.Administrators)]
    public class ConsultationsController : BaseNineStarKiController<Consultation>
    {
        public ConsultationsController(IControllerPackage<Consultation> controllerPackage, INineStarKiPackage nineStarKiPackage)
            : base(controllerPackage, nineStarKiPackage)
        {
            RecordBeforeDetails += ConsultationsController_RecordBeforeDetails;
        }

        private void ConsultationsController_RecordBeforeDetails(object sender, Base.WebApplication.EventArgs.CrudEventArgs e)
        {
            var consultation = e.Item as Consultation;
            consultation.Contact = Package.ContactsRepository.Find(consultation.ContactId);
        }
    }
}