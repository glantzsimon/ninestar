using K9.DataAccess.Models;
using K9.WebApplication.UnitsOfWork;

namespace K9.WebApplication.Controllers
{
	public class EnrollmentsController : BaseController<Enrollment>
	{
		public EnrollmentsController(IControllerPackage<Enrollment> controllerPackage) : base(controllerPackage)
		{
		}
	}
}
