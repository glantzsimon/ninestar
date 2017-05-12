using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.WebApplication.Options;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString Fail(this HtmlHelper html, string message, MvcHtmlString otherMessage)
		{
			return Fail(html, message, otherMessage.ToString());
		}

		public static MvcHtmlString Fail(this HtmlHelper html, MvcHtmlString message, MvcHtmlString otherMessage)
		{
			return Fail(html, message.ToString(), otherMessage.ToString());
		}

		public static MvcHtmlString Fail(this HtmlHelper html, MvcHtmlString message)
		{
			return Fail(html, message.ToString(), string.Empty);
		}

		public static MvcHtmlString Fail(this HtmlHelper html, string message, string otherMessage = "")
		{
			return html.Partial("_Fail", new AlertOptions
			{
				Message = message,
				OtherMessage = otherMessage
			});
		}

	}
}