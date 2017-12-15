using K9.Base.WebApplication.Controllers;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Services;
using NLog;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class ShopController : BaseController
    {
        private readonly IShopService _shopService;

        public ShopController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IShopService shopService)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
        {
            _shopService = shopService;
        }

        public ActionResult Index()
        {
            ViewData[Constants.ViewDataConstants.Locale] = _shopService.GetLocale();
            ViewData[Constants.ViewDataConstants.ShopPrefix] = _shopService.GetShopPrefix();

            return View();
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}
