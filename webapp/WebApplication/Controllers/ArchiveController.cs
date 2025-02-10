using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Constants;
using K9.Base.WebApplication.Helpers;
using K9.Base.WebApplication.ViewModels;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Services;
using NLog;
using System.Linq;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public class ArchiveController : BaseNineStarKiController
    {
        private readonly IRepository<ArchiveItemCategory> _archiveCategoryRepo;
        private readonly IRepository<ArchiveItem> _archiveItemRepo;

        public ArchiveController(ILogger logger, IDataSetsHelper dataSetsHelper, IRoles roles, IAuthentication authentication, IRepository<ArchiveItemCategory> archiveCategoryRepo, IRepository<ArchiveItem> archiveItemRepo, IFileSourceHelper fileSourceHelper, IMembershipService membershipService, IRepository<Role> rolesRepository, IRepository<UserRole> userRolesRepository)
            : base(logger, dataSetsHelper, roles, authentication, fileSourceHelper, membershipService, rolesRepository, userRolesRepository)
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
                ArchiveItemCategories = _archiveCategoryRepo.List()
                    .OrderBy(_ => _.Name)
                    .Select(a =>
                    {
                        var archiveItems = _archiveItemRepo.Find(item => item.CategoryId == a.Id).ToList();
                        var archiveItemsPerCategoryByLanguage = archiveItems.Where(item =>
                            item.LanguageCode.Equals(string.Empty) ||
                            item.LanguageCode == SessionHelper.GetStringValue(SessionConstants.LanguageCode)).ToList();
                        return new ArchiveByItemCategoryViewModel()
                        {
                            ArchiveItemCategory = a,
                            Items = archiveItemsPerCategoryByLanguage
                        };
                    }).ToList(),
                SelectedArchive = categoryId > 0 ? new ArchiveByItemCategoryViewModel
                {
                    ArchiveItemCategory = archiveCategories.FirstOrDefault(_ => _.Id == categoryId),
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
