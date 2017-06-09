using System.Web.Mvc;
using K9.DataAccess.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using NLog;

namespace K9.WebApplication.Controllers
{
	public class EnrollmentsController : BaseController<Enrollment>
	{

		public EnrollmentsController(IRepository<Enrollment> repository, ILogger logger, IDataTableAjaxHelper<Enrollment> ajaxHelper, IDataSetsHelper dataSetsHelper)
			: base(repository, logger, ajaxHelper, dataSetsHelper)
		{
		}

	}
}
