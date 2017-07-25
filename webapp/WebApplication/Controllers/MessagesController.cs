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
	public class MessagesController : BaseController<Message>
	{
		public MessagesController(IRepository<Message> repository, ILogger logger, IDataTableAjaxHelper<Message> ajaxHelper, IDataSetsHelper dataSetsHelper, IRoles roles)
			: base(repository, logger, ajaxHelper, dataSetsHelper, roles)
		{
			RecordBeforeCreate += MessagesController_RecordBeforeCreate;
		}

		void MessagesController_RecordBeforeCreate(object sender, EventArgs.CrudEventArgs e)
		{
			var message = e.Item as Message;
			message.SentByUserId = WebSecurity.IsAuthenticated ? WebSecurity.CurrentUserId : 0;
		}
	}
}
