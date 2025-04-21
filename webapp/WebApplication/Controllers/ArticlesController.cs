using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers.Html;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RequirePermissions(Role = RoleNames.Administrators)]
    [RoutePrefix("articles")]
    public class ArticlesController : BaseNineStarKiController<Article>
    {
        private readonly IArticlesService _articlesService;

        public ArticlesController(IControllerPackage<Article> controllerPackage, INineStarKiPackage nineStarKiPackage, IArticlesService articlesService)
            : base(controllerPackage, nineStarKiPackage)
        {
            _articlesService = articlesService;

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
                    HtmlParser.ParseHtml(ref model);
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
                    HtmlParser.ParseHtml(ref model);
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
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var ext = Path.GetExtension(file.FileName);
                var safeFileName = Slugify(fileName) + "-" + Guid.NewGuid().ToString("N").Substring(0, 6) + ext;
                var relativePath = articleId.HasValue ? $"~/Images/articles/{articleId}" : "~/Images/articles";
                var uploadDir = Server.MapPath(relativePath);
                Directory.CreateDirectory(uploadDir);
                var path = Path.Combine(uploadDir, safeFileName);
                file.SaveAs(path);

                var url = Url.Content($"{relativePath}/{safeFileName}");
                return Json(new { success = true, url }, JsonRequestBehavior.AllowGet);
            }

            return new HttpStatusCodeResult(400, "No file received.");
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

        private string Slugify(string input)
        {
            return Regex.Replace(input.ToLower(), @"[^\w\-]", "-").Trim('-');
        }
    }
}