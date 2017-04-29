using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static string GetDisplayNameFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
		{
			var metaData = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
			string displayName = metaData.DisplayName ?? (metaData.PropertyName ?? ExpressionHelper.GetExpressionText(expression));
			return displayName;
		}

		public static string GetPropertyNamesFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
		{
			var metaData = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
			return metaData.PropertyName ?? ExpressionHelper.GetExpressionText(expression);
		}

		public static List<ModelError> GetModelErrors(this HtmlHelper html, bool excludePropertyErrors = false)
		{
			return html.ViewData.ModelState.Where(x => !excludePropertyErrors || string.IsNullOrEmpty(x.Key)).SelectMany(x => x.Value.Errors).ToList();
		}

		public static List<ModelError> GetModelErrorsFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
		{
			return html.ViewData.ModelState.Where(x => x.Key == html.GetPropertyNamesFor(expression)).SelectMany(x => x.Value.Errors).ToList();
		}

	}
}