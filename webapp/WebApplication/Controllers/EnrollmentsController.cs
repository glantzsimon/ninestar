using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using NLog;

namespace K9.WebApplication.Controllers
{
	public class EnrollmentsController : BaseController<Enrollment>
	{
		public EnrollmentsController(IRepository<Enrollment> repository, ILogger logger, IDataTableAjaxHelper<Enrollment> ajaxHelper)
			: base(repository, logger, ajaxHelper)
		{
		}

		[Authorize]
		public ActionResult ForStudent(int studentId)
		{
			return View(studentId);
		}

		[Authorize]
		public ActionResult ListForStudent(int studentId)
		{
			return List("StudentId", studentId);
		}
	}
}
