﻿using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System;
using System.Web.Mvc;
using System.Web.UI;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("relationship-compatibility")]
    public partial class CompatibilityController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;

        public CompatibilityController(INineStarKiPackage nineStarKiPackage, INineStarKiService nineStarKiService)
            : base(nineStarKiPackage)
        {
            _nineStarKiService = nineStarKiService;
        }

        [Route("calculator")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult Index()
        {
            var dateOfBirth1 = new DateTime(DateTime.Now.Year - (27), DateTime.Now.Month, DateTime.Now.Day);
            var dateOfBirth2 = new DateTime(DateTime.Now.Year - (27), DateTime.Now.Month, DateTime.Now.Day).AddMonths(2);
            var personModel1 = new PersonModel
            {
                DateOfBirth = dateOfBirth1,
                Gender = Methods.GetRandomGender()
            };
            var personModel2 = new PersonModel
            {
                DateOfBirth = dateOfBirth2,
                Gender = Methods.GetRandomGender()
            };
            return View(new CompatibilityModel(new NineStarKiModel(personModel1), new NineStarKiModel(personModel2)));
        }

        [Route("calculator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CompatibilityModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.NineStarKiModel1?.PersonModel != null && model.NineStarKiModel2?.PersonModel != null)
                {
                    model = _nineStarKiService.CalculateCompatibility(model.NineStarKiModel1.PersonModel, model.NineStarKiModel2.PersonModel, model.IsHideSexualChemistry);
                }
            }
            return View(model);
        }

        [Route("retrieve-last")]
        [Authorize]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult RetrieveLastCompatibility(bool todayOnly = false)
        {
            var lastCompatibility = SessionHelper.GetLastCompatibility(todayOnly)?.CompatibilityModel;
            if (lastCompatibility == null)
            {
                return RedirectToAction("Index");
            }
            var model = _nineStarKiService.CalculateCompatibility(lastCompatibility.NineStarKiModel1.PersonModel, lastCompatibility.NineStarKiModel2.PersonModel, lastCompatibility.IsHideSexualChemistry);
            return View("Index", model);
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}

