using System.Linq;
using System.Threading;
using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.SharedLibrary.Models;
using NLog;

namespace K9.WebApplication.Controllers
{
	public class NewsController : BaseController
	{
		private readonly IRepository<NewsItem> _newsRepository;

		public NewsController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IRepository<NewsItem> newsRepository)
			: base(logger, dataSetsHelper, roles)
		{
			_newsRepository = newsRepository;
		}

		public ActionResult Index()
		{
			return View(_newsRepository.List().Where(n => n.LanguageCode == Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName).ToList());
		}

		public override string GetObjectName()
		{
			return typeof(NewsItem).Name;
		}
	}
}
