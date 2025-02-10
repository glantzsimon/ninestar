using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Helpers;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Constants;
using K9.WebApplication.Services;
using NLog;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class NewsController : BaseNineStarKiController
	{
		private readonly IRepository<NewsItem> _newsRepository;

		public NewsController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IRepository<NewsItem> newsRepository, IAuthentication authentication, IFileSourceHelper fileSourceHelper, IMembershipService membershipService, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository)
			: base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService, rolesRepository, userRolesRepository)
		{
			_newsRepository = newsRepository;
		}

		public ActionResult Index(int id = 0)
		{
		    ViewData[ViewDataConstants.SelectedId] = id;
			return View(_newsRepository.List().Where(n => n.LanguageCode == Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName).ToList());
		}

		public ActionResult NewsSummary()
		{
			return PartialView("_NewsSummary", _newsRepository.GetQuery("SELECT TOP 10 * FROM [NewsItem] ORDER BY [PublishedOn]")
				.Where(n => n.LanguageCode == SessionHelper.GetStringValue(Base.WebApplication.Constants.SessionConstants.LanguageCode)).ToList());
		}

		public override string GetObjectName()
		{
			return typeof(NewsItem).Name;
		}
	}
}
