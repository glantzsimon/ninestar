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
    public partial class PredictionsController : BaseNineStarKiController
    {
        private readonly IAuthentication _authentication;
        private readonly INineStarKiService _nineStarKiService;
        private readonly IRepository<User> _usersRepository;
        private readonly IBiorhythmsService _biorhythmsService;

        public PredictionsController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, INineStarKiService nineStarKiService, IMembershipService membershipService, IRepository<User> usersRepository, IBiorhythmsService biorhythmsService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService)
        {
            _authentication = authentication;
            _nineStarKiService = nineStarKiService;
            _usersRepository = usersRepository;
            _biorhythmsService = biorhythmsService;
        }

        [Route("predictions")]
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

        [Route("predictions")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CalculateCycles(NineStarKiModel model)
        {
            if (model.PersonModel != null || model.SelectedDate != DateTime.Today)
            {
                var selectedDate = model.SelectedDate ?? DateTime.Today;
                
                model = _nineStarKiService.CalculateNineStarKiProfile(model.PersonModel, false, false, selectedDate);
                model.SelectedDate = selectedDate;

                return View(model);
            }

            return View(model);
        }

        [Authorize]
        [Route("predictions/my-predictions")]
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

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}

