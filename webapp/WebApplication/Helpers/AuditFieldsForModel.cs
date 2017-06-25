using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using K9.Globalisation;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using WebMatrix.WebData;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString AuditFieldsForModel<T>(this HtmlHelper<T> html, T model) where T : IObjectBase
		{
			if (Roles.IsUserInRole(WebSecurity.CurrentUserName, RoleNames.Administrators))
			{
				var sb = new StringBuilder();

				sb.AppendLine(html.SubTitle(Dictionary.AuditInformation).ToString());

				sb.AppendLine(html.BootstrapDisplayFor(m => model.CreatedOn).ToString());
				sb.AppendLine(html.BootstrapDisplayFor(m => model.CreatedBy).ToString());
				sb.AppendLine(html.BootstrapDisplayFor(m => model.LastUpdatedOn).ToString());
				sb.AppendLine(html.BootstrapDisplayFor(m => model.LastUpdatedBy).ToString());

				return MvcHtmlString.Create(sb.ToString());
			}

			return MvcHtmlString.Empty;
		}

	}
}