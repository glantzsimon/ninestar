using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.UnitsOfWork;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
	public class ArchiveCategoriesController : BaseController<ArchiveItemCategory>
	{
		public ArchiveCategoriesController(IControllerPackage<ArchiveItemCategory> controllerPackage) : base(controllerPackage)
		{
		}
	}
}
