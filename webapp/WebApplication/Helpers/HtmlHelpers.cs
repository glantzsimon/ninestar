using K9.Base.WebApplication.Constants.Html;
using K9.Base.WebApplication.Enums;
using K9.Base.WebApplication.Helpers;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.WebApplication.Controllers;
using K9.WebApplication.Models;
using K9.WebApplication.Options;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.Base.DataAccessLayer.Attributes;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Enums;
using K9.WebApplication.ViewModels;
using WebMatrix.WebData;

namespace K9.WebApplication.Helpers
{
    public static partial class HtmlHelpers
    {
        public static string ActionWithBookmark(this UrlHelper helper, string actionName, string controllerName, string bookmark)
        {
            return helper.RequestContext.RouteData.Values["action"].ToString().ToLower() == actionName.ToLower() &&
                   helper.RequestContext.RouteData.Values["controller"].ToString().ToLower() == controllerName.ToLower()
                ? $"#{bookmark}"
                : $"{helper.Action(actionName, controllerName)}#{bookmark}";
        }

        public static MvcHtmlString CollapsiblePanel(this HtmlHelper html, CollapsiblePanelOptions options)
        {
            return html.Partial("Controls/_CollapsiblePanel", options);
        }

        public static MvcHtmlString CollapsiblePanel(this HtmlHelper html, string title, string body, bool expanded = false, string footer = "", string id = "", string imageSrc = "", EPanelImageSize imageSize = EPanelImageSize.Default, EPanelImageLayout imageLayout = EPanelImageLayout.Cover)
        {
            return html.Partial("Controls/_CollapsiblePanel", new CollapsiblePanelOptions
            {
                Id = id,
                Title = title,
                Body = body,
                Expaded = expanded,
                Footer = footer,
                ImageSrc = string.IsNullOrEmpty(imageSrc) ? string.Empty : new UrlHelper(html.ViewContext.RequestContext).Content(imageSrc),
                ImageLayout = imageLayout,
                ImageSize = imageSize
            });
        }

        public static MvcHtmlString Panel(this HtmlHelper html, PanelOptions options)
        {
            return html.Partial("Controls/_Panel", options);
        }

        public static MvcHtmlString Panel(this HtmlHelper html, string title, string body, string id = "", string imageSrc = "", EPanelImageSize imageSize = EPanelImageSize.Default, EPanelImageLayout imageLayout = EPanelImageLayout.Cover)
        {
            return html.Partial("Controls/_Panel", new PanelOptions
            {
                Id = id,
                Title = title,
                Body = body,
                ImageSrc = string.IsNullOrEmpty(imageSrc) ? string.Empty : new UrlHelper(html.ViewContext.RequestContext).Content(imageSrc),
                ImageSize = imageSize,
                ImageLayout = imageLayout
            });
        }

        public static MvcHtmlString ImagePanel(this HtmlHelper html, PanelOptions options)
        {
            return html.Partial("Controls/_ImagePanel", options);
        }

        public static MvcHtmlString ImagePanel(this HtmlHelper html, string title, string imageSrc = "", EPanelImageSize imageSize = EPanelImageSize.Default, EPanelImageLayout imageLayout = EPanelImageLayout.Cover)
        {
            return html.Partial("Controls/_ImagePanel", new PanelOptions
            {
                Title = title,
                ImageSrc = string.IsNullOrEmpty(imageSrc) ? string.Empty : new UrlHelper(html.ViewContext.RequestContext).Content(imageSrc),
                ImageSize = imageSize,
                ImageLayout = imageLayout
            });
        }

        public static string GetEnergySpecificDisplayNameFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string energyName)
        {
            return $"{energyName} {html.GetDisplayNameFor(expression)}";
        }

        public static IDisposable PayWall(this HtmlHelper html, ESection section, MembershipOption.ESubscriptionType subscriptionType = MembershipOption.ESubscriptionType.WeeklyPlatinum, bool silent = false, string displayHtml = "")
        {
            var baseController = html.ViewContext.Controller as BaseNineStarKiController;
            var activeUserMembership = baseController?.GetActiveUserMembership();
            return html.PayWall<NineStarKiModel>(section, null,
                () => activeUserMembership?.MembershipOption?.SubscriptionType >= subscriptionType || SessionHelper.CurrentUserIsAdmin(), silent, displayHtml);
        }

        public static IDisposable PayWall(this HtmlHelper html, ESection section, Func<bool?> condition, bool silent = false, string displayHtml = "")
        {
            return html.PayWall<NineStarKiModel>(section, null, condition, silent, displayHtml);
        }

        public static IDisposable PayWall<T>(this HtmlHelper html, ESection section, T model, bool silent = false,
            string displayHtml = "", bool hidePadlock = false)
        {
            var baseController = html.ViewContext.Controller as BaseNineStarKiController;
            var activeUserMembership = baseController?.GetActiveUserMembership();
            return html.PayWall<NineStarKiModel>(section, null, () => (activeUserMembership != null && activeUserMembership?.IsAuthorisedToViewPaidContent() == true) || SessionHelper.CurrentUserIsAdmin(), silent, displayHtml, hidePadlock);
        }

        public static IDisposable PayWall<T>(this HtmlHelper html, ESection section, T model, Func<bool?> condition, bool silent = false, string displayHtml = "", bool hidePadlock = false)
        {
            var baseController = html.ViewContext.Controller as BaseNineStarKiController;
            var activeUserMembership = baseController?.GetActiveUserMembership();
            var isAuthorised = activeUserMembership != null && (condition?.Invoke().Value ?? true) || activeUserMembership?.IsAuthorisedToViewPaidContent() == true || SessionHelper.CurrentUserIsAdmin();
            var isProfile = typeof(T) == typeof(NineStarKiModel);
            var isCompatibility = typeof(T) == typeof(CompatibilityModel);
            var div = new TagBuilder(Tags.Div);

            if (!(WebSecurity.IsAuthenticated && isAuthorised))
            {
                div.MergeAttribute(Base.WebApplication.Constants.Html.Attributes.Style, "display: none !important;");
                div.MergeAttribute(Base.WebApplication.Constants.Html.Attributes.Class, "paywall-remove-element");
            }

            if (!silent && !isAuthorised)
            {
                var retrieveLast = GetSectionCode(section);
                var centerDiv = new TagBuilder(Tags.Div);
                centerDiv.MergeAttribute(Base.WebApplication.Constants.Html.Attributes.Class,
                    "upgrade-container center-block");
                html.ViewContext.Writer.WriteLine(centerDiv.ToString(TagRenderMode.StartTag));
                if (WebSecurity.IsAuthenticated)
                {
                    if (hidePadlock)
                    {
                        html.ViewContext.Writer.Write(html.Partial("UpgradePromptNoPadlock", retrieveLast));
                    }
                    else
                    {
                        html.ViewContext.Writer.Write(html.Partial("UpgradePrompt", retrieveLast));
                    }
                }
                else
                {
                    if (hidePadlock)
                    {
                        html.ViewContext.Writer.Write(html.Partial("LoginPromptNoPadlock", retrieveLast));
                    }
                    else
                    {
                        html.ViewContext.Writer.Write(html.Partial("LoginPrompt", retrieveLast));
                    }
                }

                if (!string.IsNullOrEmpty(displayHtml))
                {
                    html.ViewContext.Writer.WriteLine(displayHtml);
                }

                html.ViewContext.Writer.WriteLine(centerDiv.ToString(TagRenderMode.EndTag));
            }

            html.ViewContext.Writer.WriteLine(div.ToString(TagRenderMode.StartTag));
            return new TagCloser(html, Tags.Div);
        }

        public static MvcHtmlString PayWallContent(this HtmlHelper html, ESection section, string content, bool showPadlock = false)
        {
            return html.PayWallContent<NineStarKiModel>(section, null, content, showPadlock);
        }

        public static MvcHtmlString PayWallContent<T>(this HtmlHelper html, ESection section, T model, string content, bool showPadlock = false, MembershipOption.ESubscriptionType subscriptionType = MembershipOption.ESubscriptionType.WeeklyPlatinum)
        {
            var baseController = html.ViewContext.Controller as BaseNineStarKiController;
            var activeUserMembership = baseController?.GetActiveUserMembership();
            var isAuthorised = (activeUserMembership != null && (activeUserMembership.IsAuthorisedToViewPaidContent() || 
                                activeUserMembership.MembershipOption.SubscriptionType >= subscriptionType)) ||
                                SessionHelper.CurrentUserIsAdmin();

            if (!(WebSecurity.IsAuthenticated && isAuthorised))
            {
                SetLast(section, model);
            }

            if (!isAuthorised)
            {
                using (StringWriter writer = new StringWriter())
                {
                    var retrieveLast = GetSectionCode(section);
                    var centerDiv = new TagBuilder(Tags.Div);
                    centerDiv.MergeAttribute(Base.WebApplication.Constants.Html.Attributes.Class,
                        "upgrade-container center-block");
                    writer.WriteLine(centerDiv.ToString(TagRenderMode.StartTag));

                    if (showPadlock)
                    {
                        if (WebSecurity.IsAuthenticated)
                        {
                            writer.WriteLine(html.Partial("UpgradePrompt", retrieveLast));
                        }
                        else
                        {
                            writer.WriteLine(html.Partial("LoginPrompt", retrieveLast));
                        }
                    }
                    else
                    {
                        var viewModel = new PromptViewModel
                        {
                            Content = content,
                            RetrieveLast = retrieveLast
                        };

                        if (WebSecurity.IsAuthenticated)
                        {
                            writer.WriteLine(html.Partial("UpgradePromptGentle", viewModel));
                        }
                        else
                        {
                            writer.WriteLine(html.Partial("LoginPromptGentle", viewModel));
                        }
                    }

                    writer.WriteLine(centerDiv.ToString(TagRenderMode.EndTag));

                    content = writer.ToString();
                }
            }

            return new MvcHtmlString(content);
        }
        
        public static IDisposable BeginBootstrapFormWithToken(this HtmlHelper html, string title = "", string titleTag = Tags.H2, bool insertAntiForgeryToken = true)
        {
            var div = new TagBuilder(Tags.Div);
            html.ViewContext.Writer.WriteLine(div.ToString(TagRenderMode.StartTag));

            if (insertAntiForgeryToken)
            {
                string tokenHtml;

                if (html.ViewBag.AntiForgeryToken == null)
                {
                    // Generate the anti-forgery token manually
                    tokenHtml = html.AntiForgeryToken().ToHtmlString();
                }
                else
                {
                    var token = html.ViewBag.AntiForgeryToken as string;
                    if (!string.IsNullOrEmpty(token))
                    {
                        tokenHtml = $"<input type=\"hidden\" name=\"__RequestVerificationToken\" value=\"{token}\" />";
                    }
                    else
                    {
                        tokenHtml = "";
                    }
                }

                html.ViewContext.Writer.WriteLine(tokenHtml);
            }

            html.ViewContext.Writer.WriteLine(html.BootstrapValidationSummary());

            if (!string.IsNullOrEmpty(title))
            {
                var h = new TagBuilder(titleTag);
                h.SetInnerText(title);
                html.ViewContext.Writer.WriteLine(h.ToString());
            }

            return new TagCloser(html, Tags.Div);
        }
        
        private static string GetSectionCode(ESection section)
        {
            return section.GetAttribute<EnumDescriptionAttribute>().CultureCode;
        }

        private static void SetLast(ESection section, object model)
        {
            if (model != null)
            {
                switch (section)
                {
                    case ESection.Profile:
                        SessionHelper.SetLastProfile(model as NineStarKiModel);
                        break;

                    case ESection.Compatibility:
                        SessionHelper.SetLastCompatibility(model as CompatibilityModel);
                        break;

                    case ESection.Predictions:
                        SessionHelper.SetLastPrediction(model as NineStarKiModel);
                        break;

                    case ESection.Biorhythms:
                        SessionHelper.SetLastBiorhythm(model as NineStarKiModel);
                        break;

                    case ESection.KnowledgeBase:
                        SessionHelper.SetLastKnowledgeBase(model as string);
                        break;
                }
            }
        }
    }
}