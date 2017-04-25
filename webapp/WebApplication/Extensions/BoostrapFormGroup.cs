using System;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.WebApplication.Constants;

namespace K9.WebApplication.Extensions
{
	public static partial class HtmlExtensions
	{

		public static MvcHtmlString BootstrapFormGroupFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
		{
			var sb = new StringBuilder();
			
			var div = new TagBuilder(Html.Tags.Div);
			div.MergeAttribute(Html.Attributes.Class, Bootstrap.Classes.FormGroup);
			sb.AppendLine(div.ToString(TagRenderMode.StartTag));

			sb.AppendLine(html.Label(html.GetLabelText(expression)).ToString());
			sb.AppendLine(html.EditorFor(expression).ToString());

			sb.AppendLine(div.ToString(TagRenderMode.EndTag));

			return MvcHtmlString.Create(sb.ToString());
		}

	}
}