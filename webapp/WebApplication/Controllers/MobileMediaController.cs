using K9.WebApplication.Packages;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/media")]
    public class MobileMediaController : MobileApiControllerBase
    {
        // Multipart framing is additional to the 2 MiB image payload limit.
        protected override int MaximumRequestBytes => 2 * 1024 * 1024 + 65536;
        public MobileMediaController(INineStarKiPackage package) : base(package) { }

        [HttpPost, Route("avatar")]
        public ActionResult Avatar(HttpPostedFileBase file)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            var path = SaveImage(file, id, false);
            if (path == null) return ApiError(400, "invalid_image");
            var info = Package.UserService.GetOrCreateUserInfo(id);
            info.AvatarImageUrl = Url.Content("~" + path);
            Package.UserInfosRepository.Update(info);
            return ApiSuccess(new { Url = info.AvatarImageUrl });
        }

        [HttpPost, Route("avatar/delete")]
        public ActionResult DeleteAvatar()
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            var info = Package.UserService.GetOrCreateUserInfo(id);
            var oldUrl = info.AvatarImageUrl;
            info.AvatarImageUrl = null;
            Package.UserInfosRepository.Update(info);
            var folder = "/Images/mobile/avatars/" + id + "/";
            if (oldUrl?.StartsWith(Url.Content("~" + folder), StringComparison.Ordinal) == true)
                Package.MediaManagementService.DeleteMobileImage(folder + Path.GetFileName(oldUrl), id);
            return ApiSuccess(new { Deleted = true });
        }

        [HttpPost, Route("article-image")]
        public ActionResult ArticleImage(HttpPostedFileBase file)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            var path = SaveImage(file, id, true);
            return path == null ? (ActionResult)ApiError(400, "invalid_image") : ApiSuccess(new { Url = Url.Content("~" + path), Path = path });
        }

        [HttpPost, Route("article-image/delete")]
        public ActionResult DeleteArticleImage(string path)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            try { Package.MediaManagementService.DeleteMobileImage(path, id, true); }
            catch (ArgumentException) { return ApiError(400, "invalid_image_path"); }
            return ApiSuccess(new { Deleted = true });
        }

        private string SaveImage(HttpPostedFileBase file, int userId, bool article)
        {
            if (file == null || file.ContentLength <= 0 || file.ContentLength > 2 * 1024 * 1024) return null;
            using (var buffer = new MemoryStream())
            {
                var chunk = new byte[8192];
                int read;
                while ((read = file.InputStream.Read(chunk, 0, chunk.Length)) > 0)
                {
                    if (buffer.Length + read > 2 * 1024 * 1024) return null;
                    buffer.Write(chunk, 0, read);
                }
                try { return Package.MediaManagementService.SaveMobileImage(buffer.ToArray(), userId, article); }
                catch (ArgumentException) { return null; }
            }
        }
    }
}
