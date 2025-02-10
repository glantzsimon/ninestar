using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.EventArgs;
using K9.Base.WebApplication.UnitsOfWork;
using System;
using System.Web.Mvc;
using K9.WebApplication.Helpers;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    public class ArchiveItemsController : BaseController<ArchiveItem>
    {

        public ArchiveItemsController(IControllerPackage<ArchiveItem> controllerPackage)
            : base(controllerPackage)
        {
            RecordBeforeCreate += ArchiveItemsController_RecordBeforeCreate;
        }

        void ArchiveItemsController_RecordBeforeCreate(object sender, CrudEventArgs e)
        {
            var archiveItem = e.Item as ArchiveItem;
            archiveItem.PublishedBy = WebSecurity.IsAuthenticated ? Current.UserName : string.Empty;
            archiveItem.PublishedOn = DateTime.Now;
        }

    }
}