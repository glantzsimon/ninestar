using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.WebApplication.Options;

namespace K9.WebApplication.Helpers
{
    public static partial class HtmlHelpers
	{

		public static MvcHtmlString CollapsiblePanel(this HtmlHelper html, CollapsiblePanelOptions options)
		{
			return html.Partial("Controls/_CollapsiblePanel", options);
		}

	    public static MvcHtmlString CollapsiblePanel(this HtmlHelper html, string title, string body, bool expanded = false, string footer = "")
	    {
	        return html.Partial("Controls/_CollapsiblePanel", new CollapsiblePanelOptions
	        {
	            Title = title,
                Body = body,
                Expaded = expanded,
                Footer = footer
	        });
	    }

	    public static MvcHtmlString Panel(this HtmlHelper html, PanelOptions options)
	    {
	        return html.Partial("Controls/_Panel", options);
	    }

	    public static MvcHtmlString Panel(this HtmlHelper html, string title, string body, string imageSrc = "", EPanelImageSize imageSize = EPanelImageSize.Default, EPanelImageLayout imageLayout = EPanelImageLayout.Cover)
	    {
	        return html.Partial("Controls/_Panel", new PanelOptions
	        {
	            Title = title,
	            Body = body,
                ImageSrc = imageSrc,
                ImageSize = imageSize,
                ImageLayout = imageLayout
	        });
	    }

	    public static MvcHtmlString ImagePanel(this HtmlHelper html, PanelOptions options)
	    {
	        return html.Partial("Controls/_ImagePanel", options);
	    }

	    public static MvcHtmlString ImagePanel(this HtmlHelper html, string title, string imageSrc = "", EPanelImageSize imageSize = EPanelImageSize.Default, EPanelImageLayout imageLayout = EPanelImageLayout.Cover)
	    {
	        return html.Partial("Controls/_ImagePanel", new PanelOptions
	        {
	            Title = title,
	            ImageSrc = imageSrc,
                ImageSize = imageSize,
	            ImageLayout = imageLayout
	        });
	    }
    }
}