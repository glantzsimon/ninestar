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
    public class NineStarKiController : BaseController
    {
        private readonly IRepository<EnergyInfo> _energyRepository;

        public NineStarKiController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IRepository<EnergyInfo> energyRepository)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
        {
            _energyRepository = energyRepository;
            SetBetaWarningSessionVariable();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Relationships()
        {
            return View();
        }

        public ActionResult Predictions()
        {
            return View();
        }

        public ActionResult Information()
        {
            return View();
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
