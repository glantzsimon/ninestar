using K9.DataAccess.Models;
using K9.WebApplication.UnitsOfWork;

namespace K9.WebApplication.Controllers
{
	public class StudentsController : BaseController<Student>
	{
		public StudentsController(IControllerPackage<Student> controllerPackage) : base(controllerPackage)
		{
		}
	}
}
