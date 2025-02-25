using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using System;
using System.Web.Mvc;
using System.Web.UI;

namespace K9.WebApplication.Controllers
{
    public partial class PredictionsController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;

        public PredictionsController(INineStarKiPackage nineStarKiPackage, INineStarKiService nineStarKiService)
            : base(nineStarKiPackage)
        {
            _nineStarKiService = nineStarKiService;
        }

        [Route("predictions")]
        [OutputCache(Duration = 2592000, VaryByParam = "none", VaryByCustom = "User", Location = OutputCacheLocation.ServerAndClient)]
        public ActionResult Index()
        {
            var dateOfBirth = new DateTime(DateTime.Now.Year - (27), DateTime.Now.Month, DateTime.Now.Day);
            var personModel = new PersonModel
            {
                DateOfBirth = dateOfBirth,
                Gender = Methods.GetRandomGender()
            };
            return View(
                new PredictionsViewModel(new NineStarKiModel(personModel), _nineStarKiService.GetNineStarKiSummaryViewModel()));
        }

        [Route("predictions")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(NineStarKiModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.PersonModel != null || model.SelectedDate != DateTime.Today)
                {
                    var selectedDate = model.SelectedDate ?? DateTime.Today;

                    model = _nineStarKiService.CalculateNineStarKiProfile(model.PersonModel, false, false,
                        selectedDate);
                    model.SelectedDate = selectedDate;

                    if (Current.UserId > 0)
                    {
                        var user = My.UserService.Find(Current.UserId);
                        model.IsMyProfile = user.BirthDate == model.PersonModel.DateOfBirth;
                    }
                }
                return View(new PredictionsViewModel(model, _nineStarKiService.GetNineStarKiSummaryViewModel()));
            }

            return View(model);
        }

        [Authorize]
        [Route("predictions/my-predictions")]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult MyCycles()
        {
            var myAccount = My.UsersRepository.Find(Current.UserId);
            return View(_nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                Name = myAccount.FullName,
                DateOfBirth = myAccount.BirthDate,
                Gender = myAccount.Gender
            }, false, true));
        }

        [Route("predictions/retrieve-last")]
        [Authorize]
        [OutputCache(Duration = 0, NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult RetrieveLastPredictions(bool todayOnly = false)
        {
            var lastPredictions = SessionHelper.GetLastCompatibility(todayOnly).PersonModel;
            if (lastPredictions == null)
            {
                return RedirectToAction("Index");
            }
            var model = _nineStarKiService.CalculateNineStarKiProfile(lastPredictions.DateOfBirth, lastPredictions.Gender);
            return View("Index", model);
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}

