using K9.DataAccess.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using NLog;

namespace K9.WebApplication.Controllers
{
	public class CountriesController : BaseController<Country>
	{
		public CountriesController(IRepository<Country> repository, ILogger logger, IDataTableAjaxHelper<Country> ajaxHelper, IDataSetsHelper dataSetsHelper, IRoles roles) : base(repository, logger, ajaxHelper, dataSetsHelper, roles)
		{
		}
	}
}
