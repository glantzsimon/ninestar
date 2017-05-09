using System.Text;
using System.Web.Mvc;
using K9.WebApplication.Constants;
using K9.WebApplication.Constants.Html;
using K9.WebApplication.Enums;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString BootstrapButton(this HtmlHelper html, string value, EButtonType buttonType = EButtonType.Submit )
		{
			var sb = new StringBuilder();
			if (buttonType == EButtonType.Submit)
			{
				var hr = new TagBuilder(Tags.Hr);
				sb.AppendLine(hr.ToString(TagRenderMode.SelfClosing));
			}

			var button = new TagBuilder(Tags.Button);
			button.MergeAttribute(Attributes.Type, buttonType.ToString());
			button.MergeAttribute(Attributes.Class, Bootstrap.Classes.PrimaryButton);
			button.MergeAttribute(Attributes.DataLoadingText, string.Format("<i class='fa fa-circle-o-notch fa-spin'></i> {0}", value));
			button.InnerHtml = value;
			
			sb.AppendLine(button.ToString());

			return MvcHtmlString.Create(sb.ToString());
		}

	}
}