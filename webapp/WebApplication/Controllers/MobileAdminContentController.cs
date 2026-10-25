using HtmlAgilityPack;
using K9.WebApplication.Packages;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/admin/content")]
    public class MobileAdminContentController : MobileApiControllerBase
    {
        public MobileAdminContentController(INineStarKiPackage package) : base(package) { }

        // Matches the existing admin export's deployed Globalisation location.
        // A client can choose only a name from the immediate directory catalog.
        private static string ResourceRoot => Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "Globalisation"));

        [HttpGet, Route("folders")]
        public ActionResult Folders()
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            if (!Directory.Exists(ResourceRoot)) return ApiError(404, "resource_sources_not_deployed");
            return ApiSuccess(Directory.GetDirectories(ResourceRoot).Select(Path.GetFileName).OrderBy(e => e).ToArray());
        }

        [HttpGet, Route("files")]
        public ActionResult Files(string folder, string after = "", int limit = 20)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            if (!ModelState.IsValid || string.IsNullOrEmpty(folder) || folder.Length > 128 ||
                (after?.Length ?? 0) > 1000 || limit < 1 || limit > 100) return ApiError(400, "invalid_resource_page");
            if (!Directory.Exists(ResourceRoot)) return ApiError(404, "resource_sources_not_deployed");
            var directory = Directory.GetDirectories(ResourceRoot)
                .FirstOrDefault(e => string.Equals(Path.GetFileName(e), folder, StringComparison.OrdinalIgnoreCase));
            if (directory == null) return ApiError(404, "resource_folder_not_found");
            var paths = Directory.GetFiles(directory, "*.htm", SearchOption.AllDirectories)
                .Select(e => new { FullPath = e, Key = e.Substring(directory.Length + 1).Replace('\\', '/') })
                .Where(e => string.CompareOrdinal(e.Key, after ?? "") > 0).OrderBy(e => e.Key, StringComparer.Ordinal)
                .Take(limit + 1).ToArray();
            long bytes = 0;
            var page = paths.Take(limit).TakeWhile(e =>
            {
                bytes += new FileInfo(e.FullPath).Length;
                return bytes <= 1024 * 1024;
            }).ToArray();
            if (paths.Length > 0 && page.Length == 0) return ApiError(422, "resource_file_too_large");
            var items = page.Select(e =>
            {
                var html = System.IO.File.ReadAllText(e.FullPath);
                var document = new HtmlDocument();
                document.LoadHtml(html);
                return new { File = e.Key, HtmlBody = html, Text = document.DocumentNode.InnerText.Trim() };
            }).ToArray();
            return ApiSuccess(new
            {
                Folder = Path.GetFileName(directory), Items = items,
                NextAfter = page.LastOrDefault()?.Key, HasMore = paths.Length > page.Length
            });
        }
    }
}
