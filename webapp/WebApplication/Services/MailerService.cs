using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Exceptions;
using K9.WebApplication.Packages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Services
{
    public class MailerService : BaseService, IMailerService
    {
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IMailingListService _mailingListService;
        private readonly IPromotionService _promotionService;
        private readonly IEmailQueueService _emailQueueService;

        public MailerService(INineStarKiBasePackage my, IEmailTemplateService emailTemplateService, IMailingListService mailingListService, IPromotionService promotionService, IEmailQueueService emailQueueService)
            : base(my)
        {
            _emailTemplateService = emailTemplateService;
            _mailingListService = mailingListService;
            _promotionService = promotionService;
            _emailQueueService = emailQueueService;
        }

        public void TestEmailTemplate(int id)
        {
            var emailTemplate = _emailTemplateService.Find(id);
            if (emailTemplate == null)
            {
                My.Logger.Error($"MailerService => TestEmailTemplate => Email Template {id} not found");
                throw new NotFoundException();
            }
            var systemUser = My.UsersRepository.Find(e => e.Username == "SYSTEM").FirstOrDefault();
            SendEmailTemplateToUser(emailTemplate.Id, systemUser, true);
        }

        public void SendEmailTemplateToUser(int id, User user)
        {
            SendEmailTemplateToUser(id, user, false);
        }

        public void SendEmailTemplateToUsers(int id, List<User> users)
        {
            bool isError = false;

            try
            {
                var emailTemplate = _emailTemplateService.Find(id);
                if (emailTemplate == null)
                {
                    My.Logger.Error($"MailerService => SendEmailTemplateToUsers => Email Template {id} not found");
                    throw new NotFoundException();
                }

                if (emailTemplate.PromotionId.HasValue)
                {
                    var promotion = _promotionService.Find(emailTemplate.PromotionId.Value);

                    foreach (var user in users)
                    {
                        try
                        {
                            _promotionService.SendPromotionFromTemplateToUser(user.Id, emailTemplate, promotion, true, TimeSpan.FromMinutes(1));
                        }
                        catch (Exception e)
                        {
                            isError = true;
                            My.Logger.Error($"MailerService => SendEmailTemplateToUsers => {e.GetFullErrorMessage()}");
                        }
                    }
                }
                else
                {
                    foreach (User user in users)
                    {
                        try
                        {
                            var parsedTemplate = _emailTemplateService.Parse(
                                emailTemplate.Id,
                                user.FirstName,
                                My.UrlHelper.AbsoluteAction("UnsubscribeUser", "UsersController", new { externalId = user.Name }), null);

                            _emailQueueService.AddEmailToQueueForUser(emailTemplate.Id, user.Id, emailTemplate.Subject, parsedTemplate, EEmailType.General, TimeSpan.FromMinutes(1));
                        }
                        catch (Exception e)
                        {
                            isError = true;
                            My.Logger.Error($"MailerService => SendEmailTemplateToUsers => {e.GetFullErrorMessage()}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                isError = true;
                My.Logger.Error($"MailerService => SendEmailTemplateToUsers => {e.GetFullErrorMessage()}");
            }

            if (isError)
            {
                throw new Exception($"MailerService => SendEmailTemplateToUsers => An error occured when sending mail to users. Please check the logs for details.");
            }
        }

        private void SendEmailTemplateToUser(int id, User user, bool isTest)
        {
            try
            {
                var emailTemplate = _emailTemplateService.Find(id);
                if (emailTemplate == null)
                {
                    My.Logger.Error($"MailerService => SendEmailTemplateToUser => Email Template {id} not found");
                    throw new NotFoundException();
                }

                if (emailTemplate.PromotionId.HasValue)
                {
                    var promotion = _promotionService.Find(emailTemplate.PromotionId.Value);
                    _promotionService.SendPromotionFromTemplateToUser(user.Id, emailTemplate, promotion, true, TimeSpan.FromSeconds(1), isTest);
                }
                else
                {
                    var parsedTemplate = _emailTemplateService.Parse(
                        emailTemplate.Id,
                        user.FirstName,
                        My.UrlHelper.AbsoluteAction("UnsubscribeUser", "UsersController", new { externalId = user.Name }), null);

                    _emailQueueService.AddEmailToQueueForUser(emailTemplate.Id, user.Id, emailTemplate.Subject, parsedTemplate, EEmailType.General, TimeSpan.FromSeconds(1));
                }
            }
            catch (Exception e)
            {
                My.Logger.Error($"MailerService => SendEmailTemplateToUser => Error: {e.GetFullErrorMessage()}");
                throw;
            }
        }
    }
}