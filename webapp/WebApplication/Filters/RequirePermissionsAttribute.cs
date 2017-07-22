using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using WebMatrix.WebData;

namespace K9.WebApplication.Filters
{
	[DefaultProperty("Permission")]
	public class RequirePermissionsAttribute : ActionFilterAttribute
	{

		public string Permission { get; set; }
		public string Role { get; set; }

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var controller = filterContext.Controller as IBaseController;
			var roles = controller.Roles;
			var permissions = roles.GetPermissionsForCurrentUser().Select(r => r.Name).ToList();
			var userRoles = roles.GetRolesForCurrentUser().Select(r => r.Name).ToList();
			var unauthorized = "Unauthorized";	

			if (!string.IsNullOrEmpty(Permission))
			{
				var fullyQualifiedPermissionName = string.Format("{0}{1}", Permission, controller.GetObjectName());
				if (!WebSecurity.IsAuthenticated || !permissions.Contains(fullyQualifiedPermissionName) && !roles.UserIsInRole(WebSecurity.CurrentUserName, RoleNames.Administrators))
				{
					filterContext.Result = new ViewResult
					{
						ViewName = unauthorized
					};
				}
			}

			if (!string.IsNullOrEmpty(Role))
			{
				if (!WebSecurity.IsAuthenticated || !userRoles.Contains(Role) && !roles.UserIsInRole(WebSecurity.CurrentUserName, RoleNames.Administrators))
				{
					filterContext.Result = new ViewResult
					{
						ViewName = unauthorized
					};
				}
			}
		}

	}
}