using System;
using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Extensions;
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
		private readonly IFileSourceService _fileSourceService;

		public NewsItemsController(IRepository<NewsItem> repository, ILogger logger, IDataTableAjaxHelper<NewsItem> ajaxHelper, IDataSetsHelper dataSetsHelper, IRoles roles, IFileSourceService fileSourceService)
			: base(repository, logger, ajaxHelper, dataSetsHelper, roles)
		{
			_fileSourceService = fileSourceService;
			RecordBeforeCreate += NewsItemsController_RecordBeforeCreate;
			RecordBeforeCreated += NewsItemsController_RecordBeforeCreated;
			RecordBeforeUpdated += NewsItemsController_RecordBeforeUpdated;
		}

		void NewsItemsController_RecordBeforeUpdated(object sender, EventArgs.CrudEventArgs e)
		{
			var newsItem = e.Item as NewsItem;
			if (newsItem.ImageFileSource != null)
			{
				SaveImageFiles(newsItem);
			}
		}

		void NewsItemsController_RecordBeforeCreated(object sender, EventArgs.CrudEventArgs e)
		{
			var newsItem = e.Item as NewsItem;
			if (newsItem.ImageFileSource != null)
			{
				SaveImageFiles(newsItem);
			}
		}

		private void SaveImageFiles(NewsItem newsItem)
		{
			_fileSourceService.SaveFilesToDisk(newsItem.ImageFileSource);
		}

		void NewsItemsController_RecordBeforeCreate(object sender, EventArgs.CrudEventArgs e)
		{
			var newsItem = e.Item as NewsItem;
			newsItem.PublishedBy = WebSecurity.IsAuthenticated ? WebSecurity.CurrentUserName : string.Empty;
			newsItem.PublishedOn = DateTime.Now;
		}

		
	}
}
