using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public interface IMailerService : IBaseService
    {
        void TestEmailTemplate(int id);
        void SendEmailTemplateToUser(int id, User user);
        void SendEmailTemplateToUsers(int id, List<User> users);
    }
}