using K9.Base.DataAccessLayer.Enums;
using K9.Base.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using NLog;
using System;
using System.Text;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public partial class PersonalChartController : BaseNineStarKiController
    {
        private readonly IAuthentication _authentication;
        private readonly INineStarKiService _nineStarKiService;
        private readonly IRepository<User> _usersRepository;
        private readonly IBiorhythmsService _biorhythmsService;

        public PersonalChartController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, INineStarKiService nineStarKiService, IMembershipService membershipService, IRepository<User> usersRepository, IBiorhythmsService biorhythmsService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService)
        {
            _authentication = authentication;
            _nineStarKiService = nineStarKiService;
            _usersRepository = usersRepository;
            _biorhythmsService = biorhythmsService;
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
        public ActionResult CalculateNineStarKi(NineStarKiModel model)
        {
            if (model.PersonModel != null || model.SelectedDate != DateTime.Today)
            {
                var selectedDate = model.SelectedDate ?? DateTime.Today;
                var isScrollToCyclesOverview = model.IsScrollToCyclesOverview;
                var activeTabId = model.ActiveCycleTabId;

                model = _nineStarKiService.CalculateNineStarKiProfile(model.PersonModel, false, false, selectedDate);
                model.SelectedDate = selectedDate;
                model.IsScrollToCyclesOverview = isScrollToCyclesOverview;
                model.ActiveCycleTabId = activeTabId;
            }

            model.BiorhythmResultSet = _biorhythmsService.Calculate(model, model.SelectedDate ?? DateTime.Today);

            return View("Index", model);
        }

        [Authorize]
        [Route("personalchart/view")]
        public ActionResult ViewProfile(int id)
        {
            try
            {
                var profile = _nineStarKiService.RetrieveNineStarKiProfile(id);
                return View("Index", profile);
            }
            catch (UnauthorizedAccessException e)
            {
                return new HttpUnauthorizedResult();
            }
        }

        [Authorize]
        [Route("personalchart/my-chart")]
        public ActionResult MyProfile()
        {
            var myAccount = _usersRepository.Find(_authentication.CurrentUserId);
            var personModel = new PersonModel
            {
                Name = myAccount.FullName,
                DateOfBirth = myAccount.BirthDate,
                Gender = myAccount.Gender
            };
            var nineStarKiProfile = _nineStarKiService.CalculateNineStarKiProfile(personModel, false, true);

            nineStarKiProfile.BiorhythmResultSet = _biorhythmsService.Calculate(nineStarKiProfile, DateTime.Today);

            return View(nineStarKiProfile);
        }

        [Route("personalchart/retrieve")]
        [Authorize]
        public ActionResult RetrieveLast()
        {
            var retrieveLast = TempData["RetrieveLast"].ToString();
            switch (retrieveLast)
            {
                case "p":
                    return RedirectToAction("RetrieveLastProfile");

                case "c":
                    return RedirectToAction("RetrieveLastCompatibility");

                default:
                    return RedirectToAction("Index");
            }
        }

        [Route("personalchart/retrievelast")]
        [Authorize]
        public ActionResult RetrieveLastProfile(bool todayOnly = false)
        {
            var lastProfile = SessionHelper.GetLastProfile(todayOnly);
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
            model.BiorhythmResultSet = _biorhythmsService.Calculate(model, DateTime.Today);
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

