using K9.SharedLibrary.Helpers;
using K9.WebApplication.Packages;
using System.Globalization;
using System.Web.Mvc;
using Dictionary = K9.Globalisation.Dictionary;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/content")]
    public class MobileContentController : MobileApiControllerBase
    {
        public MobileContentController(INineStarKiPackage package) : base(package) { }

        [HttpGet, Route("{page}")]
        public ActionResult Page(string page)
        {
            string title;
            string body;
            switch (page)
            {
                case "privacy": title = Dictionary.PrivacyPolicyTitle; body = Dictionary.PrivacyPolicy; break;
                case "terms": title = Dictionary.TermsOfServiceTitle; body = Dictionary.TermsOfService; break;
                case "faq":
                    title = Dictionary.FAQTitle;
                    body = TemplateParser.Parse(Dictionary.faq, new { CalculationMethodsLink = Url.Action("CalculationMethods", "Home") });
                    break;
                case "remove-data":
                    title = Dictionary.HowToRemoveYourDataTitle;
                    body = TemplateParser.Parse(Dictionary.HowToRemoveYourData, new { RemoveDataUrl = Url.Action("RemoveMyData", "Account") });
                    break;
                case "calculation-methods": title = Dictionary.CalculationMethods; body = Dictionary.calculation_methods; break;
                case "southern-hemisphere": title = Dictionary.SouthernHemisphereInversionOfDailyKi; body = Dictionary.southern_hemisphere_dailyki_inversion; break;
                case "ai-astrologer": title = Dictionary.AIAstrologer; body = Dictionary.gptinfo; break;
                default: return ApiError(404, "content_not_found");
            }
            // Resource-authored prose/HTML is content. No Razor layout or controls
            // are rendered, and no resource key or filesystem path is accepted.
            return ApiSuccess(new { Page = page, Title = title, HtmlBody = body, Culture = CultureInfo.CurrentUICulture.Name });
        }
    }
}
