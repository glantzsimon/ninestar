using System.Text;
using System.Web.Mvc;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString CrudButtonsForModel<T>(this HtmlHelper<T> html, T model) where T : IObjectBase
		{
			var sb = new StringBuilder();
			var roles = html.GetRoles();

			if ((roles.CurrentUserIsInRole(RoleNames.Administrators) || roles.CurrentUserHasPermission<T>(Permissions.Edit)) && !model.IsSystemStandard)
			{
				sb.AppendLine(html.BootstrapLinkToEditButton(model.Id).ToString());
			}
			if ((roles.CurrentUserIsInRole(RoleNames.Administrators) || roles.CurrentUserHasPermission<T>(Permissions.Delete)) && !model.IsSystemStandard)
			{
				sb.AppendLine(html.BootstrapLinkToDeleteButton(model.Id).ToString());
			}
			return MvcHtmlString.Create(sb.ToString());
		}

	}
}