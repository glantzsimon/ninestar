using System;
using System.Linq;
using System.Web.Mvc;
using K9.WebApplication.Constants;
using K9.WebApplication.Constants.Html;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static IDisposable BeginBootstrapForm(this HtmlHelper html, string title = "")
		{
			var div = new TagBuilder(Tags.Div);
			div.MergeAttribute(Attributes.Class, Bootstrap.Classes.Well);
			html.ViewContext.Writer.WriteLine(div.ToString(TagRenderMode.StartTag));

			html.ViewContext.Writer.WriteLine(html.AntiForgeryToken());
			html.ViewContext.Writer.WriteLine(html.BootstrapValidationSummary());

			if (!string.IsNullOrEmpty(title))
			{
				var h2 = new TagBuilder(Tags.H2);
				h2.SetInnerText(title);
				html.ViewContext.Writer.WriteLine(h2.ToString());
			}

			return new TagCloser(html, Tags.Div);
		}

	}
}