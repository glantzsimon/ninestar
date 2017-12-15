using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.UnitsOfWork;

namespace K9.WebApplication.Controllers
{
    public class CountriesController : BaseController<Country>
	{
		public CountriesController(IControllerPackage<Country> controllerPackage) : base(controllerPackage)
		{
		}
	}
}
