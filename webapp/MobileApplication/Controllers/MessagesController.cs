using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.EventArgs;
using K9.Base.WebApplication.UnitsOfWork;
using K9.SharedLibrary.Attributes;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{
    [Authorize]
	[LimitByUserId]
	public class MessagesController : BaseController<Message>
	{
		
		public MessagesController(IControllerPackage<Message> controllerPackage)
			: base(controllerPackage)
		{
			RecordBeforeCreate += MessagesController_RecordBeforeCreate;
		}

		void MessagesController_RecordBeforeCreate(object sender, CrudEventArgs e)
		{
			var message = e.Item as Message;
			message.SentByUserId = WebSecurity.IsAuthenticated ? WebSecurity.CurrentUserId : 0;
		}

	}
}
