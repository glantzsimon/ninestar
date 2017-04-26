using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.WebApplication.Constants;

namespace K9.WebApplication.Extensions
{
	public static partial class HtmlExtensions
	{

		public static IDisposable BeginBootstrapForm(this HtmlHelper html, string title = "")
		{
			html.ViewContext.Writer.WriteLine(html.AntiForgeryToken());
			html.ViewContext.Writer.WriteLine(html.ValidationSummary(true));

			var div = new TagBuilder(Html.Tags.Div);
			div.MergeAttribute(Html.Attributes.Class, Bootstrap.Classes.Well);
			html.ViewContext.Writer.WriteLine(div.ToString(TagRenderMode.StartTag));

			if (!string.IsNullOrEmpty(title))
			{
				var h2 = new TagBuilder(Html.Tags.H2);
				h2.SetInnerText(title);
				html.ViewContext.Writer.WriteLine(h2.ToString());
			}

			return new TagCloser(html, Html.Tags.Div);
		}

	}
}