using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using NLog;

namespace K9.WebApplication.Controllers
{
	[Authorize]
	public class MessagesController : BaseController<Message>
	{
		public MessagesController(IRepository<Message> repository, ILogger logger, IDataTableAjaxHelper<Message> ajaxHelper, IDataSetsHelper dataSetsHelper, IRoles roles)
			: base(repository, logger, ajaxHelper, dataSetsHelper, roles)
		{
		}
	}
}
