using System;
using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.Services;
using NLog;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{
	[Authorize]
	[LimitByUserId]
	public class NewsItemsController : BaseController<NewsItem>
	{
		private readonly INewsItemsService _newsItemsService;

		public NewsItemsController(IRepository<NewsItem> repository, ILogger logger, IDataTableAjaxHelper<NewsItem> ajaxHelper, IDataSetsHelper dataSetsHelper, IRoles roles, INewsItemsService NewsItemsService)
			: base(repository, logger, ajaxHelper, dataSetsHelper, roles)
		{
			_newsItemsService = NewsItemsService;
			RecordBeforeCreate += NewsItemsController_RecordBeforeCreate;
			RecordBeforeCreated += NewsItemsController_RecordBeforeCreated;
		}

		void NewsItemsController_RecordBeforeCreated(object sender, EventArgs.CrudEventArgs e)
		{
			var newsItem = e.Item as NewsItem;
			if (newsItem.ImageFile != null)
			{
				var imageUrl = _newsItemsService.SaveNewsItemImageToDisk(newsItem.ImageFile);
				newsItem.ImageUrl = imageUrl;
			}
		}

		void NewsItemsController_RecordBeforeCreate(object sender, EventArgs.CrudEventArgs e)
		{
			var newsItem = e.Item as NewsItem;
			newsItem.PublishedBy = WebSecurity.IsAuthenticated ? WebSecurity.CurrentUserName : string.Empty;
			newsItem.PublishedOn = DateTime.Now;
		}


	}
}
