using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Constants;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.Helpers;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using NLog;
using System.Linq;
using System.Web.Mvc;
using K9.Base.WebApplication.ViewModels;

namespace K9.WebApplication.Controllers
{
    public class ArchiveController : BaseController
    {
        private readonly IRepository<ArchiveCategory> _archiveCategoryRepo;
        private readonly IRepository<ArchiveItem> _archiveItemRepo;

        public ArchiveController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IRepository<ArchiveCategory> archiveCategoryRepo, IRepository<ArchiveItem> archiveItemRepo, IFileSourceHelper fileSourceHelper)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper)
        {
            _archiveCategoryRepo = archiveCategoryRepo;
            _archiveItemRepo = archiveItemRepo;
        }

        public ActionResult Index(int? categoryId)
        {
            var archiveCategories = _archiveCategoryRepo.List();
            var archiveItemsToDisplay = _archiveItemRepo.Find(item => item.CategoryId == categoryId).ToList();
            var archiveItemsByLanguage = archiveItemsToDisplay.Where(item =>
                item.LanguageCode.Equals(string.Empty) ||
                item.LanguageCode == SessionHelper.GetStringValue(SessionConstants.LanguageCode)).ToList();
            var archiveModel = new ArchiveViewModel
            {
                CategoryId = categoryId ?? 0,
                ArchiveCategories = _archiveCategoryRepo.List()
                    .OrderBy(_ => _.Name)
                    .Select(a =>
                    {
                        var archiveItems = _archiveItemRepo.Find(item => item.CategoryId == a.Id).ToList();
                        var archiveItemsPerCategoryByLanguage = archiveItems.Where(item =>
                            item.LanguageCode.Equals(string.Empty) ||
                            item.LanguageCode == SessionHelper.GetStringValue(SessionConstants.LanguageCode)).ToList();
                        return new ArchiveCategoryViewModel
                        {
                            ArchiveCategory = a,
                            Items = archiveItemsPerCategoryByLanguage
                        };
                    }).ToList(),
                SelectedArchive = categoryId > 0 ? new ArchiveCategoryViewModel
                {
                    ArchiveCategory = archiveCategories.FirstOrDefault(_ => _.Id == categoryId),
                    Items = archiveItemsByLanguage
                } : null
            };
            archiveModel.SelectedArchive?.Items.ForEach(item => LoadUploadedFiles(item));
            return View(archiveModel);
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }
    }
}
