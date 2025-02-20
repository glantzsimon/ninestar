using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.EventArgs;
using K9.Base.WebApplication.UnitsOfWork;
using K9.SharedLibrary.Attributes;
using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{
    [Authorize]
	[LimitByUserId]
	public class MessagesController : BaseNineStarKiController<Message>
	{
		
		public MessagesController(IControllerPackage<Message> controllerPackage, INineStarKiPackage nineStarKiPackage)
			: base(controllerPackage, nineStarKiPackage)
		{
			RecordBeforeCreate += MessagesController_RecordBeforeCreate;
		}

		void MessagesController_RecordBeforeCreate(object sender, CrudEventArgs e)
		{
			var message = e.Item as Message;
			message.SentByUserId = WebSecurity.IsAuthenticated ? Current.UserId : 0;
		}

	}
}
