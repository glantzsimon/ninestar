using K9.Base.DataAccessLayer.Enums;
using K9.Base.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using NLog;
using System;
using System.Web.Mvc;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api")]
    public partial class ApiController : BaseNineStarKiController
    {
        private readonly IAuthentication _authentication;
        private readonly INineStarKiService _nineStarKiService;
        private readonly IRepository<User> _usersRepository;
        private readonly IBiorhythmsService _biorhythmsService;
        private readonly IMembershipService _membershipService;
        private readonly ApiConfiguration _apiConfig;
        private const string authRequestHeader = "Authorization";

        public ApiController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, INineStarKiService nineStarKiService, IMembershipService membershipService, IRepository<User> usersRepository, IBiorhythmsService biorhythmsService, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository, IOptions<ApiConfiguration> apiConfig)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService, rolesRepository, userRolesRepository)
        {
            _authentication = authentication;
            _nineStarKiService = nineStarKiService;
            _usersRepository = usersRepository;
            _biorhythmsService = biorhythmsService;
            _membershipService = membershipService;
            _apiConfig = apiConfig.Value;
        }

        [Route("personal-chart/get/{accountNumber}/" +
               "{dateOfBirth}/{gender}")]
        public JsonResult GetPersonalChart(string accountNumber, DateTime dateOfBirth, EGender gender)
        {
            return Validate(accountNumber, () =>
            {
                var model = new NineStarKiModel(new PersonModel
                {
                    DateOfBirth = dateOfBirth,
                    Gender = gender
                })
                {
                    SelectedDate = DateTime.Today
                };

                var selectedDate = model.SelectedDate;
                model = _nineStarKiService.CalculateNineStarKiProfile(model.PersonModel, false, false, selectedDate);
                model.SelectedDate = selectedDate;

                return Json(new { success = true, data = model }, JsonRequestBehavior.AllowGet);
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
        
        private JsonResult Validate(string accountNumber, Func<JsonResult> method)
        {
            if (!IsValidApiKey(Request.Headers[authRequestHeader]))
            {
                return InvalidApiKeyResult();
            }

            var membership = GetMembership(accountNumber);
            if (membership == null)
            {
                return InvalidAccountNumberResult();
            }

            if (!IsValidMembership(membership))
            {
                return MembershipRequiresUpgradeResult();
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
            return apiKey != null && apiKey == _apiConfig.ApiKey;
        }

        private bool IsValidMembership(UserMembership membership)
        {
            return (membership.IsActive && membership.MembershipOption != null && membership.MembershipOption.IsUnlimited) ||
                   Roles.UserIsInRoles(membership.User?.Username, RoleNames.Administrators);
        }

        private UserMembership GetMembership(string accountNumber)
        {
            return _membershipService.GetActiveUserMembership(accountNumber);
        }
    }
}