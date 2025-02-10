using K9.Base.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using NLog;
using System;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public partial class CompatibilityController : BaseNineStarKiController
    {
        private readonly IAuthentication _authentication;
        private readonly INineStarKiService _nineStarKiService;
        private readonly IRepository<User> _usersRepository;
        private readonly IBiorhythmsService _biorhythmsService;

        public CompatibilityController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, INineStarKiService nineStarKiService, IMembershipService membershipService, IRepository<User> usersRepository, IBiorhythmsService biorhythmsService, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService, rolesRepository, userRolesRepository)
        {
            _authentication = authentication;
            _nineStarKiService = nineStarKiService;
            _usersRepository = usersRepository;
            _biorhythmsService = biorhythmsService;
        }

        [Route("compatibility")]
        public ActionResult Index()
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
            return View(new CompatibilityModel(new NineStarKiModel(personModel1), new NineStarKiModel(personModel2)));
        }

        [Route("compatibility")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CompatibilityModel model)
        {
            if (model.NineStarKiModel1?.PersonModel != null && model.NineStarKiModel2?.PersonModel != null)
            {
                model = _nineStarKiService.CalculateCompatibility(model.NineStarKiModel1.PersonModel, model.NineStarKiModel2.PersonModel, model.IsHideSexualChemistry);
            }
            return View(model);
        }

        [Route("compatibility/retrieve-last")]
        [Authorize]
        public ActionResult RetrieveLastCompatibility(bool todayOnly = false)
        {
            var lastCompatibility = SessionHelper.GetLastCompatibility(todayOnly).CompatibilityModel;
            if (lastCompatibility == null)
            {
                return RedirectToAction("Index");
            }
            var model = _nineStarKiService.CalculateCompatibility(lastCompatibility.NineStarKiModel1.PersonModel, lastCompatibility.NineStarKiModel2.PersonModel, lastCompatibility.IsHideSexualChemistry);
            return View("Index", model);
        }

        [Authorize]
        [Route("compatibility/view")]
        public ActionResult ViewCompatibility(int id)
        {
            return View("Index", _nineStarKiService.RetrieveCompatibility(id));
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}

