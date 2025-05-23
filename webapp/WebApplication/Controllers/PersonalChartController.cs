﻿using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Enums;
using K9.WebApplication.Enums;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("personal-chart")]
    public partial class PersonalChartController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;

        public PersonalChartController(INineStarKiPackage nineStarKiPackage, INineStarKiService nineStarKiService)
            : base(nineStarKiPackage)
        {
            _nineStarKiService = nineStarKiService;
        }

        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", VaryByHeader="User-Agent", Location = OutputCacheLocation.ServerAndClient)]
        [Route("free-calculator")]
        public ActionResult Index()
        {
            return View(new NineStarKiModel());
        }

        [Route("free-calculator/result")]
        [HttpPost]
        public ActionResult IndexPost(NineStarKiModel model)
        {
            if (ModelState.IsValid)
            {
                // Set user calculation method preference cookie
                UpdateUserPreferenceInt(Constants.SessionConstants.UserCalculationMethod,
                    (int)model.CalculationMethod);

                if (model.PersonModel != null)
                {
                    model.SelectedDate = model.SelectedDate ?? model.GetLocalNow();
                    var isScrollToCyclesOverview = model.IsScrollToCyclesOverview;
                    var activeTabId = model.ActiveCycleTabId;
                    var invertYinEnergies = model.CalculationMethod == ECalculationMethod.Chinese;

                    // Add time of birth
                    model.PersonModel.DateOfBirth = model.PersonModel.DateOfBirth.Add(model.PersonModel.TimeOfBirth);

                    model = _nineStarKiService.CalculateNineStarKiProfile(model.PersonModel, false, false,
                        model.SelectedDate, model.CalculationMethod, false, false, model.PersonModel.BirthTimeZoneId, EHousesDisplay.SolarHouse, model.InvertDailyAndHourlyKiForSouthernHemisphere);

                    model.IsScrollToCyclesOverview = isScrollToCyclesOverview;
                    model.ActiveCycleTabId = activeTabId;

                    if (Current.UserId > 0)
                    {
                        var user = My.UserService.Find(Current.UserId);
                        model.IsMyProfile = user.BirthDate == model.PersonModel.DateOfBirth && model.PersonModel.TimeOfBirth == user.BirthDate.TimeOfDay && user.Gender == model.PersonModel.Gender;

                        if (!model.IsMyProfile)
                        {
                            var myAccount = My.AccountService.GetAccount(Current.UserId);
                            if (myAccount.Membership.MembershipOption.IsFree)
                            {
                                if (myAccount.Membership.ComplementaryPersonalChartReadingCount > 0)
                                {
                                    My.MembershipService.UseComplementaryPersonalChartReading(Current.UserId);
                                    model.IsComplementary = true;
                                }
                            }
                        }
                    }
                }
            }

            return View("Index", model);
        }

        [Route("free-calculator/alchemy")]
        public async Task<JsonResult> GetAlchemy(DateTime dateOfBirth, string birthTimeZoneId, TimeSpan timeOfBirth, EGender gender, ECalculationMethod calculationMethod, EHousesDisplay housesDisplay, bool invertDailyAndHourlyKiForSouthernHemisphere)
        {
            if (!My.SystemSettings.IsEnabledAlchemy)
            {
                return Json(new
                {
                    AlchemisedSummary = "",
                    AlchemisedDescription = ""
                }, JsonRequestBehavior.AllowGet);
            }

            var userId = Current.GetUserId(System.Web.HttpContext.Current);

            if (ModelState.IsValid)
            {
                var personModel = new PersonModel
                {
                    DateOfBirth = dateOfBirth,
                    BirthTimeZoneId = birthTimeZoneId,
                    TimeOfBirth = timeOfBirth,
                    Gender = gender
                };

                // Add time of birth
                personModel.DateOfBirth = personModel.DateOfBirth.Add(personModel.TimeOfBirth);

                var invertYinEnergies = calculationMethod == ECalculationMethod.Chinese;

                var model = _nineStarKiService.CalculateNineStarKiProfile(personModel, false, false,
                    null, calculationMethod, false, false, personModel.BirthTimeZoneId, housesDisplay, invertDailyAndHourlyKiForSouthernHemisphere);

                var alchemy = await _nineStarKiService.GetNineStarKiPersonalChartAlchemy(model);
                return Json(new
                {
                    alchemy.AlchemisedSummary,
                    alchemy.AlchemisedDescription
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false });
        }

        [Authorize]
        [Route("my-free-chart")]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult MyProfile()
        {
            var myAccount = My.AccountService.GetAccount(Current.UserId);
            var personModel = new PersonModel
            {
                Name = myAccount.User.FullName,
                DateOfBirth = myAccount.User.BirthDate,
                TimeOfBirth = myAccount.UserInfo.TimeOfBirth,
                BirthTimeZoneId = myAccount.UserInfo.BirthTimeZoneId,
                Gender = myAccount.User.Gender
            };
            personModel.DateOfBirth = personModel.DateOfBirth.Add(personModel.TimeOfBirth);
            var nineStarKiProfile = _nineStarKiService.CalculateNineStarKiProfile(personModel, false, true);

            return View("Index", nineStarKiProfile);
        }

        [Route("retrieve-last", Name = "RetrieveLastPersonalChart")]
        [Authorize]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
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
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult RetrieveLastProfile(bool todayOnly = false)
        {
            var lastProfile = SessionHelper.GetLastProfile(todayOnly)?.PersonModel;
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

        [OutputCache(Duration = 2592000, VaryByParam = "none", Location = OutputCacheLocation.ServerAndClient)]
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

