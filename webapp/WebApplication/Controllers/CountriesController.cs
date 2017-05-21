using K9.DataAccess.Models;
using K9.DataAccess.Respositories;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using NLog;

namespace K9.WebApplication.Controllers
{
	public class CountriesController : BaseController<Country>
	{
		public CountriesController(IRepository<Country> repository, ILogger logger, IDataTableAjaxHelper<Country> dataTableAjaxHelper)
			: base(repository, logger, dataTableAjaxHelper)
		{
		}
	}
}
