using K9.Base.DataAccessLayer.Enums;
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
        private readonly INineStarKiService _nineStarKiService;

        public NineStarKiController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, INineStarKiService nineStarKiService, IMembershipService membershipService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService)
        {
            _nineStarKiService = nineStarKiService;
        }

        [Route("calculate")]
        public ActionResult Index()
        {
            var dateOfBirth = new DateTime(DateTime.Now.Year - (24), DateTime.Now.Month, DateTime.Now.Day);
            var personModel = new PersonModel
            {
                DateOfBirth = dateOfBirth,
                Gender = EGender.Female
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
                SessionHelper.SetLastProfile(model.PersonModel);
            }
            return View("Index", model);
        }

        [Route("calculate/profile")]
        [Authorize]
        public ActionResult RetrieveProfile()
        {
            var lastProfile = SessionHelper.GetLastProfile();
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

        [Route("relationships/compatibility")]
        public ActionResult Compatibility()
        {
            var dateOfBirth = new DateTime(DateTime.Now.Year - (24), DateTime.Now.Month, DateTime.Now.Day);
            var personModel = new PersonModel
            {
                DateOfBirth = dateOfBirth
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
