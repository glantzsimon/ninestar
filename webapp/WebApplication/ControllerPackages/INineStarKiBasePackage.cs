using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using NLog;
using System.Web.Mvc;

namespace K9.WebApplication.Packages
{
    public interface INineStarKiBasePackage
    {
        ILogger Logger { get; }
        IDataSetsHelper DataSetsHelper { get; }
        IRoles Roles { get; }
        IFileSourceHelper FileSourceHelper { get; }
        IAuthentication Authentication { get; }
        IMailer Mailer { get; }
        UrlHelper UrlHelper { get; }

        IRepository<User> UsersRepository { get; }
        IRepository<Contact> ContactsRepository { get; }
        IRepository<Role> RolesRepository { get; }
        IRepository<UserRole> UserRolesRepository { get; }

        DefaultValuesConfiguration DefaultValuesConfiguration { get; }
        SmtpConfiguration SmtpConfiguration { get; }
        ApiConfiguration ApiConfiguration { get; }
        WebsiteConfiguration WebsiteConfiguration { get; }
        GoogleConfiguration GoogleConfiguration { get; }

        SystemSetting SystemSettings { get; }
    }
}