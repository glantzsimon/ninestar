using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.WebApplication.Constants;

namespace K9.WebApplication.Extensions
{
	public static partial class HtmlExtensions
	{

		public static MvcHtmlString BootstrapButton(this HtmlHelper html, string value, string type = "submit" )
		{
			var button = new TagBuilder(Html.Tags.Button);
			button.MergeAttribute(Html.Attributes.Type, type);
			button.MergeAttribute(Html.Attributes.Class, Bootstrap.Classes.DefaultButton);
			button.InnerHtml = value;

			return MvcHtmlString.Create(button.ToString());
		}

	}
}