using System.Collections.Generic;
using System.Web.Mvc;

namespace K9.WebApplication.Attributes
{
	public class RequirePermissionsAttribute : ActionFilterAttribute
	{
		/// <summary>
		/// Any matching roles will result in the request being authorized
		/// </summary>
		public List<string> Roles { get; set; }

		/// <summary>
		/// Any matching permissions will result in the request being authorized
		/// </summary>
		public List<string> Permissions { get; set; }

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			
			base.OnActionExecuting(filterContext);
		}
	}
}