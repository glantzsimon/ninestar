using NLog;
using System;
using System.Web.Mvc;
using ServiceStack.Text;

namespace K9.WebApplication.Filters
{
    public class LogCustomExceptionFilter : FilterAttribute, IExceptionFilter
    {
        private readonly ILogger _logger;

        public LogCustomExceptionFilter()
        {
            _logger = LogManager.GetCurrentClassLogger() as ILogger;
        }

        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                var exceptionMessage = filterContext.Exception.Message;
                var stackTrace = filterContext.Exception.StackTrace;
                var controllerName = filterContext.RouteData.Values["controller"].ToString();
                var actionName = filterContext.RouteData.Values["action"].ToString();

                var message = $"Date: {DateTime.Now.ToString()}, {Environment.NewLine}" +
                              $"Controller: {controllerName.ToTitleCase()}Controller, {Environment.NewLine}" +
                              $"Action: {actionName.ToTitleCase()}, {Environment.NewLine}" +
                              $"Error Message: {exceptionMessage}, {Environment.NewLine}{Environment.NewLine}" +
                              $"Stack Trace: {stackTrace}";
                

                _logger.Error(message);
            }
        }
    }
}