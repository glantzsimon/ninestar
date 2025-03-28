﻿using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("learn-about-9-star-ki")]
    public class KnowledgeBaseController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;

        public KnowledgeBaseController(INineStarKiPackage nineStarKiPackage, INineStarKiService nineStarKiService)
            : base(nineStarKiPackage)
        {
            _nineStarKiService = nineStarKiService;
        }

        [Route("")]
        public ActionResult Index()
        {
            return View(_nineStarKiService.GetNineStarKiSummaryViewModel());
        }

        [Route("retrieve-last")]
        [Authorize]
        public ActionResult RetrieveLastKnowledgeBaseSection(bool todayOnly = false)
        {
            var lastKnowledgeBaseBookmark = SessionHelper.GetLastKnowledgeBase(todayOnly)?.Value;
            if (lastKnowledgeBaseBookmark != null)
            {
                return Redirect(Url.Action("Index") + $"#{lastKnowledgeBaseBookmark}");
            }
            return RedirectToAction("Index");
        }
        
        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}
