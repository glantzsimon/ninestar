using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Enums;

namespace K9.WebApplication.Extensions
{
	public static partial class HtmlExtensions
	{

		public static MvcHtmlString EditorFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, EInputSize size = EInputSize.Default, object additionalViewData = null)
		{
			var viewDataDictionary = new ViewDataDictionary(new { }).Extend(additionalViewData);
			viewDataDictionary.MergeAttribute("class", size.ToCssClass());
			return html.EditorFor(expression, viewDataDictionary);
		}

	}
}