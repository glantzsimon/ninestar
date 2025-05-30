﻿using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System;
using System.Web.Mvc;
using System.Web.UI;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("biorhythms")]
    public partial class BiorhythmsController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;
        private readonly IBiorhythmsService _biorhythmsService;

        public BiorhythmsController(INineStarKiPackage nineStarKiPackage, INineStarKiService nineStarKiService, IBiorhythmsService biorhythmsService)
            : base(nineStarKiPackage)
        {
            _nineStarKiService = nineStarKiService;
            _biorhythmsService = biorhythmsService;
        }

        [Route("calculator")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", VaryByHeader="User-Agent", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult Index()
        {
            return View(new BioRhythmsModel(new NineStarKiModel(), DateTime.Now));
        }

        [Route("calculator/result")]
        [HttpPost]
        public ActionResult IndexPost(BioRhythmsModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.PersonModel != null || model.SelectedDate != DateTime.Today)
                {
                    var selectedDate = model.SelectedDate ?? DateTime.Today;

                    var nineStarKiModel = _nineStarKiService.CalculateNineStarKiProfile(model.PersonModel, false, false, selectedDate);
                    nineStarKiModel.SelectedDate = selectedDate;

                    var bioRhythmsModel = _biorhythmsService.Calculate(nineStarKiModel, selectedDate);

                    return View("Index", bioRhythmsModel);
                }
            }

            model.NineStarKiModel = new NineStarKiModel(model.PersonModel);
            return View("Index", model);
        }

        [Route("retrieve-last")]
        [Authorize]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult RetrieveLastBiorhythms(bool todayOnly = false)
        {
            var lastBiorhythms = SessionHelper.GetLastBiorhythm(todayOnly)?.PersonModel;
            if (lastBiorhythms == null)
            {
                return RedirectToAction("Index");
            }
            var model = _nineStarKiService.CalculateNineStarKiProfile(lastBiorhythms.DateOfBirth, lastBiorhythms.Gender);
            return View("Index", model);
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}

