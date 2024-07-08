using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RequirePermissions(Role = RoleNames.Administrators)]
    public class ConsultationsController : BaseController<Consultation>
    {
        private readonly IRepository<Contact> _contactRepository;

        public ConsultationsController(IControllerPackage<Consultation> controllerPackage, IRepository<Contact> contactRepository)
            : base(controllerPackage)
        {
            _contactRepository = contactRepository;
            RecordBeforeDetails += ConsultationsController_RecordBeforeDetails;
        }

        private void ConsultationsController_RecordBeforeDetails(object sender, Base.WebApplication.EventArgs.CrudEventArgs e)
        {
            var consultation = e.Item as Consultation;
            consultation.Contact = _contactRepository.Find(consultation.ContactId);
        }
    }
}