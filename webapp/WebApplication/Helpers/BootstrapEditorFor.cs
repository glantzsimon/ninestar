using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

		public static MvcHtmlString BootstrapEditorFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, EditorOptions options = null)
		{
			var modelType = ModelMetadata.FromLambdaExpression(expression, html.ViewData).ModelType;

			// Get additional view data for the control
			var viewDataDictionary = new ViewDataDictionary();
			options = options ?? new EditorOptions();
			viewDataDictionary.MergeAttribute(Attributes.Class, options.InputSize.ToCssClass());
			viewDataDictionary.MergeAttribute(Attributes.Class, options.InputWidth.ToCssClass());

			if (modelType != typeof(bool))
			{
				viewDataDictionary.MergeAttribute(Attributes.Class, Bootstrap.Classes.FormControl);
			}

			var additionalViewData = new
			{
				@class = viewDataDictionary[Attributes.Class],
				placeholder = options.PlaceHolder
			};

			// Get container div
			var div = new TagBuilder(Tags.Div);
			var attributes = new Dictionary<string, object>();
			attributes.MergeAttribute(Attributes.Class, modelType == typeof(bool) ? Bootstrap.Classes.Checkbox : Bootstrap.Classes.FormGroup);
			if (html.GetModelErrorsFor(expression).Any())
			{
				attributes.MergeAttribute(Attributes.Class, Bootstrap.Classes.HasError);
			}

			div.MergeAttributes(attributes);
			html.ViewContext.Writer.WriteLine(div.ToString(TagRenderMode.StartTag));

			// Show label for all types but boolean
			if (modelType != typeof(bool))
			{
				html.ViewContext.Writer.Write(html.LabelFor(expression));
			}

			html.ViewContext.Writer.Write(html.EditorFor(expression, additionalViewData));
			html.ViewContext.Writer.Write(html.ValidationMessageFor(expression));
			html.ViewContext.Writer.WriteLine(div.ToString(TagRenderMode.EndTag));

			return MvcHtmlString.Empty;
		}

	}
}