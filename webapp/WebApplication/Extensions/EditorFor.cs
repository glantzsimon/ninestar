using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Constants;
using K9.WebApplication.Enums;
using K9.WebApplication.Options;

namespace K9.WebApplication.Extensions
{
	public static partial class HtmlExtensions
	{

		public static MvcHtmlString EditorFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, EditorOptions options = null)
		{
			var viewDataDictionary = new ViewDataDictionary();
			options = options ?? new EditorOptions();

			viewDataDictionary.MergeAttribute(Html.Attributes.Class, options.InputSize.ToCssClass());
			viewDataDictionary.MergeAttribute(Html.Attributes.Class, options.InputWidth.ToCssClass());

			var additionalViewData = new
			{
				@class = viewDataDictionary[Html.Attributes.Class],
				placeholder = options.PlaceHolder
			};

			return html.EditorFor(expression, additionalViewData);
		}

	}
}