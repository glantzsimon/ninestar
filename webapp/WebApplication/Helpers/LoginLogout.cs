using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.Globalisation;
using K9.WebApplication.Constants.Html;
using WebMatrix.WebData;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString LoginLogout(this HtmlHelper html)
		{
			var sb = new StringBuilder();

			if (WebSecurity.IsAuthenticated)
			{
				sb.Append(html.ActionLink(Dictionary.LogOut, "LogOff", "Account"));
			}
			else
			{
				sb.Append(html.ActionLink(Dictionary.LogIn, "Login", "Account"));
			}

			return MvcHtmlString.Create(sb.ToString());
		}

	}
}