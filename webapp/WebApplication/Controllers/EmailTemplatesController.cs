using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RequirePermissions(Role = RoleNames.Administrators)]
    [Authorize]
    [RoutePrefix("emailtemplates")]
    public class EmailTemplatesController : BaseNineStarKiController<EmailTemplate>
    {
        private readonly IMailingListService _mailingListService;
        private readonly IMailerService _mailerService;

        public EmailTemplatesController(IControllerPackage<EmailTemplate> controllerPackage, INineStarKiPackage nineStarKiPackage, IMailingListService mailingListService, IMailerService mailerService)
            : base(controllerPackage, nineStarKiPackage)
        {
            _mailingListService = mailingListService;
            _mailerService = mailerService;
        }

        [Route("send-to-user")]
        public ActionResult SendToUser(int id)
        {
            var emailTemplate = Repository.Find(id);
            if (emailTemplate == null)
            {
                return HttpNotFound();
            }

            return View(new SendEmailTemplateViewModel
            {
                EmailTemplate = emailTemplate
            });
        }
        
        [Route("send-to-user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendToUser(SendEmailTemplateViewModel model)
        {
            EmailTemplate emailTemplate = null;

            if (ModelState.IsValid)
            {
                emailTemplate = Repository.Find(model.EmailTemplate.Id);
                if (emailTemplate == null)
                {
                    ModelState.AddModelError("", "Email Template not found");
                }

                if (!model.UserId.HasValue)
                {
                    ModelState.AddModelError("UserId", Base.Globalisation.Dictionary.FieldIsRequired);
                }

                if (ModelState.IsValid)
                {
                    var user = My.UsersRepository.Find(model.UserId.Value);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "User not found");
                    }
                    else
                    {
                        try
                        {
                            _mailerService.SendEmailTemplateToUser(emailTemplate.Id, user);
                            return View("SendEmailTemplateSuccess",new SendEmailTemplateViewModel
                            {
                                EmailTemplate = emailTemplate
                            });
                        }
                        catch (Exception e)
                        {
                            ViewBag.ErrorMessage = e.GetFullErrorMessage();
                            return View("SendEmailTemplateFailure",new SendEmailTemplateViewModel
                            {
                                EmailTemplate = emailTemplate
                            });
                        }
                    }
                }
            }
            return View(new SendEmailTemplateViewModel
            {
                EmailTemplate = emailTemplate
            });
        }

        [Route("send-to-mailinglist")]
        public ActionResult SendToMailingList(int id)
        {
            var emailTemplate = Repository.Find(id);
            if (emailTemplate == null)
            {
                return HttpNotFound();
            }

            ViewData["MailingListListItems"] = _mailingListService.GetMailingListListItems();
            return View(new SendEmailTemplateViewModel
            {
                EmailTemplate = emailTemplate
            });
        }

        [Route("send-to-mailinglist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendToMailingList(SendEmailTemplateViewModel model)
        {
            EmailTemplate emailTemplate = null;

            if (ModelState.IsValid)
            {
                emailTemplate = Repository.Find(model.EmailTemplate.Id);
                if (emailTemplate == null)
                {
                    ModelState.AddModelError("", "Email Template not found");
                }

                if (!model.MailingListId.HasValue)
                {
                    ModelState.AddModelError("MailingListId", Base.Globalisation.Dictionary.FieldIsRequired);
                }

                if (ModelState.IsValid)
                {
                    var mailingList = _mailingListService.Find(model.MailingListId.Value);
                    if (mailingList == null)
                    {
                        ModelState.AddModelError("", "Mailing List not found");
                    }
                    else
                    {
                        try
                        {
                            _mailerService.SendEmailTemplateToUsers(emailTemplate.Id, mailingList.Users);
                            return View("SendEmailTemplateSuccess",new SendEmailTemplateViewModel
                            {
                                EmailTemplate = emailTemplate
                            });
                        }
                        catch (Exception e)
                        {
                            ViewBag.ErrorMessage = e.GetFullErrorMessage();
                            return View("SendEmailTemplateFailure",new SendEmailTemplateViewModel
                            {
                                EmailTemplate = emailTemplate
                            });
                        }
                    }
                }
            }
            return View(new SendEmailTemplateViewModel
            {
                EmailTemplate = emailTemplate
            });
        }
        
        [Route("test")]
        public ActionResult TestEmailTemplate(int id)
        {
            var emailTemplate = Repository.Find(id);
            if (emailTemplate == null)
            {
                return HttpNotFound();
            }
            var systemUser = My.UsersRepository.Find(e => e.Username == "SYSTEM").FirstOrDefault();

            try
            {
                _mailerService.SendEmailTemplateToUser(emailTemplate.Id, systemUser);
                return View("SendEmailTemplateSuccess",new SendEmailTemplateViewModel
                {
                    EmailTemplate = emailTemplate
                });
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.GetFullErrorMessage();
                return View("SendEmailTemplateFailure",new SendEmailTemplateViewModel
                {
                    EmailTemplate = emailTemplate
                });
            }
        }
    }
}