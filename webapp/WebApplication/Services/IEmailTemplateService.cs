using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;

namespace K9.WebApplication.Services
{
    public interface IEmailTemplateService : IBaseService
    {
        EmailTemplate Find(int id);
        EmailTemplate FindSystemTemplate(ESystemEmailTemplate systemEmailTemplate);
        string ParseForUser(int emailTemplateId, User user, object data);
        string ParseForContact(int emailTemplateId, Contact contact, object data);
        string ParseForUser(EmailTemplate emailTemplate, User user, object data);
        string ParseForContact(EmailTemplate emailTemplate, Contact contact, object data);
        string ParseForUser(string title, string body, User user, object data);
        string ParseForContact(string title, string body, Contact contact, object data);
    }
}