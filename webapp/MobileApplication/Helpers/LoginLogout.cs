using K9.Base.Globalisation;
using K9.Base.WebApplication.Constants.Html;
using K9.Base.WebApplication.Extensions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.Base.WebApplication.Helpers;

namespace K9.WebApplication.Helpers
{
    public static partial class HtmlHelpers
    {

        public static MvcHtmlString LoginLogoutControl(this HtmlHelper html)
        {
            var sb = new StringBuilder();
            var authentication = html.GetAuthentication();

            if (authentication.IsAuthenticated)
            {
                var icon = new TagBuilder(Tags.Icon);
                //icon.MergeAttribute(Base.WebApplication.Constants.Html.Attributes.Class, "fa fa-sign-out");

                var textSpan = new TagBuilder(Tags.Span);
                textSpan.MergeAttribute(Base.WebApplication.Constants.Html.Attributes.Class, "site-controls-text");
                textSpan.InnerHtml = Dictionary.LogOut;

                var anchor = new TagBuilder(Tags.Anchor);
                anchor.MergeAttribute(Base.WebApplication.Constants.Html.Attributes.Href, html.GeturlHeler().Action("LogOff", "Account"));
                anchor.MergeAttribute(Base.WebApplication.Constants.Html.Attributes.Title, Dictionary.LogOut);
                anchor.InnerHtml = $"{icon} {textSpan}";

                var li = new TagBuilder(Tags.Li) { InnerHtml = anchor.ToString() };
                li.MergeAttribute(Base.WebApplication.Constants.Html.Attributes.Class, html.ViewContext.GetActiveClass("LogOff", "Account"));
                sb.Append(li);
            }
            else
            {
                var icon = new TagBuilder(Tags.Icon);
                //icon.MergeAttribute(Base.WebApplication.Constants.Html.Attributes.Class, "fa fa-sign-in");

                var textSpan = new TagBuilder(Tags.Span);
                textSpan.MergeAttribute(Base.WebApplication.Constants.Html.Attributes.Class, "site-controls-text");
                textSpan.InnerHtml = Dictionary.LogIn;

                var anchor = new TagBuilder(Tags.Anchor);
                anchor.MergeAttribute(Base.WebApplication.Constants.Html.Attributes.Href, html.GeturlHeler().Action("Login", "Account"));
                anchor.MergeAttribute(Base.WebApplication.Constants.Html.Attributes.Title, Globalisation.Dictionary.LogIn);
                anchor.InnerHtml = $"{icon} {textSpan}";

                var li = new TagBuilder(Tags.Li) { InnerHtml = html.ActionLink(Globalisation.Dictionary.CreateAccount, "Register", "Account").ToString() };
                li.MergeAttribute(Base.WebApplication.Constants.Html.Attributes.Class, html.ViewContext.GetActiveClass("Register", "Account"));
                sb.Append(li);

                li = new TagBuilder(Tags.Li) { InnerHtml = anchor.ToString() };
                li.MergeAttribute(Base.WebApplication.Constants.Html.Attributes.Class, html.ViewContext.GetActiveClass("Login", "Account"));
                sb.Append(li);
            }

            return MvcHtmlString.Create(sb.ToString());
        }

    }
}