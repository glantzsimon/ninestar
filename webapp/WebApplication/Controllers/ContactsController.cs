using K9.Base.WebApplication.Extensions;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using NLog;
using System;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RequirePermissions(Role = RoleNames.Administrators)]
    public class ContactsController : BaseNineStarKiController<Contact>
    {
        private readonly IRepository<Donation> _donationRepository;
        private readonly IMailChimpService _mailChimpService;
        
        public ContactsController(IControllerPackage<Contact> controllerPackage, INineStarKiControllerPackage nineStarKiControllerPackage,
            IRepository<Donation> donationRepository, IMailChimpService mailChimpService) 
            : base(controllerPackage, nineStarKiControllerPackage)
        {
            _donationRepository = donationRepository;
            _mailChimpService = mailChimpService;

            RecordUpdated += ContactsController_RecordUpdated;
        }

        private void ContactsController_RecordUpdated(object sender, Base.WebApplication.EventArgs.CrudEventArgs e)
        {
            var contact = (Contact)e.Item;
            var user = Package.UsersRepository.Find(u => u.EmailAddress == contact.EmailAddress).FirstOrDefault();

            if (user != null && user.IsUnsubscribed != contact.IsUnsubscribed)
            {
                user.IsUnsubscribed = contact.IsUnsubscribed;

                try
                {
                    Package.UsersRepository.Update(user);
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error,
                        $"ContactsController => ContactsController_RecordUpdated => Could not update user => UserId: {user.Id} => Error: {ex.GetFullErrorMessage()}");
                    throw;
                }
            }
        }

        public ActionResult ImportContactsFromDonations()
        {
            var existing = Repository.List();

            var contactsToImport = _donationRepository.Find(c => !string.IsNullOrEmpty(c.CustomerEmail) && existing.All(e => e.EmailAddress != c.CustomerEmail))
                .Select(e => new Contact
                {
                    FullName = e.CustomerName,
                    EmailAddress = e.CustomerEmail
                }).ToList();

            Repository.CreateBatch(contactsToImport);

            return RedirectToAction("Index");
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult SignUpToNewsLetter()
        {
            return View(new Contact());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult SignUpToNewsLetter(Contact contact)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Repository.Exists(_ => _.EmailAddress == contact.EmailAddress))
                    {
                        ModelState.AddModelError("EmailAddress", K9.Globalisation.Dictionary.DuplicateContactError);
                    }
                    else
                    {
                        Repository.Create(contact);
                        return RedirectToAction("SignUpSuccess");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.GetFullErrorMessage());
                    ModelState.AddErrorMessageFromException<Contact>(ex, contact);
                }
            }

            return View("", contact);
        }

        public ActionResult SignUpSuccess()
        {
            return View();
        }

        public ActionResult AddAllContactsToMailChimp()
        {
            _mailChimpService.AddAllContacts();
            return RedirectToAction("MailChimpImportSuccess");
        }

        public ActionResult MailChimpImportSuccess()
        {
            return View();
        }
    }
}
