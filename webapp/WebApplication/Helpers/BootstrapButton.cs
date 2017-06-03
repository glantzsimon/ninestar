using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.Globalisation;
using K9.WebApplication.Constants;
using K9.WebApplication.Constants.Html;
using K9.WebApplication.Enums;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString BootstrapBackToListButton(this HtmlHelper html)
		{
			return html.ActionLink(Dictionary.BackToList, "Index", null, new { @class = Bootstrap.Classes.PrimaryButton });
		}

		public static MvcHtmlString BootstrapButton(this HtmlHelper html, string value, EButtonType buttonType = EButtonType.Submit)
		{
			var sb = new StringBuilder();
			if (buttonType == EButtonType.Submit || buttonType == EButtonType.Delete || buttonType == EButtonType.Edit)
			{
				var hr = new TagBuilder(Tags.Hr);
				sb.AppendLine(hr.ToString(TagRenderMode.SelfClosing));
			}

			var button = new TagBuilder(Tags.Button);
			button.MergeAttribute(Attributes.Type, buttonType.ToString());
			button.MergeAttribute(Attributes.Class, GetButtonClass(buttonType));
			button.MergeAttribute(Attributes.DataLoadingText, string.Format("<i class='fa fa-circle-o-notch fa-spin'></i> {0}", value));

			switch (buttonType)
			{
				case EButtonType.Delete:
					button.InnerHtml = string.Format("<i class='fa fa-trash'></i> {0}", value);
					break;

				case EButtonType.Edit:
					button.InnerHtml = string.Format("<i class='fa fa-pencil'></i> {0}", value);
					break;

				default:
					button.InnerHtml = value;
					break;
			}

			sb.AppendLine(button.ToString());

			return MvcHtmlString.Create(sb.ToString());
		}

		private static string GetButtonClass(EButtonType buttonType)
		{
			switch (buttonType)
			{
				case EButtonType.Delete:
					return Bootstrap.Classes.RedButton;

				default:
					return Bootstrap.Classes.PrimaryButton;

			}
		}

	}
}