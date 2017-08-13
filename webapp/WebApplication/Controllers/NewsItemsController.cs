using System;
using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.SharedLibrary.Attributes;
using K9.WebApplication.UnitsOfWork;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{
	[Authorize]
	[LimitByUserId]
	public class NewsItemsController : BaseController<NewsItem>
	{

		public NewsItemsController(IControllerPackage<NewsItem> controllerPackage)
			: base(controllerPackage)
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
