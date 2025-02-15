using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Config;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.Services;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RequirePermissions(Role = RoleNames.Administrators)]
    public class AdminController : BaseNineStarKiController
    {
        private readonly IRepository<User> _userRepository;
        private readonly ILogger _logger;
        private readonly Services.IAccountService _accountService;
        private readonly IAuthentication _authentication;
        private readonly IFacebookService _facebookService;
        private readonly IMembershipService _membershipService;
        private readonly IContactService _contactService;
        private readonly IUserService _userService;
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly IRecaptchaService _recaptchaService;
        private readonly RecaptchaConfiguration _recaptchaConfig;

        public AdminController(IRepository<User> userRepository, ILogger logger, IMailer mailer, IOptions<WebsiteConfiguration> websiteConfig, IDataSetsHelper dataSetsHelper, IRoles roles, Services.IAccountService accountService, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IFacebookService facebookService, IMembershipService membershipService, IContactService contactService, IUserService userService, IRepository<PromoCode> promoCodesRepository, IOptions<RecaptchaConfiguration> recaptchaConfig, IRecaptchaService recaptchaService, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService, rolesRepository, userRolesRepository)
        {
            _userRepository = userRepository;
            _logger = logger;
            _accountService = accountService;
            _authentication = authentication;
            _facebookService = facebookService;
            _membershipService = membershipService;
            _contactService = contactService;
            _userService = userService;
            _promoCodesRepository = promoCodesRepository;
            _recaptchaService = recaptchaService;
            _recaptchaConfig = recaptchaConfig.Value;
        }

        [Route("admin/display/all-content-files/{folder}/")]
        public ActionResult DisplayCompleteGlobalisationContents(string folder, bool download = false)
        {
            var solutionRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".."));
            var projectPath = Path.Combine(solutionRoot, "Globalisation");

            if (!Directory.Exists(projectPath))
            {
                return View("ViewContent", new AdminViewModel
                {
                    Content = $"<p>Error: Globalisation directory not found: {projectPath}</p>"
                });
            }

            var htmlContent = new StringBuilder();
            htmlContent.Append("<div class=\"admin-contents-container\">");

            var directories = Directory.GetDirectories(projectPath).ToList();
            if (string.IsNullOrEmpty(folder))
            {
                foreach (var directory in directories)
                {
                    AppendFolderContents(htmlContent, directory);
                }
            }
            else
            {
                var directory = directories.FirstOrDefault(e => new DirectoryInfo(e).Name.ToLower() == folder.ToLower());
                if (directory != null)
                {
                    AppendFolderContents(htmlContent, directory);
                }
                else
                {
                    htmlContent.Append($"<p>Error: Directory not found: {folder}</p>");
                }
            }

            htmlContent.Append("</div>");

            if (download)
            {
                return DownloadTextFile($"{folder}.txt", htmlContent.ToString());
            }

            return View("ViewContent", new AdminViewModel
            {
                Content = htmlContent.ToString()
            });
        }

        public ActionResult DownloadTextFile(string fileName, string contents)
        {
            byte[] fileBytes = Encoding.UTF8.GetBytes(contents);
            return File(fileBytes, "text/plain", fileName);
        }

        private static void AppendFolderContents(StringBuilder htmlContent, string directory)
        {
            htmlContent.Append("<div class=\"well\">");
            htmlContent.AppendFormat("<h1>{0}</h2>", new DirectoryInfo(directory).Name);

            var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".htm"));

            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                string content = System.IO.File.ReadAllText(file);

                htmlContent.AppendFormat("<h4>{0}</h4>", fileName);
                htmlContent.AppendFormat("<div>{0}</div>", content);
                htmlContent.Append("<hr />");
            }

            htmlContent.Append("</div");
        }
    }
}