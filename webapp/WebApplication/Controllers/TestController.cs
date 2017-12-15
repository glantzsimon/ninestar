using K9.Base.Globalisation;
using K9.Base.WebApplication.Config;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.Enums;
using K9.Base.WebApplication.Options;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Models;
using NLog;
using System;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class TestController : BaseController
	{
	    private readonly IMailer _mailer;
	    private readonly WebsiteConfiguration _config;

	    public TestController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IMailer mailer, IOptions<WebsiteConfiguration> config, IFileSourceHelper fileSourceHelper)
			: base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
	    {
	        _mailer = mailer;
	        _config = config.Value;
	    }

		public ActionResult Index()
		{
		    return View();
		}

        [HttpPost]
		public ActionResult SendMail(SendMailModel model)
		{
		    try
		    {
		        _mailer.SendEmail(model.Subject, model.Body, model.EmailAddress, _config.CompanyName);
		        ViewBag.IsPopupAlert = true;
		        ViewBag.AlertOptions = new AlertOptions
		        {
		            AlertType = EAlertType.Success,
		            Message = Dictionary.Success,
		            OtherMessage = "Your email was sent!"
		        };
            }
		    catch (Exception e)
		    {
		        ViewBag.IsPopupAlert = true;
		        ViewBag.AlertOptions = new AlertOptions
		        {
		            AlertType = EAlertType.Fail,
		            Message = Dictionary.Error,
		            OtherMessage = e.Message
		        };
            }

		    return View("Index");
		}
		
		public override string GetObjectName()
		{
			return string.Empty;
		}
	}
}
