using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers.Html;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RequirePermissions(Role = RoleNames.Administrators)]
    [RoutePrefix("articles")]
    public class ArticlesController : BaseNineStarKiController<Article>
    {
        private readonly IArticlesService _articlesService;
        private readonly IMediaManagementService _mediaManagementService;

        public ArticlesController(IControllerPackage<Article> controllerPackage, INineStarKiPackage nineStarKiPackage, IArticlesService articlesService, IMediaManagementService mediaManagementService)
            : base(controllerPackage, nineStarKiPackage)
        {
            _articlesService = articlesService;
            _mediaManagementService = mediaManagementService;

            RecordBeforeUpdate += ArticlesController_RecordBeforeUpdate;
        }

        private void ArticlesController_RecordBeforeUpdate(object sender, Base.WebApplication.EventArgs.CrudEventArgs e)
        {
            var article = e.Item as Article;
            var fullArticle = _articlesService.GetArticle(article.Id);
            article.Tags = fullArticle.Tags;
            article.TagsText = fullArticle.TagsText;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public override ActionResult Create(Article model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    HtmlMarkupHelper.ConvertToHtml(ref model);
                    _articlesService.CreateArticle(model);
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    Logger.Error($"Articles => Create => {e.GetFullErrorMessage()}");
                    ModelState.AddModelError("", e.GetFullErrorMessage());
                }
            }

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public override ActionResult Edit(Article model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    HtmlMarkupHelper.ConvertToHtml(ref model);
                    _articlesService.SaveArticle(model);
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    Logger.Error($"Articles => Edit => {e.GetFullErrorMessage()}");
                    ModelState.AddModelError("", e.GetFullErrorMessage());
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase file, int? articleId = null)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    Logger.Error("ArticlesController => UploadImage => No file received.");
                    return Json(new { success = false, message = "No file received." });
                }

                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var ext = Path.GetExtension(file.FileName);
                var safeFileName = fileName.Slugify() + "-" + Guid.NewGuid().ToString("N").Substring(0, 6) + ext;
                var relativePath = articleId.HasValue && articleId.Value > 0
                    ? $"~/Images/articles/{articleId}"
                    : "~/Images/articles";
                var uploadDir = Server.MapPath(relativePath);

                Directory.CreateDirectory(uploadDir);
                var path = Path.Combine(uploadDir, safeFileName);
                file.SaveAs(path);

                var url = Url.Content($"{relativePath}/{safeFileName}");

                Logger.Info($"ArticlesController => UploadImage => Successfully uploaded image to server: {url}");

#if DEBUG
                return Json(new { success = true, url });
#else
                Logger.Info($"ArticlesController => UploadImage => Uploading image to Storj");

                var uploadRelativePath = articleId.HasValue ? $"articles/{articleId}/{safeFileName}" : $"articles/{safeFileName}";
                var absoluteFilePath = Path.Combine(uploadDir, safeFileName);

                Logger.Info($"ArticlesController => UploadImage => UploadImageToStorj => {absoluteFilePath}, {uploadRelativePath}");

                var storjUrl = UploadImageToStorj(absoluteFilePath, uploadRelativePath);

                var finalUrl = string.IsNullOrWhiteSpace(storjUrl) ? url : storjUrl;
                Logger.Info($"UploadImage => Successfully uploaded: {finalUrl}");

                return Json(new { success = true, url = finalUrl });
#endif
            }
            catch (Exception ex)
            {
                Logger.Error("UploadImage => Exception: " + ex.GetFullErrorMessage());
                return Json(new { success = false, message = "Upload failed: " + ex.Message });
            }
        }
        
        [HttpPost]
        public ActionResult DeleteImages(DeleteFilesRequest request)
        {
            if (request?.Urls == null || !request.Urls.Any())
            {
                return Json(new { success = false, message = "No URLs provided." });
            }

            var failedDeletes = new List<string>();

            foreach (var relativePath in request.Urls)
            {
                try
                {
                    var localVirtualPath = $"~/{relativePath.TrimStart('/')}";
                    var fullPath = Server.MapPath(localVirtualPath);

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    else
                    {
                        failedDeletes.Add($"Not found: {relativePath}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"DeleteImages => Error deleting {relativePath}: {ex.Message}");
                    failedDeletes.Add($"Error: {relativePath}");
                }
            }

            if (failedDeletes.Any())
            {
                return Json(new
                {
                    success = false,
                    message = $"Some files failed to delete: {string.Join(", ", failedDeletes)}"
                });
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public PartialViewResult GetImageOptions(int? articleId = null)
        {
            var model = new ArticleDynamicFieldsViewModel(articleId);
            return PartialView("_ImageListItems", model);
        }

        public ActionResult Publish(int id)
        {
            var article = Repository.Find(id);
            article.PublishedOn = DateTime.UtcNow;
            Repository.Update(article);

            return RedirectToAction("PublishSuccess");
        }

        public ActionResult PublishSuccess()
        {
            return View();
        }

        public JsonResult GetAllTags()
        {
            return Json(_articlesService.GetAllTags().Select(t => new
            {
                value = t.Name,
                slug = t.Slug
            }), JsonRequestBehavior.AllowGet);
        }
        
        private string UploadImageToStorj(string absoluteFilePath, string relativePath)
        {
            return _mediaManagementService.UploadToStorj(absoluteFilePath, relativePath); // throws if fails
        }

    }
}