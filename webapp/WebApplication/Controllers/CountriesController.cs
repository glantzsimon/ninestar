using K9.DataAccess.Models;
using K9.DataAccess.Respositories;
using NLog;

namespace K9.WebApplication.Controllers
{
	public class CountriesController : BaseController<Country>
	{
		public CountriesController(IRepository<Country> repository, ILogger logger)
			: base(repository, logger)
		{
		}
	}
}
