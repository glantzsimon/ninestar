using K9.SharedLibrary.Extensions;
using NLog;
using System.Web.Mvc;

namespace K9.WebApplication.Services
{
    public class CustomExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            var exception = filterContext.Exception;
            var controllerName = filterContext.RouteData.Values["controller"]?.ToString() ?? "Unknown";
            var actionName = filterContext.RouteData.Values["action"]?.ToString() ?? "Unknown";
            var message = exception.Message;
            var stackTrace = exception.StackTrace;
            
            LogManager.GetCurrentClassLogger().Log(LogLevel.Error, 
                $"Unhandled Error: Controller: {controllerName} => Action: {actionName} => Error: {exception.GetFullErrorMessage()} => StackTrace: {stackTrace}");

            filterContext.ExceptionHandled = true;
            
            filterContext.Result = new ViewResult
            {
                ViewName = "FriendlyError"
            };
        }
    }
}