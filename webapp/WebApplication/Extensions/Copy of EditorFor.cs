using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.WebApplication.Constants;
using K9.WebApplication.Enums;

namespace K9.WebApplication.Extensions
{
	public static partial class HtmlExtensions
	{

		public static IDisposable BootstrapEditorFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, EInputSize size = EInputSize.Default)
		{
			var div = new TagBuilder(Html.Tags.Div);
			div.MergeAttribute(Html.Attributes.Class, Bootstrap.Classes.FormGroup);
			html.ViewContext.Writer.WriteLine(div.ToString(TagRenderMode.StartTag));

			html.ViewContext.Writer.WriteLine(html.LabelFor(expression));
			html.ViewContext.Writer.WriteLine(html.EditorFor(expression));

			return new TagCloser(html, Html.Tags.Div);
		}

	}
}