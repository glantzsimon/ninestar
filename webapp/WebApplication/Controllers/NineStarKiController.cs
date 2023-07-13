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
    public class NineStarKiController : BaseNineStarKiController
    {
        private readonly IAuthentication _authentication;
        private readonly INineStarKiService _nineStarKiService;
        private readonly IRepository<User> _usersRepository;
        private readonly IBiorhythmsService _biorhythmsService;

        public NineStarKiController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, INineStarKiService nineStarKiService, IMembershipService membershipService, IRepository<User> usersRepository, IBiorhythmsService biorhythmsService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService)
        {
            _authentication = authentication;
            _nineStarKiService = nineStarKiService;
            _usersRepository = usersRepository;
            _biorhythmsService = biorhythmsService;
        }

        [Route("calculate")]
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

        [Route("calculate")]
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

            model.Biorhythms = _biorhythmsService.Calculate(model, model.SelectedDate ?? DateTime.Today);

            return View("Index", model);
        }

        [Authorize]
        [Route("view-saved-chart")]
        public ActionResult ViewProfile(int id)
        {
            return View("Index", _nineStarKiService.RetrieveNineStarKiProfile(id));
        }

        [Authorize]
        [Route("my-chart")]
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

            nineStarKiProfile.Biorhythms = _biorhythmsService.Calculate(nineStarKiProfile, DateTime.Today);

            return View(nineStarKiProfile);
        }

        [Authorize]
        [Route("my-chart/cycles")]
        public ActionResult MyCycles()
        {
            var myAccount = _usersRepository.Find(_authentication.CurrentUserId);
            return View(_nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                Name = myAccount.FullName,
                DateOfBirth = myAccount.BirthDate,
                Gender = myAccount.Gender
            }, false, true));
        }

        [Route("retrieve-last")]
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

        [Route("last-profile")]
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
            model.Biorhythms = _biorhythmsService.Calculate(model, DateTime.Today);
            return View("Index", model);
        }

        [Route("last-compatibility")]
        [Authorize]
        public ActionResult RetrieveLastCompatibility(bool todayOnly = false)
        {
            var lastCompatibility = SessionHelper.GetLastCompatibility(todayOnly);
            if (lastCompatibility == null)
            {
                return RedirectToAction("Compatibility");
            }
            var model = _nineStarKiService.CalculateCompatibility(lastCompatibility.NineStarKiModel1.PersonModel, lastCompatibility.NineStarKiModel2.PersonModel, lastCompatibility.IsHideSexualChemistry);
            return View("Compatibility", model);
        }

        [Authorize]
        [Route("view-saved-compatibility")]
        public ActionResult ViewCompatibility(int id)
        {
            return View("Compatibility", _nineStarKiService.RetrieveCompatibility(id));
        }

        [Route("compatibility")]
        public ActionResult Compatibility()
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
            return View("Compatibility", new CompatibilityModel(new NineStarKiModel(personModel1), new NineStarKiModel(personModel2)));
        }

        [Route("compatibility")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Compatibility(CompatibilityModel model)
        {
            if (model.NineStarKiModel1?.PersonModel != null && model.NineStarKiModel2?.PersonModel != null)
            {
                model = _nineStarKiService.CalculateCompatibility(model.NineStarKiModel1.PersonModel, model.NineStarKiModel2.PersonModel, model.IsHideSexualChemistry);
            }
            return View("Compatibility", model);
        }

        [Route("all-enegies")]
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

