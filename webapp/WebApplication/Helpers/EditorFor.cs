using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Constants;
using K9.WebApplication.Constants.Html;
using K9.WebApplication.Enums;
using K9.WebApplication.Options;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString EditorFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, EditorOptions options = null)
		{
			var viewDataDictionary = new ViewDataDictionary();
			options = options ?? new EditorOptions();

			viewDataDictionary.MergeAttribute(Attributes.Class, options.InputSize.ToCssClass());
			viewDataDictionary.MergeAttribute(Attributes.Class, options.InputWidth.ToCssClass());

			var additionalViewData = new
			{
				@class = viewDataDictionary[Attributes.Class],
				placeholder = options.PlaceHolder
			};

			var div = new TagBuilder(Tags.Div);
			div.MergeAttribute(Attributes.Class, Bootstrap.Classes.FormGroup);
			if (html.GetModelErrorsFor(expression).Any())
			{
				div.MergeAttribute(Attributes.Class, Bootstrap.Classes.Error);
			}
			html.ViewContext.Writer.WriteLine(div.ToString(TagRenderMode.EndTag));

			return html.EditorFor(expression, additionalViewData);
		}

	}
}