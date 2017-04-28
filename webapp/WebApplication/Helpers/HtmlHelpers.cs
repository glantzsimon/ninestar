using System;
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

	}
}