using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
			var sb = new StringBuilder();
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
				placeholder = options.PlaceHolder,
				title = ""
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
			sb.AppendLine(div.ToString(TagRenderMode.StartTag));

			// Show label for all types but boolean
			if (modelType != typeof(bool))
			{
				sb.AppendLine(html.LabelFor(expression).ToString());
			}

			sb.AppendLine(html.EditorFor(expression, additionalViewData).ToString());
			sb.AppendLine(html.ValidationMessageFor(expression).ToString());
			sb.AppendLine(div.ToString(TagRenderMode.EndTag));

			return MvcHtmlString.Create(sb.ToString());
		}

	}
}