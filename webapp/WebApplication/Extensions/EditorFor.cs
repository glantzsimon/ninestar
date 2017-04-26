using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.WebApplication.Enums;

namespace K9.WebApplication.Extensions
{
	public static partial class HtmlExtensions
	{

		public static MvcHtmlString EditorFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, EInputSize size = EInputSize.Default)
		{
			return html.EditorFor(expression, new {@class = size.ToCssClass()});
		}

	}
}