using System.Web.Mvc;
using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Services;
using NLog;

namespace K9.WebApplication.Packages
{
    public interface INineStarKiPackage
    {
        ILogger Logger { get; set; }
        IDataSetsHelper DataSetsHelper { get; set; }
        IRoles Roles { get; set; }
        IFileSourceHelper FileSourceHelper { get; set; }
        IAuthentication Authentication { get; set; }
        IMailer Mailer { get; set; }
        UrlHelper UrlHelper { get; set; }

        IMembershipService MembershipService { get; set; }
        IAccountService AccountService { get; set; }
        IUserService UserService { get; set; }
        IContactService ContactService { get; set; }

        IRepository<User> UsersRepository { get; set; }
        IRepository<Contact> ContactsRepository { get; set; }
        IRepository<Role> RolesRepository { get; set; }
        IRepository<UserRole> UserRolesRepository { get; set; }

        DefaultValuesConfiguration DefaultValuesConfiguration { get; set; }
        SmtpConfiguration SmtpConfiguration { get; set; }
        ApiConfiguration ApiConfiguration { get; set; }
        WebsiteConfiguration WebsiteConfiguration { get; set; }
        GoogleConfiguration GoogleConfiguration { get; set; }
    }
}