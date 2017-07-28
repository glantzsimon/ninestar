using System;
using System.Collections.Generic;
using System.Linq;
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

		public static MvcHtmlString BootstrapActionLinkButton(this HtmlHelper html, string linkText, string actionName, string controllerName, object routeValues = null, string iconCssClass = "", params EButtonClass[] buttonClasses)
		{
			var buttonCssClass = GetButtonClass(EButtonType.Custom, buttonClasses);
			var isRightAligned = buttonClasses.Contains(EButtonClass.IconRight);
			var rightAlignedText = isRightAligned ? string.Format(" {0}", linkText) : string.Empty;
			var leftAlignedText = !isRightAligned ? string.Format("{0} ", linkText) : string.Empty;

			if (!string.IsNullOrEmpty(iconCssClass))
			{
				return MvcHtmlString.Create(string.Format("<a class=\"{0}\" href=\"{1}\">{2}<i class='fa {4}'></i>{3}</a>",
					buttonCssClass,
					html.GeturlHeler().Action(actionName, controllerName, routeValues),
					leftAlignedText,
					rightAlignedText,
					iconCssClass));
			}
			return MvcHtmlString.Create(string.Format("<a class=\"{2}\" href=\"{0}\">{1}</a>", html.GeturlHeler().Action(actionName, controllerName, routeValues), linkText, buttonCssClass));
		}

		public static MvcHtmlString BootstrapCreateNewButton(this HtmlHelper html)
		{
			return MvcHtmlString.Create(string.Format("<a class=\"btn btn-primary\" href=\"{0}\"><i class='fa fa-plus-circle'></i> {1}</a>", html.GeturlHeler().Action("Create", html.GetStatelessFilter().GetFilterRouteValues()), Dictionary.CreateNew));
		}

		public static MvcHtmlString BootstrapButton(this HtmlHelper html, string value, EButtonType buttonType = EButtonType.Submit, string iconCssClass = "", params EButtonClass[] buttonClasses)
		{
			var button = new TagBuilder(Tags.Button);
			var isRightAlignedCaret = buttonClasses.Contains(EButtonClass.IconRight);
			var leftAlignedText = isRightAlignedCaret ? string.Format("{0} ", value) : string.Empty;
			var rightAlignedText = !isRightAlignedCaret ? string.Format(" {0}", value) : string.Empty;

			button.MergeAttribute(Attributes.Type, buttonType.ToString());
			button.MergeAttribute(Attributes.Class, GetButtonClass(buttonType, buttonClasses));
			button.MergeAttribute(Attributes.DataLoadingText, string.Format("<i class='fa fa-circle-o-notch fa-spin'></i> {0}", value));

			switch (buttonType)
			{
				case EButtonType.Delete:
					button.InnerHtml = string.Format("{0}<i class='fa fa-trash'></i>{1}", leftAlignedText, rightAlignedText);
					break;

				case EButtonType.Edit:
					button.InnerHtml = string.Format("{0}<i class='fa fa-pencil'></i>{1}", leftAlignedText, rightAlignedText);
					break;

				case EButtonType.Custom:
					if (string.IsNullOrEmpty(iconCssClass))
					{
						button.InnerHtml = value;
					}
					else
					{
						button.InnerHtml = string.Format("{0}<i class='fa {1}'></i>{2}", leftAlignedText, iconCssClass, rightAlignedText);
					}
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

		private static string GetButtonClass(EButtonType buttonType, params EButtonClass[] buttonClasses)
		{
			switch (buttonType)
			{
				case EButtonType.Delete:
					return Bootstrap.Classes.ButtonDanger;

				case EButtonType.Custom:
					return !buttonClasses.Any()
						? "btn btn-primary"
						: GetButtonClasses(buttonClasses).Select(_ => _.ToCssClass()).Aggregate((a, b) => string.Format("{0} {1}", a, b)).Trim();


				default:
					return Bootstrap.Classes.ButtonPrimary;

			}
		}

		private static List<EButtonClass> GetButtonClasses(EButtonClass[] buttonClasses)
		{
			var list = buttonClasses.ToList();
			if (!new List<EButtonClass>
			{
				EButtonClass.Default,
				EButtonClass.Primary,
				EButtonClass.Info,
				EButtonClass.Warning,
				EButtonClass.Success,
				EButtonClass.Danger
			}.Any(x => list.Any(y => x == y)))
			{
				list.Add(EButtonClass.Primary);
			}
			return list;
		}


	}
}