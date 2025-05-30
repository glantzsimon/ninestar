﻿using System;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using System.Web.Mvc;
using K9.Base.WebApplication.Filters;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Helpers;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("latest-articles")]
    public partial class BlogController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;
        private readonly IArticlesService _articlesService;

        public BlogController(INineStarKiPackage nineStarKiPackage, INineStarKiService nineStarKiService, IArticlesService articlesService)
            : base(nineStarKiPackage)
        {
            _nineStarKiService = nineStarKiService;
            _articlesService = articlesService;
        }

        [Route("")]
        public ActionResult Index()
        {
            return View(new BlogViewModel
            {
                Articles = _articlesService.GetArticles(true),
                Tags = _articlesService.GetAllTags()
            });
        }

        [Route("latest-articles/{id:int}/{slug?}")]
        public ActionResult Redirection(int id, string slug = null)
        {
            var article = _articlesService.GetArticle(id);
            TempData.Keep();
            return RedirectToAction("Details", new { id = article.Id, slug = article.Slug });
        }

        public ActionResult View(int id)
        {
            var article = _articlesService.GetArticle(id);
            TempData.Keep();
            return RedirectToAction("Details", new { id = article.Id, slug = article.Slug });
        }

        [Route("{id:int}/{slug?}")]
        public ActionResult Details(int id, string slug = null)
        {
            var article = _articlesService.GetArticle(id);
            if (article == null || (!article.PublishedOn.HasValue && !SessionHelper.CurrentUserIsAdmin()))
            {
                return HttpNotFound();
            }

            var correctSlug = article.Title.Slugify();
            if (!string.Equals(slug, correctSlug, StringComparison.OrdinalIgnoreCase))
            {
                // Redirect to canonical URL with slug
                return RedirectToRoutePermanent("Default", new { controller = "Blog", action = "Details", id, slug = correctSlug });
            }

            return View(article);
        }

        [Route("post-comment")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult PostComment(ArticleComment model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.GetErrorMessage();
                TempData["ErrorMessage"] = error ?? Dictionary.FriendlyErrorMessage;
                TempData["IsError"] = true;
                return RedirectToAction("View", new { id = model.ArticleId });
            }

            var comment = new ArticleComment
            {
                ArticleId = model.ArticleId,
                UserId = Current.UserId,
                Comment = model.Comment,
                IsApproved = false
            };

            try
            {
                _articlesService.CreateArticleComment(comment);
            }
            catch (Exception e)
            {
                Logger.Error(e.GetFullErrorMessage());
                TempData["ErrorMessage"] = Dictionary.FriendlyErrorMessage;
                TempData["IsError"] = true;
                return RedirectToAction("View", new { id = model.ArticleId });
            }

            TempData["IsSuccess"] = true;
            return RedirectToAction("View", new { id = model.ArticleId });
        }

        [Route("toggle-like")]
        [HttpPost]
        [Authorize]
        public JsonResult ToggleCommentLike(int articleId, int? articleCommentId = null)
        {
            try
            {
                var result = _articlesService.ToggleLike(articleId, articleCommentId);
                return Json(new
                {
                    success = true,
                    newCount = result.Count,
                    toggleState = result.ToggleState,
                    likeSummary = result.LikeSummary
                });
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = e.GetFullErrorMessage() });
            }
        }

        [Route("delete-comment")]
        [HttpPost]
        [Authorize]
        public JsonResult DeleteComment(int id)
        {
            try
            {
                _articlesService.DeleteComment(id);
            }
            catch (UnauthorizedAccessException e)
            {
                return Json(new { success = false, message = "Not authorized." });
            }

            return Json(new { success = true });
        }

        [Route("edit-comment")]
        [HttpPost]
        [Authorize]
        public JsonResult EditComment(int id, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                return Json(new { success = false, message = "Comment can't be empty." });

            try
            {
                _articlesService.EditComment(id, comment);
            }
            catch (UnauthorizedAccessException e)
            {
                return Json(new { success = false, message = "Not authorized." });
            }

            return Json(new { success = true });
        }

        [Route("moderate/dashboard")]
        [Authorize]
        [RequirePermissions(Role = RoleNames.Administrators)]
        public ActionResult Dashboard()
        {
            return View(_articlesService.GetDashboardViewModel());
        }

        [HttpPost]
        [Route("moderate/approve-comment")]
        [Authorize]
        [RequirePermissions(Role = RoleNames.Administrators)]
        public JsonResult ApproveComment(int id)
        {
            try
            {
                _articlesService.ApproveComment(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        [Route("moderate/reject-comment")]
        [Authorize]
        [RequirePermissions(Role = RoleNames.Administrators)]
        public JsonResult RejectComment(int id)
        {
            try
            {
                _articlesService.RejectComment(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        [Route("moderate/save-comment")]
        [Authorize]
        [RequirePermissions(Role = RoleNames.Administrators)]
        public JsonResult SaveComment(int id, string comment)
        {
            try
            {
                _articlesService.EditComment(id, comment);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        [Route("moderate/delete-comment")]
        [Authorize]
        [RequirePermissions(Role = RoleNames.Administrators)]
        public JsonResult ModerateDeleteComment(int id)
        {
            try
            {
                _articlesService.DeleteComment(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}