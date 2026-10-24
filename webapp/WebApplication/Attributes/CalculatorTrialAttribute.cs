using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using System.Web.Mvc;
using System.Web.Routing;

namespace K9.WebApplication.Attributes
{
    // Applied only to the website's calculator submissions, never the mobile/plugin API.
    public class CalculatorTrialAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Response.Cache.SetNoStore();
            if (context.HttpContext.User?.Identity?.IsAuthenticated == true ||
                !CalculatorTrialHelper.HasUsedTrial(context.HttpContext)) return;

            var controller = context.ActionDescriptor.ControllerDescriptor.ControllerName;
            var returnUrl = new UrlHelper(context.RequestContext).Action("Index", controller);
            context.Controller.TempData["CalculatorSignInRequired"] = true;
            context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
            {
                controller = "Account", action = "Login", returnUrl
            }));
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.User?.Identity?.IsAuthenticated == true || context.Exception != null ||
                !context.Controller.ViewData.ModelState.IsValid || !(context.Result is ViewResult view)) return;

            var model = view.Model;
            var success = model is NineStarKiModel chart && chart.IsProcessed ||
                model is CompatibilityModel compatibility && compatibility.IsProcessed ||
                model is NumerologyModel numerology && numerology.IsProcessed ||
                model is PredictionsViewModel predictions && predictions.NineStarKiModel?.IsProcessed == true ||
                model is BioRhythmsModel biorhythms && biorhythms.BiorhythmResults?.Count > 0 ||
                model is IChingViewModel iching && iching.Hexagram != null;
            if (success) CalculatorTrialHelper.RecordResult(context.HttpContext);
        }
    }
}
