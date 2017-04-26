using System.Web.Mvc;
using K9.WebApplication.Constants;
using K9.WebApplication.Enums;

namespace K9.WebApplication.Extensions
{
	public static partial class HtmlExtensions
	{

		public static MvcHtmlString BootstrapButton(this HtmlHelper html, string value, EButtonType buttonType = EButtonType.Submit )
		{
			var button = new TagBuilder(Html.Tags.Button);
			button.MergeAttribute(Html.Attributes.Type, buttonType.ToString());
			button.MergeAttribute(Html.Attributes.Class, Bootstrap.Classes.DefaultButton);
			button.InnerHtml = value;

			return MvcHtmlString.Create(button.ToString());
		}

	}
}