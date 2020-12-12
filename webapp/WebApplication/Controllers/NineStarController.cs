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
    public class NineStarController : BaseController
    {
        private readonly IRepository<EnergyInfo> _energyRepository;

        public NineStarController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IRepository<EnergyInfo> energyRepository)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
        {
            _energyRepository = energyRepository;
            SetBetaWarningSessionVariable();
        }

        [Route("calculate")]
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
                model.MainEnergyInfo = _energyRepository.Find(e => e.EnergyType == EEnergyType.MainEnergy && e.Energy == ninestar.MainEnergy.Energy).FirstOrDefault() ?? new EnergyInfo
                {
                    EnergyType = EEnergyType.MainEnergy
                };
                model.EmotionalEnergyInfo = _energyRepository.Find(e => e.EnergyType == EEnergyType.EmotionalEnergy && e.Energy == ninestar.EmotionalEnergy.Energy).FirstOrDefault() ?? new EnergyInfo
                {
                    EnergyType = EEnergyType.EmotionalEnergy
                };
                model.SurfaceEnergyInfo = _energyRepository.Find(e => e.EnergyType == EEnergyType.SurfaceEnergy && e.Energy == ninestar.SurfaceEnergy.Energy).FirstOrDefault() ?? new EnergyInfo
                {
                    EnergyType = EEnergyType.SurfaceEnergy
                };
            }
            return View("Index", model);
        }

        [Route("relationships")]
        public ActionResult Relationships()
        {
            return View();
        }

        [Route("predications")]
        public ActionResult Predictions()
        {
            return View();
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
            if (numberOfDisplays < 1)
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
