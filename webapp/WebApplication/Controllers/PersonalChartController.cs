﻿using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Enums;
using K9.WebApplication.Enums;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System;
using System.Text;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public partial class PersonalChartController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;
        
        public PersonalChartController(INineStarKiPackage nineStarKiPackage, INineStarKiService nineStarKiService)
            : base(nineStarKiPackage)
        {
            _nineStarKiService = nineStarKiService;
        }

        [Route("personalchart/calculate")]
        public ActionResult Index()
        {
            var dateOfBirth = new DateTime(DateTime.Now.Year - (27), DateTime.Now.Month, DateTime.Now.Day);
            var personModel = new PersonModel
            {
                DateOfBirth = dateOfBirth,
                Gender = Methods.GetRandomGender()
            };
            return View(new NineStarKiModel(personModel));
        }

        [Route("personalchart/calculate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(NineStarKiModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.PersonModel != null || model.SelectedDate != DateTime.Today)
                {
                    var selectedDate = model.SelectedDate ?? DateTime.Today;
                    var isScrollToCyclesOverview = model.IsScrollToCyclesOverview;
                    var activeTabId = model.ActiveCycleTabId;

                    model = _nineStarKiService.CalculateNineStarKiProfile(model.PersonModel, false, false,
                        selectedDate);
                    model.SelectedDate = selectedDate;
                    model.IsScrollToCyclesOverview = isScrollToCyclesOverview;
                    model.ActiveCycleTabId = activeTabId;
                }
            }

            return View(model);
        }

        [Authorize]
        [Route("personalchart/my-chart")]
        public ActionResult MyProfile()
        {
            var myAccount = My.UsersRepository.Find(Current.UserId);
            var personModel = new PersonModel
            {
                Name = myAccount.FullName,
                DateOfBirth = myAccount.BirthDate,
                Gender = myAccount.Gender
            };
            var nineStarKiProfile = _nineStarKiService.CalculateNineStarKiProfile(personModel, false, true);

            nineStarKiProfile.IsMyProfile = true;

            return View("Index", nineStarKiProfile);
        }

        [Route("retrieve-last")]
        [Authorize]
        public ActionResult RetrieveLast()
        {
            var retrieveLast = TempData["RetrieveLast"].ToString();
            Methods.TryGetEnumProperty<ESection>(retrieveLast, nameof(EnumDescriptionAttribute.CultureCode), out var section);

            switch (section)
            {
                case ESection.Profile:
                    return RedirectToAction("RetrieveLastProfile");

                case ESection.Compatibility:
                    return RedirectToAction("RetrieveLastCompatibility", "Compatibility");

                case ESection.Predictions:
                    return RedirectToAction("RetrieveLastPredictions", "Predictions");

                case ESection.Biorhythms:
                    return RedirectToAction("RetrieveLastBiorhythms", "Biorhythms");

                case ESection.KnowledgeBase:
                    return RedirectToAction("RetrieveLastKnowledgeBaseSection", "KnowledgeBase");

                default:
                    return RedirectToAction("Index");
            }
        }

        [Route("personalchart/retrieve-last")]
        [Authorize]
        public ActionResult RetrieveLastProfile(bool todayOnly = false)
        {
            var lastProfile = SessionHelper.GetLastProfile(todayOnly).PersonModel;
            if (lastProfile == null)
            {
                return RedirectToAction("Index");
            }

            var personModel = new PersonModel
            {
                Name = lastProfile.Name,
                DateOfBirth = lastProfile.DateOfBirth,
                Gender = lastProfile.Gender
            };
            var model = _nineStarKiService.CalculateNineStarKiProfile(personModel);
            
            return View("Index", model);
        }

        [Route("list/allenegies")]
        public ContentResult GetAllEnergies()
        {
            var sb = new StringBuilder();
            for (var i = 1; i <= 9; i++)
            {
                var maleModel = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
                {
                    DateOfBirth = new DateTime(1979, i, 15),
                    Gender = EGender.Male
                });

                var femaleModel = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
                {
                    DateOfBirth = new DateTime(1979, i, 15),
                    Gender = EGender.Female
                });

                sb.Append($"<p>{maleModel.MainEnergy.EnergyNumber} {maleModel.CharacterEnergy.EnergyNumber} {maleModel.SurfaceEnergy.EnergyNumber} -- ");
                sb.Append($"-- {femaleModel.MainEnergy.EnergyNumber} {maleModel.CharacterEnergy.EnergyNumber} {maleModel.SurfaceEnergy.EnergyNumber}</p>");
            }

            return new ContentResult { Content = sb.ToString() };
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}

