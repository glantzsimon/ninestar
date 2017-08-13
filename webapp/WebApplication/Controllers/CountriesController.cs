using K9.DataAccess.Models;
using K9.WebApplication.UnitsOfWork;

namespace K9.WebApplication.Controllers
{
	public class CountriesController : BaseController<Country>
	{
		public CountriesController(IControllerPackage<Country> controllerPackage) : base(controllerPackage)
		{
		}
	}
}
