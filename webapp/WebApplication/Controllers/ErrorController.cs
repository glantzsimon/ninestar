﻿using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
	public class ErrorController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult NotFound()
		{
			return View();
		}

		public ActionResult Unauthorized()
		{
			return View("Unauthorized");
		}
	}
}
