using K9.DataAccess.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using NLog;

namespace K9.WebApplication.Controllers
{
	public class StudentsController : BaseController<Student>
	{
		public StudentsController(IRepository<Student> repository, ILogger logger, IDataTableAjaxHelper<Student> ajaxHelper, IDataSetsHelper dataSetsHelper, IRoles roles) : base(repository, logger, ajaxHelper, dataSetsHelper, roles)
		{
		}
	}
}
