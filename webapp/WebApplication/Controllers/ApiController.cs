using K9.Base.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Enums;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api")]
    public partial class ApiController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;
        private readonly IIChingService _iChingService;
        private const string authRequestHeader = "Authorization";

        public ApiController(INineStarKiPackage nineStarKiPackage, INineStarKiService nineStarKiService, IIChingService iChingService)
            : base(nineStarKiPackage)
        {
            _nineStarKiService = nineStarKiService;
            _iChingService = iChingService;
        }

        [Route("personal-chart/get/{accountNumber}/" +
               "{dateOfBirth}/{timeOfBirth}/{birthLocation}/{gender}")]
        public JsonResult GetPersonalChart(string accountNumber, DateTime dateOfBirth, string timeOfBirth, string birthLocation, EGender gender)
        {
            return Validate(accountNumber, () =>
            {
                var model = new NineStarKiModel(new PersonModel
                {
                    DateOfBirth = dateOfBirth.Add(DateTimeHelper.ParseTime(timeOfBirth)),
                    Gender = gender,
                    BirthTimeZoneId = DateTimeHelper.ResolveTimeZone(birthLocation)
                })
                {
                    SelectedDate = DateTime.Today
                };

                var selectedDate = model.SelectedDate;
                model = _nineStarKiService.CalculateNineStarKiProfile(model.PersonModel, false, false, selectedDate,
                    ECalculationMethod.Chinese);
                model.SelectedDate = selectedDate;

                return Json(new { success = true, data = new
                {
                    model.PersonModel,
                    GenerationEnergy = model.PersonalChartEnergies.Generation,
                    model.MainEnergy,
                    model.CharacterEnergy,
                    model.SurfaceEnergy,
                    DayStarEnergy = model.PersonalChartEnergies.Day,
                    
                    model.Summary,
                    model.Overview,
                    IntellectualQualities = model.MainEnergy.IntellectualQualitiesSummary,
                    InterpersonalQualities = model.MainEnergy.InterpersonalQualitiesSummary,
                    EmotionalLandscape = model.MainEnergy.EmotionalLandscapeSummary,
                    Spirituality = model.MainEnergy.SpiritualitySummary,
                    Health = model.MainEnergy.HealthSummary,
                    model.MainEnergy.Illnesses,
                    Career = model.MainEnergy.CareerSummary,
                    Finances = model.MainEnergy.FinancesSummary,
                    model.MainEnergy.Occupations,
                    model.MainEnergyRelationshipsSummary,
                    model.StressResponseDetails,
                    model.StressResponseFromNatalHouseDetails,
                    model.AdultChildRelationsihpDescription,

                } }, JsonRequestBehavior.AllowGet);
            });
        }

        [Route("compatibility/get/{accountNumber}/" +
               "{firstPersonName}/{firstPersonDateOfBirth}/{firstPersonGender}/" +
               "{secondPersonName}/{secondPersonDateOfBirth}/{secondPersonGender}/" +
               "{displaySexualChemistry}")]
        public JsonResult GetCompatibility(string accountNumber,
            string firstPersonName, DateTime firstPersonDateOfBirth, EGender firstPersonGender,
            string secondPersonName, DateTime secondPersonDateOfBirth, EGender secondPersonGender,
            bool displaySexualChemistry = false)
        {
            return Validate(accountNumber, () =>
            {
                var personModel1 = new PersonModel
                {
                    Name = firstPersonName,
                    DateOfBirth = firstPersonDateOfBirth,
                    Gender = firstPersonGender
                };
                var personModel2 = new PersonModel
                {
                    Name = secondPersonName,
                    DateOfBirth = secondPersonDateOfBirth,
                    Gender = secondPersonGender
                };

                var model = _nineStarKiService.CalculateCompatibility(personModel1, personModel2, false);

                foreach (var propertyInfo in model.GetProperties())
                {
                    if (propertyInfo.PropertyType == typeof(string) && propertyInfo.CanWrite)
                    {
                        try
                        {
                            model.SetProperty(propertyInfo, string.Empty);
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }

                return Json(new { success = true, data = model }, JsonRequestBehavior.AllowGet);
            });
        }

        [Route("predictions/get/{accountNumber}/" +
               "{dateOfBirth}/{gender}/{selectedDate}")]
        public JsonResult GetPredictions(string accountNumber, DateTime dateOfBirth, EGender gender, DateTime selectedDate)
        {
            return Validate(accountNumber, () =>
            {
                var model = new NineStarKiModel(new PersonModel
                {
                    DateOfBirth = dateOfBirth,
                    Gender = gender
                })
                {
                    SelectedDate = selectedDate
                };

                model = _nineStarKiService.CalculateNineStarKiProfile(model.PersonModel, false, false, selectedDate);
                model.SelectedDate = selectedDate;

                var predictionsViewModel =
                    new PredictionsViewModel(model, _nineStarKiService.GetNineStarKiSummaryViewModel());

                return Json(new { success = true, data = predictionsViewModel }, JsonRequestBehavior.AllowGet);
            });
        }

        [Route("knowledgebase/get/{accountNumber}")]
        [OutputCache(Duration = 2592000, VaryByParam = "accountNumber", Location = OutputCacheLocation.ServerAndClient)]
        public JsonResult GetKnowledgeBase(string accountNumber)
        {
            Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(1));
            Response.Cache.SetValidUntilExpires(true);

            return Validate(accountNumber, () =>
            {
                var model = new NineStarKiSummaryKbViewModel(_nineStarKiService.GetNineStarKiSummaryViewModel());
                return Json(new { success = true, data = model }, JsonRequestBehavior.AllowGet);
            });
        }

        [Route("iching/get")]
        public JsonResult GetIChing()
        {
            return Validate(null, () =>
            {
                var model = new IChingViewModel(_iChingService.GenerateHexagram());
                return Json(new { success = true, data = model }, JsonRequestBehavior.AllowGet);
            });
        }

        public JsonResult GetCompatibilityTest()
        {
            var personModel1 = new PersonModel
            {
                Name = "Simon Baby Kotik",
                DateOfBirth = new DateTime(1979, 06, 16),
                Gender = EGender.Male
            };
            var personModel2 = new PersonModel
            {
                Name = "Andrei Kotik",
                DateOfBirth = new DateTime(1984, 09, 07),
                Gender = EGender.Male
            };

            var model = _nineStarKiService.CalculateCompatibility(personModel1, personModel2, false);

            foreach (var propertyInfo in model.GetProperties())
            {
                if (propertyInfo.PropertyType == typeof(string) && propertyInfo.CanWrite)
                {
                    try
                    {
                        model.SetProperty(propertyInfo, string.Empty);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            return Json(new { success = true, data = model }, JsonRequestBehavior.AllowGet);
        }

        private JsonResult Validate(string accountNumber, Func<JsonResult> method)
        {
            if (!IsValidApiKey(Request.Headers[authRequestHeader]))
            {
                return InvalidApiKeyResult();
            }

            if (accountNumber != null)
            {
                var membership = GetMembership(accountNumber);
                if (membership == null)
                {
                    return InvalidAccountNumberResult();
                }

                if (!IsValidMembership(membership))
                {
                    return MembershipRequiresUpgradeResult();
                }
            }

            return method.Invoke();
        }

        private JsonResult InvalidApiKeyResult()
        {
            return Json(new
            {
                success = false,
                error = "Invalid ApiKey",
                statusCode = 401
            }, JsonRequestBehavior.AllowGet);
        }

        private JsonResult InvalidAccountNumberResult()
        {
            return Json(new
            {
                success = false,
                error = "Invalid Account Number",
                statusCode = 404
            }, JsonRequestBehavior.AllowGet);
        }

        private JsonResult MembershipRequiresUpgradeResult()
        {
            return Json(new
            {
                success = false,
                error = "Membership Has Insufficient Permissions. Upgrade Required.",
                statusCode = 422
            }, JsonRequestBehavior.AllowGet);
        }

        private bool IsValidApiKey(string authHeader)
        {
            string apiKey = null;
            if (!string.IsNullOrEmpty(authHeader))
            {
                apiKey = authHeader.Substring("ApiKey".Length).Trim();
            }
            return apiKey != null && apiKey == My.ApiConfiguration.ApiKey;
        }

        private bool IsValidMembership(UserMembership membership)
        {
            return (membership.IsActive && membership.MembershipOption != null && membership.MembershipOption.IsUnlimited) ||
                   Roles.UserIsInRoles(membership.User?.Username, RoleNames.Administrators);
        }

        private UserMembership GetMembership(string accountNumber)
        {
            return My.MembershipService.GetActiveUserMembership(accountNumber);
        }
    }
}