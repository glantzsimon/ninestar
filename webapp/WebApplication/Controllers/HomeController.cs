using K9.Base.WebApplication.Constants;
using K9.Base.WebApplication.Controllers;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using NLog;
using System;
using System.Linq;
using System.Web.Mvc;
using K9.Base.WebApplication.Helpers;
using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;

namespace K9.WebApplication.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IRepository<EnergyInfo> _energyRepository;

        public HomeController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IRepository<EnergyInfo> energyRepository)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
        {
            _energyRepository = energyRepository;
            SetBetaWarningSessionVariable();
        }

        public ActionResult Index()
        {
            var dateOfBirth = new DateTime(DateTime.Now.Year - (24), DateTime.Now.Month, DateTime.Now.Day);
            var personModel = new PersonModel
            {
                DateOfBirth = dateOfBirth
            };
            return View(new NineStarKiViewModel
            {
                PersonModel = personModel
            });
        }

        [Route("calculate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CalculateNineStarKi(NineStarKiViewModel model)
        {
            if (model.PersonModel != null)
            {
                var ninestar = new NineStarKiModel(model.PersonModel);
                model.NineStarKiModel = ninestar;
                model.MainEnergyInfo = _energyRepository.Find(e => e.EnergyType == EEnergyType.MainEnergy && e.Energy == ninestar.MainEnergy.Energy).FirstOrDefault() ?? new EnergyInfo();
                model.CharacterEnergyInfo = _energyRepository.Find(e => e.EnergyType == EEnergyType.CharacterEnergy && e.Energy == ninestar.CharacterEnergy.Energy).FirstOrDefault() ?? new EnergyInfo();
                model.RisingEnergyInfo = _energyRepository.Find(e => e.EnergyType == EEnergyType.RisingEnergy && e.Energy == ninestar.RisingEnergy.Energy).FirstOrDefault() ?? new EnergyInfo();
            }
            return View("Index", model);
        }

        public ActionResult SetLanguage(string languageCode, string cultureCode)
        {
            Session[SessionConstants.LanguageCode] = languageCode;
            Session[SessionConstants.CultureCode] = cultureCode;
            return Redirect(Request.UrlReferrer?.ToString());
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }

        private static void SetBetaWarningSessionVariable()
        {
            var numberOfDisplays = Helpers.SessionHelper.GetIntValue(Constants.SessionConstants.BetaWarningDisplay);
            if (numberOfDisplays < 3)
            {
                numberOfDisplays++;
                SessionHelper.SetValue(Constants.SessionConstants.BetaWarningDisplay, numberOfDisplays);
            }
            else
            {
                SessionHelper.SetValue(Constants.SessionConstants.BetaWarningHide, true);
            }
        }
    }
}
