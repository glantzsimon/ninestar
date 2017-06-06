using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Constants;
using K9.WebApplication.Constants.Html;
using K9.WebApplication.Enums;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString BootstrapBackToListButton(this HtmlHelper html)
		{
			return html.ActionLink(string.Format("< {0}", Dictionary.BackToList), "Index", html.GetStatelessFilter().GetFilterRouteValues(), new { @class = Bootstrap.Classes.InfoButton });
		}

		public static MvcHtmlString BootstrapLinkToDeleteButton(this HtmlHelper html, int id)
		{
			return MvcHtmlString.Create(string.Format("<a class=\"btn btn-danger\" href=\"{0}\"><i class='fa fa-trash'></i> {1}</a>", html.GeturlHeler().Action("Delete", GetFilterRouteValueDictionaryWithId(html, id)), Dictionary.Delete));
		}

		public static MvcHtmlString BootstrapLinkToEditButton(this HtmlHelper html, int id)
		{
			return MvcHtmlString.Create(string.Format("<a class=\"btn btn-primary\" href=\"{0}\"><i class='fa fa-pencil'></i> {1}</a>", html.GeturlHeler().Action("Edit", GetFilterRouteValueDictionaryWithId(html, id)), Dictionary.Edit));
		}

		public static MvcHtmlString BootstrapLinkButton(this HtmlHelper html, string linkText, string actionName, string controllerName, int id)
		{
			return MvcHtmlString.Create(string.Format("<a class=\"btn btn-info\" href=\"{0}\"><i class='fa fa-link'></i> {1}</a>", html.GeturlHeler().Action(actionName, controllerName, GetFilterRouteValueDictionaryWithId(html, id)), linkText));
		}

		public static MvcHtmlString BootstrapCreateNewButton(this HtmlHelper html, RouteValueDictionary routeValues)
		{
			return html.ActionLink(string.Format("{0}", Dictionary.CreateNew, routeValues), "Create", null, new { @class = Bootstrap.Classes.PrimaryButton });
		}

		public static MvcHtmlString BootstrapButton(this HtmlHelper html, string value, EButtonType buttonType = EButtonType.Submit)
		{
			var button = new TagBuilder(Tags.Button);
			button.MergeAttribute(Attributes.Type, buttonType.ToString());
			button.MergeAttribute(Attributes.Class, GetButtonClass(buttonType));
			button.MergeAttribute(Attributes.DataLoadingText, string.Format("<i class='fa fa-circle-o-notch fa-spin'></i> {0}", value));

			switch (buttonType)
			{
				case EButtonType.Delete:
					button.InnerHtml = string.Format("<i class='fa fa-trash'></i> {0}", value);
					break;

				case EButtonType.Edit:
					button.InnerHtml = string.Format("<i class='fa fa-pencil'></i> {0}", value);
					break;

				default:
					button.InnerHtml = value;
					break;
			}

			return MvcHtmlString.Create(button.ToString());
		}

		/// <summary>
		/// Gets a RouteValueDictionary containing the ForeignKey and Id of any filters applied to the list view (stateless filtering)
		/// </summary>
		/// <param name="html"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		private static RouteValueDictionary GetFilterRouteValueDictionaryWithId(HtmlHelper html, int id)
		{
			var routeValues = html.GetStatelessFilter().GetFilterRouteValues();
			routeValues.Add("id", id);
			return routeValues;
		}

		private static string GetButtonClass(EButtonType buttonType)
		{
			switch (buttonType)
			{
				case EButtonType.Delete:
					return Bootstrap.Classes.RedButton;

				default:
					return Bootstrap.Classes.PrimaryButton;

			}
		}

	}
}