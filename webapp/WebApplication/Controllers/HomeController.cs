using K9.Base.WebApplication.Constants;
using K9.Base.WebApplication.Controllers;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Models;
using NLog;
using System;
using System.Web.Mvc;
using K9.WebApplication.ViewModels;

namespace K9.WebApplication.Controllers
{
    public class HomeController : BaseController
    {

        public HomeController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
        {
        }

        public ActionResult Index()
        {
            var personModel = new PersonModel
            {
                DateOfBirth = new DateTime(1980, 1, 1)
            };
            return View(new NineStarKiViewModel
            {
                PersonModel = personModel,
                NineStarKiModel = new NineStarKiModel(personModel)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CalculateNineStarKi(NineStarKiViewModel model)
        {
            return View("Index");
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
    }
}
