using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using WebMatrix.WebData;

namespace K9.WebApplication.Filters
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method )]
	[DefaultProperty("Permission")]
	public class RequirePermissionsAttribute : ActionFilterAttribute
	{

		public string Permission { get; set; }
		public string Role { get; set; }

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var controller = filterContext.Controller as IBaseController;
			var roles = controller.Roles;
			var unauthorized = "Unauthorized";

			// Check controller level roles first
			var controllerPermissionAttribute =
				controller.GetType().GetCustomAttributes(typeof(RequirePermissionsAttribute), true).FirstOrDefault() as RequirePermissionsAttribute;

			if (controllerPermissionAttribute != null)
			{
				if (!string.IsNullOrEmpty(controllerPermissionAttribute.Role))
				{
					if (!CheckRole(roles, controllerPermissionAttribute.Role))
					{
						filterContext.Result = new ViewResult
						{
							ViewName = unauthorized
						};
						return;
					}
				}
			}

			if (!string.IsNullOrEmpty(Permission))
			{
				var fullyQualifiedPermissionName = string.Format("{0}{1}", Permission, controller.GetObjectName());
				if (!CheckPermission(roles, fullyQualifiedPermissionName))
				{
					filterContext.Result = new ViewResult
					{
						ViewName = unauthorized
					};
					return;
				}
			}

			if (!string.IsNullOrEmpty(Role))
			{
				if (!CheckRole(roles, Role))
				{
					filterContext.Result = new ViewResult
					{
						ViewName = unauthorized
					};
				}
			}
		}

		private bool CheckPermission(IRoles roles, string permissionName)
		{
			var permissions = roles.GetPermissionsForCurrentUser().Select(r => r.Name).ToList();
			if (!WebSecurity.IsAuthenticated || !permissions.Contains(permissionName) && !roles.CurrentUserIsInRole(RoleNames.Administrators))
			{
				return false;
			}
			return true;
		}

		private bool CheckRole(IRoles roles, string roleName)
		{
			var userRoles = roles.GetRolesForCurrentUser().Select(r => r.Name).ToList();
			if (!WebSecurity.IsAuthenticated || !userRoles.Contains(roleName) && !roles.CurrentUserIsInRole(RoleNames.Administrators))
			{
				return false;
			}

			return true;
		}


	}
}