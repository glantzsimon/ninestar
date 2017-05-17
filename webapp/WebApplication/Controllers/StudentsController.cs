using K9.DataAccess.Models;
using K9.DataAccess.Respositories;
using NLog;

namespace K9.WebApplication.Controllers
{
	public class StudentsController : BaseController<Student>
	{
		public StudentsController(IRepository<Student> repository, ILogger logger) : base(repository, logger)
		{
		}
	}
}
