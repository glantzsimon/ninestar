using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;

namespace K9.WebApplication.Services
{
    public interface IEmailTemplateService : IBaseService
    {
        EmailTemplate Find(int id);
        string ParseForUser(int emailTemplateId, string title, User user, object data);
        string ParseForContact(int emailTemplateId, string title, Contact contact, object data);
        string ParseForUser(string title, string body, User user, object data);
        string ParseForContact(string title, string body, Contact contact, object data);
    }
}