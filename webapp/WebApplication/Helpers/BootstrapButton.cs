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
			return MvcHtmlString.Create(string.Format("<a class=\"btn btn-info\" href=\"{0}\"><i class='fa fa-angle-left'></i> {1}</a>", html.GeturlHeler().Action("Index", html.GetStatelessFilter().GetFilterRouteValues()), Dictionary.BackToList));
		}

		public static MvcHtmlString BootstrapLinkToDeleteButton(this HtmlHelper html, int id)
		{
			return MvcHtmlString.Create(string.Format("<a class=\"btn btn-danger\" href=\"{0}\"><i class='fa fa-trash'></i> {1}</a>", html.GeturlHeler().Action("Delete", GetFilterRouteValueDictionaryWithId(html, id)), Dictionary.Delete));
		}

		public static MvcHtmlString BootstrapLinkToEditButton(this HtmlHelper html, int id)
		{
			return MvcHtmlString.Create(string.Format("<a class=\"btn btn-primary\" href=\"{0}\"><i class='fa fa-pencil'></i> {1}</a>", html.GeturlHeler().Action("Edit", GetFilterRouteValueDictionaryWithId(html, id)), Dictionary.Edit));
		}

		/// <summary>
		/// Link to a ICollection property of the model
		/// </summary>
		/// <param name="html"></param>
		/// <param name="linkText"></param>
		/// <param name="actionName"></param>
		/// <param name="controllerName"></param>
		/// <param name="id">The Id of the model, which is used in the IStatelessFilter object to enable navigation back to filtered list</param>
		/// <returns></returns>
		public static MvcHtmlString BootstrapLinkToCollectionButton(this HtmlHelper html, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues)
		{
			return MvcHtmlString.Create(string.Format("<a class=\"btn btn-info\" href=\"{0}\"><i class='fa fa-link'></i> {1}</a>", html.GeturlHeler().Action(actionName, controllerName, routeValues), linkText));
		}

		public static MvcHtmlString BootstrapActionLinkButton(this HtmlHelper html, string linkText, string actionName, string controllerName, object routeValues = null, string iconCssClass = "")
		{
			if(!string.IsNullOrEmpty(iconCssClass))
			{
				return MvcHtmlString.Create(string.Format("<a class=\"btn btn-info\" href=\"{0}\"><i class='fa {2}'></i> {1}</a>", html.GeturlHeler().Action(actionName, controllerName, routeValues), linkText, iconCssClass));
			}
			return MvcHtmlString.Create(string.Format("<a class=\"btn btn-info\" href=\"{0}\">{1}</a>", html.GeturlHeler().Action(actionName, controllerName, null), linkText));
		}

		public static MvcHtmlString BootstrapCreateNewButton(this HtmlHelper html)
		{
			return MvcHtmlString.Create(string.Format("<a class=\"btn btn-primary\" href=\"{0}\"><i class='fa fa-plus-circle'></i> {1}</a>", html.GeturlHeler().Action("Create", html.GetStatelessFilter().GetFilterRouteValues()), Dictionary.CreateNew));
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
					return Bootstrap.Classes.ButtonDanger;

				default:
					return Bootstrap.Classes.ButtonPrimary;

			}
		}

	}
}