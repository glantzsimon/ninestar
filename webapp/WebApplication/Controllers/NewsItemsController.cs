using System;
using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
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
		private readonly IFileSourceHelper _fileSourceHelper;

		public NewsItemsController(IRepository<NewsItem> repository, ILogger logger, IDataTableAjaxHelper<NewsItem> ajaxHelper, IDataSetsHelper dataSetsHelper, IRoles roles, IFileSourceHelper fileSourceHelper)
			: base(repository, logger, ajaxHelper, dataSetsHelper, roles)
		{
			_fileSourceHelper = fileSourceHelper;
			RecordBeforeCreate += NewsItemsController_RecordBeforeCreate;
			RecordBeforeCreated += NewsItemsController_RecordBeforeCreated;
			RecordBeforeUpdate += NewsItemsController_RecordBeforeUpdate;
			RecordBeforeUpdated += NewsItemsController_RecordBeforeUpdated;
		}

		void NewsItemsController_RecordBeforeUpdate(object sender, EventArgs.CrudEventArgs e)
		{
			var newsItem = e.Item as NewsItem;
			LoadUploadedFiels(newsItem);
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

		void NewsItemsController_RecordBeforeCreate(object sender, EventArgs.CrudEventArgs e)
		{
			var newsItem = e.Item as NewsItem;
			newsItem.PublishedBy = WebSecurity.IsAuthenticated ? WebSecurity.CurrentUserName : string.Empty;
			newsItem.PublishedOn = DateTime.Now;
			LoadUploadedFiels(newsItem);
		}

		private void SaveImageFiles(NewsItem newsItem)
		{
			_fileSourceHelper.SaveFilesToDisk(newsItem.ImageFileSource, true);
		}

		private void LoadUploadedFiels(NewsItem newsItem)
		{
			_fileSourceHelper.LoadFiles(newsItem.ImageFileSource, false);
		}

	}
}
