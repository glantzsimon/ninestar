using System.Text;
using System.Web.Mvc;
using K9.WebApplication.Constants.Html;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString SubTitle(this HtmlHelper html, string subtitle, string titleTag = Tags.H3)
		{
			var sb = new StringBuilder();

			var h = new TagBuilder(titleTag);
			h.SetInnerText(subtitle);
			h.MergeAttribute(Attributes.Class, "subtitle");
			sb.AppendLine(h.ToString());

			var hr = new TagBuilder(Tags.Hr);
			sb.AppendLine(hr.ToString(TagRenderMode.SelfClosing));

			return MvcHtmlString.Create(sb.ToString());
		}

	}
}