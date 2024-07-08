using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.Filters;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Services;
using NLog;
using System;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RequirePermissions(Role = RoleNames.Administrators)]
    public class LogController : BaseController
    {
        private readonly ILogService _logService;

        public LogController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, ILogService logService) : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
        {
            _logService = logService;
        }

        [Route("log")]
        public ActionResult Index()
        {
            return View(_logService.GetLogItems());
        }

        public override string GetObjectName()
        {
            throw new NotImplementedException();
        }
    }
}