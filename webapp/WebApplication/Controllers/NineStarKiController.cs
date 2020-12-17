using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using NLog;
using System;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class NineStarKiController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;

        public NineStarKiController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, INineStarKiService nineStarKiService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
        {
            _nineStarKiService = nineStarKiService;
        }

        [Route("calculate")]
        public ActionResult Index()
        {
            var dateOfBirth = new DateTime(DateTime.Now.Year - (24), DateTime.Now.Month, DateTime.Now.Day);
            var personModel = new PersonModel
            {
                DateOfBirth = dateOfBirth
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
                model = _nineStarKiService.CalculateNineStarKi(model.PersonModel);
            }
            return View("Index", model);
        }
      
        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}
