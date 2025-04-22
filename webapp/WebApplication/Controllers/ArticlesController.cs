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
            RecordBeforeDelete += ArticlesController_RecordBeforeDelete;
        }

        private void ArticlesController_RecordBeforeDelete(object sender, Base.WebApplication.EventArgs.CrudEventArgs e)
        {
            LoadFullArticle(e.Item as Article);
        }

        private void ArticlesController_RecordBeforeUpdate(object sender, Base.WebApplication.EventArgs.CrudEventArgs e)
        {
            LoadFullArticle(e.Item as Article);
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

        [ValidateAntiForgeryToken]
        [HttpPost]
        public override ActionResult DeleteConfirmed(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _articlesService.DeleteArtciel(id);
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    Logger.Error($"Articles => Edit => {e.GetFullErrorMessage()}");
                    ModelState.AddModelError("", e.GetFullErrorMessage());
                }
            }

            return View("Delete", _articlesService.GetArticle(id));
        }
        
        public ActionResult Preview(int id)
        {
            return RedirectToAction("Preview", "Blog", new { id });
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

        private void LoadFullArticle(Article article)
        {
            var fullArticle = _articlesService.GetArticle(article.Id);
            article.Tags = fullArticle.Tags;
            article.TagsText = fullArticle.TagsText;
        }
        
    }
}