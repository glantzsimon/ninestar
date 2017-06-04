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
				var li = new TagBuilder(Tags.Li) {InnerHtml = html.ActionLink(Dictionary.LogOut, "LogOff", "Account").ToString()};
				sb.Append(li);
			}
			else
			{
				var li = new TagBuilder(Tags.Li) {InnerHtml = html.ActionLink(Dictionary.LogIn, "Login", "Account").ToString()};
				sb.Append(li);

				li = new TagBuilder(Tags.Li) {InnerHtml = html.ActionLink(Dictionary.Register, "Register", "Account").ToString()};
				sb.Append(li);
			}

			return MvcHtmlString.Create(sb.ToString());
		}

	}
}