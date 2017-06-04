using System.Text;
using System.Web.Mvc;
using K9.SharedLibrary.Models;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString AuditFieldsForModel<T>(this HtmlHelper<T> html, T model) where T : IObjectBase
		{
			var sb = new StringBuilder();

			sb.AppendLine(html.BootstrapDisplayFor(m => model.CreatedOn).ToString());
			sb.AppendLine(html.BootstrapDisplayFor(m => model.CreatedBy).ToString());
			sb.AppendLine(html.BootstrapDisplayFor(m => model.LastUpdatedOn).ToString());
			sb.AppendLine(html.BootstrapDisplayFor(m => model.LastUpdatedBy).ToString());

			return MvcHtmlString.Create(sb.ToString());
		}

	}
}