using HtmlAgilityPack;
using K9.Base.WebApplication.Filters;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.ViewModels;
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
        private readonly IRepository<UserMembership> _userMembershipsRepository;

        public AdminController(INineStarKiPackage nineStarKiPackage, IRepository<UserMembership> userMembershipsRepository)
            : base(nineStarKiPackage)
        {
            _userMembershipsRepository = userMembershipsRepository;
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

        public ActionResult DownloadTextFile(string fileName, string html)
        {
            var text = ExtractTextFromHtml(html);
            byte[] fileBytes = Encoding.UTF8.GetBytes(text);
            return File(fileBytes, "text/plain", fileName);
        }

        public ActionResult RunMaintenance()
        {
            AllocateFreeReadingsToAllAccounts();
            return RedirectToAction("MaintenanceComplete");
        }

        public ActionResult MaintenanceComplete()
        {
            return View();
        }

        private static string ExtractTextFromHtml(string htmlContent)
        {
            if (string.IsNullOrWhiteSpace(htmlContent))
                return string.Empty;

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);
            return doc.DocumentNode.InnerText.Trim();
        }

        private static void AppendFolderContents(StringBuilder htmlContent, string directory)
        {
            htmlContent.Append("<div class=\"well\">");
            htmlContent.AppendFormat("<h1>{0}</h1>", new DirectoryInfo(directory).Name);

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

        private void AllocateFreeReadingsToAllAccounts()
        {
            _userMembershipsRepository.GetQuery($"UPDATE [{nameof(UserMembership)}] " +
                                                $"SET [{nameof(UserMembership.ComplementaryPersonalChartReadingCount)}] = {UserMembership.TotalFreeReadings}, " +
                                                $"[{nameof(UserMembership.ComplementaryPredictionsReadingCount)}] = {UserMembership.TotalFreeReadings}, " +
                                                $"[{nameof(UserMembership.ComplementaryCompatibilityReadingCount)}] = {UserMembership.TotalFreeReadings}, " +
                                                $"[{nameof(UserMembership.ComplementaryKarmicReadingCount)}] = {UserMembership.TotalFreeReadings}");
        }
    }
}