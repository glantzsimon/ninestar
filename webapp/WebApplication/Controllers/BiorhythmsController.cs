﻿using K9.Base.DataAccessLayer.Models;
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
    public partial class BiorhythmsController : BaseNineStarKiController
    {
        private readonly IAuthentication _authentication;
        private readonly INineStarKiService _nineStarKiService;
        private readonly IRepository<User> _usersRepository;
        private readonly IBiorhythmsService _biorhythmsService;

        public BiorhythmsController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, INineStarKiService nineStarKiService, IMembershipService membershipService, IRepository<User> usersRepository, IBiorhythmsService biorhythmsService, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService, rolesRepository, userRolesRepository)
        {
            _authentication = authentication;
            _nineStarKiService = nineStarKiService;
            _usersRepository = usersRepository;
            _biorhythmsService = biorhythmsService;
        }

        [Route("biorhythms")]
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

        [Route("biorhythms")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(NineStarKiModel model)
        {
            if (model.PersonModel != null || model.SelectedDate != DateTime.Today)
            {
                var selectedDate = model.SelectedDate ?? DateTime.Today;
                var isScrollToCyclesOverview = model.IsScrollToCyclesOverview;
                var activeTabId = model.ActiveCycleTabId;

                model = _nineStarKiService.CalculateNineStarKiProfile(model.PersonModel, false, false, selectedDate);
                model.SelectedDate = selectedDate;
                model.IsScrollToCyclesOverview = isScrollToCyclesOverview;
                model.ActiveCycleTabId = activeTabId;
            }

            model.BiorhythmResultSet = _biorhythmsService.Calculate(model, model.SelectedDate ?? DateTime.Today);

            return View("Index", model);
        }

        [Route("biorhythms/retrieve-last")]
        [Authorize]
        public ActionResult RetrieveLastBiorhythms(bool todayOnly = false)
        {
            var lastBiorhythms = SessionHelper.GetLastBiorhythm(todayOnly).PersonModel;
            if (lastBiorhythms == null)
            {
                return RedirectToAction("Index");
            }
            var model = _nineStarKiService.CalculateNineStarKiProfile(lastBiorhythms.DateOfBirth, lastBiorhythms.Gender);
            return View("Index", model);
        }
        
        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}

