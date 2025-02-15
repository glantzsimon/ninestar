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

namespace K9.WebApplication.Controllers
{
    public partial class NineStarKiController : BaseNineStarKiController
    {
        private readonly IAuthentication _authentication;
        private readonly INineStarKiService _nineStarKiService;
        private readonly IRepository<User> _usersRepository;
        private readonly IBiorhythmsService _biorhythmsService;
        private readonly ApiConfiguration _apiConfig;
        private const string authRequestHeader = "Authorization";

        public NineStarKiController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, INineStarKiService nineStarKiService, IMembershipService membershipService, IRepository<User> usersRepository, IBiorhythmsService biorhythmsService, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository, IOptions<ApiConfiguration> apiConfig)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService, rolesRepository, userRolesRepository)
        {
            _authentication = authentication;
            _nineStarKiService = nineStarKiService;
            _usersRepository = usersRepository;
            _biorhythmsService = biorhythmsService;
            _apiConfig = apiConfig.Value;
        }

        [Route("api/personal-chart/get")]
        public JsonResult GetPersonalChart(DateTime dateOfBirth, EGender gender)
        {
            if (!IsValidApiKey(Request.Headers[authRequestHeader]))
            {
                return InvalidApiKeyResult();
            }

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
        }

        private JsonResult InvalidApiKeyResult()
        {
            return Json(new {success = false, error = "Invalid ApiKey"}, JsonRequestBehavior.AllowGet);
        }

        private bool IsValidApiKey(string apiKey)
        {
            return apiKey != null && apiKey == _apiConfig.ApiKey;
        }
    }
}