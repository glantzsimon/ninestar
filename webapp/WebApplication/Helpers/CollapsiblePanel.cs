using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.WebApplication.Options;

namespace K9.WebApplication.Helpers
{
    public static partial class HtmlHelpers
	{

		public static MvcHtmlString CollapsiblePanel(this HtmlHelper html, CollapsiblePanelOptions options)
		{
			return html.Partial("_CollapsiblePanel", options);
		}

	    public static MvcHtmlString CollapsiblePanel(this HtmlHelper html, string title, string body, bool expanded = false, string footer = "")
	    {
	        return html.Partial("_CollapsiblePanel", new CollapsiblePanelOptions
	        {
	            Title = title,
                Body = body,
                Expaded = expanded,
                Footer = footer
	        });
	    }
    }
}