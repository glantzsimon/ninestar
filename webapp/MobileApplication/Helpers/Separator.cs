using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace K9.WebApplication.Helpers
{
    public static partial class HtmlHelpers
	{

		public static MvcHtmlString Separator(this HtmlHelper html)
		{
			return html.Partial("_Separator");
		}

	}
}