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

        public NineStarKiController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, INineStarKiService nineStarKiService, IMembershipService membershipService, IRepository<User> usersRepository)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService)
        {
            _authentication = authentication;
            _nineStarKiService = nineStarKiService;
            _usersRepository = usersRepository;
        }

        [Route("calculate")]
        public ActionResult Index()
        {
            var rand = new Random();
            var dateOfBirth = new DateTime(DateTime.Now.Year - (27), DateTime.Now.Month, DateTime.Now.Day);
            var personModel = new PersonModel
            {
                DateOfBirth = dateOfBirth,
                Gender = EGender.Other + rand.Next(1, 2)
            };
            return View(new NineStarKiModel(personModel));
        }

        [Route("calculate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CalculateNineStarKi(NineStarKiModel model)
        {
            if (model.PersonModel != null)
            {
                model = _nineStarKiService.CalculateNineStarKiProfile(model.PersonModel);
            }
            return View("Index", model);
        }

        [Authorize]
        [Route("my-profile")]
        public ActionResult MyProfile()
        {
            var myAccount = _usersRepository.Find(_authentication.CurrentUserId);
            return View(_nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
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
            var model = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = lastProfile.DateOfBirth,
                Gender = lastProfile.Gender
            });
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
            var model = _nineStarKiService.CalculateCompatibility(lastCompatibility.NineStarKiModel1.PersonModel, lastCompatibility.NineStarKiModel2.PersonModel);
            return View("Compatibility", model);
        }

        [Route("relationships/compatibility")]
        public ActionResult Compatibility()
        {
            var dateOfBirth = new DateTime(DateTime.Now.Year - (24), DateTime.Now.Month, DateTime.Now.Day);
            var personModel = new PersonModel
            {
                DateOfBirth = dateOfBirth,
                Gender = EGender.Female
            };
            return View("Compatibility", new CompatibilityModel(new NineStarKiModel(personModel), new NineStarKiModel(personModel)));
        }

        [Route("relationships/compatibility")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Compatibility(CompatibilityModel model)
        {
            if (model.NineStarKiModel1?.PersonModel != null && model.NineStarKiModel2?.PersonModel != null)
            {
                model = _nineStarKiService.CalculateCompatibility(model.NineStarKiModel1.PersonModel, model.NineStarKiModel2.PersonModel);
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

