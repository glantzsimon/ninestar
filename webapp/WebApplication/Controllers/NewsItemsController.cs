using System;
using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using NLog;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{
	[Authorize]
	[LimitByUserId]
	public class NewsItemsController : BaseController<NewsItem>
	{
		public NewsItemsController(IRepository<NewsItem> repository, ILogger logger, IDataTableAjaxHelper<NewsItem> ajaxHelper, IDataSetsHelper dataSetsHelper, IRoles roles) : base(repository, logger, ajaxHelper, dataSetsHelper, roles)
		{
			RecordBeforeCreate += NewsItemsController_RecordBeforeCreate;
		}

		void NewsItemsController_RecordBeforeCreate(object sender, EventArgs.CrudEventArgs e)
		{
			var newsItem = e.Item as NewsItem;
			newsItem.PublishedBy = WebSecurity.IsAuthenticated ? WebSecurity.CurrentUserName : string.Empty;
			newsItem.PublishedOn = DateTime.Now;
		}
	}
}
