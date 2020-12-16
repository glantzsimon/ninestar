using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.Extensions;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Services;
using NLog;
using System;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RequirePermissions(Role = RoleNames.Administrators)]
    public class ContactsController : BaseController<Contact>
    {
        private readonly IRepository<Donation> _donationRepository;
        private readonly ILogger _logger;
        private readonly IMailChimpService _mailChimpService;

        public ContactsController(IControllerPackage<Contact> controllerPackage, IRepository<Donation> donationRepository, ILogger logger, IMailChimpService mailChimpService) : base(controllerPackage)
        {
            _donationRepository = donationRepository;
            _logger = logger;
            _mailChimpService = mailChimpService;
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
                    _logger.Error(ex.GetFullErrorMessage());
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
