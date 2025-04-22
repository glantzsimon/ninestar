using K9.Base.DataAccessLayer.Attributes;
using K9.Base.WebApplication.Constants;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.EventArgs;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers.Html;
using K9.SharedLibrary.Models;
using K9.WebApplication.Enums;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.ViewModels;
using SessionHelper = K9.Base.WebApplication.Helpers.SessionHelper;

namespace K9.WebApplication.Controllers
{
    public class BaseNineStarKiController : BaseController
    {
        public BaseNineStarKiController(INineStarKiPackage nineStarKiPackage)
            : base(nineStarKiPackage.Logger, nineStarKiPackage.DataSetsHelper,
                nineStarKiPackage.Roles, nineStarKiPackage.Authentication, nineStarKiPackage.FileSourceHelper)
        {
            My = nineStarKiPackage;
            UrlHelper = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            SetBetaWarningSessionVariable();
            SetSessionRoles(Current.UserId);

            ViewBag.DeviceType = GetDeviceType();
        }

        public INineStarKiPackage My { get; }

        public UrlHelper UrlHelper { get; }

        public ActionResult SetLanguage(string languageCode, string cultureCode)
        {
            Session[SessionConstants.LanguageCode] = languageCode;
            Session[SessionConstants.CultureCode] = cultureCode;
            return Redirect(Request.UrlReferrer?.ToString());
        }

        public UserMembership GetActiveUserMembership()
        {
            if (Authentication.IsAuthenticated)
            {
                return My.MembershipService.GetActiveUserMembership(Authentication.CurrentUserId);
            }

            return null;
        }

        public ActionResult RedirectToLastSaved()
        {
            // Redirect to previous profile or compatibility reading if set
            var lastProfile = Helpers.SessionHelper.GetLastProfile(true, false);
            var lastCompatibility = Helpers.SessionHelper.GetLastCompatibility(true, false);
            var lastPrediction = Helpers.SessionHelper.GetLastPrediction(true, false);
            var lastBiorhythm = Helpers.SessionHelper.GetLastBiorhythm(true, false);
            var lastKnowledgeBase = Helpers.SessionHelper.GetLastKnowledgeBase(true, false);

            var lastItems = new List<RetrieveLastModel>
            {
                lastProfile,
                lastCompatibility,
                lastPrediction,
                lastBiorhythm,
                lastKnowledgeBase
            };

            var lastStoredItem = lastItems.OrderByDescending(e => e.StoredOn).FirstOrDefault();

            if (lastStoredItem != null && lastStoredItem.StoredOn.Value.Date == DateTime.Today)
            {
                switch (lastStoredItem.Section)
                {
                    case ESection.Profile:
                        return RedirectToAction("RetrieveLastProfile", "PersonalChart");

                    case ESection.Compatibility:
                        return RedirectToAction("RetrieveLastCompatibility", "PersonalChart");

                    case ESection.Predictions:
                        return RedirectToAction("RetrieveLastPrediction", "Predictions");

                    case ESection.Biorhythms:
                        return RedirectToAction("RetrieveLastBiorhythms", "Biorhythms");

                    case ESection.KnowledgeBase:
                        return RedirectToAction("RetrieveLastKnowledgeBaseSection", "KnowledgeBase");
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public EDeviceType GetDeviceType()
        {
            return new BrowserInfo(Request.Headers["User-Agent"]).DeviceType;
        }

        public JsonResult UpdateUserPreferenceInt(string key, int value)
        {
            try
            {
                SessionHelper.SetValue(key, value);
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = e.GetFullErrorMessage() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateUserPreferenceBool(string key, bool value)
        {
            try
            {
                SessionHelper.SetValue(key, value);
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = e.GetFullErrorMessage() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public void SetSessionRoles(int userId)
        {
            Helpers.SessionHelper.SetCurrentUserRoles(My.RolesRepository, My.UserRolesRepository, userId);
        }

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase file, int? id = null, string folderName = "upload")
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    Logger.Error("BaseController => UploadImage => No file received.");
                    return Json(new { success = false, message = "No file received." });
                }

                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var ext = Path.GetExtension(file.FileName);
                var safeFileName = fileName.Slugify() + "-" + Guid.NewGuid().ToString("N").Substring(0, 6) + ext;
                var relativePath = id.HasValue && id.Value > 0
                    ? $"~/Images/{folderName}/{id}"
                    : $"~/Images/{folderName}";
                var uploadDir = Server.MapPath(relativePath);

                Directory.CreateDirectory(uploadDir);
                var path = Path.Combine(uploadDir, safeFileName);
                file.SaveAs(path);

                var url = Url.Content($"{relativePath}/{safeFileName}");

                Logger.Info($"BaseController => UploadImage => Successfully uploaded image to server: {url}");

#if DEBUG
                //return Json(new { success = true, url });
                //#else
                Logger.Info($"BaseController => UploadImage => Uploading image to Storj");

                var uploadRelativePath = id.HasValue ? $"{folderName}/{id}/{safeFileName}" : $"{folderName}/{safeFileName}";
                var absoluteFilePath = Path.Combine(uploadDir, safeFileName);

                Logger.Info($"BaseControlle => UploadImage => UploadImageToStorj => {absoluteFilePath}, {uploadRelativePath}");

                var storjUrl = UploadImageToStorj(absoluteFilePath, uploadRelativePath);

                var finalUrl = string.IsNullOrWhiteSpace(storjUrl) ? url : storjUrl;
                Logger.Info($"UploadImage => Successfully uploaded: {finalUrl}");

                return Json(new { success = true, url = finalUrl });
#endif
            }
            catch (Exception ex)
            {
                Logger.Error("UploadImage => Exception: " + ex.GetFullErrorMessage());
                return Json(new { success = false, message = "Upload failed: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult DeleteImages(DeleteFilesRequest request)
        {
            if (request?.Urls == null || !request.Urls.Any())
            {
                return Json(new { success = false, message = "No URLs provided." });
            }

            var failedDeletes = new List<string>();

            foreach (var relativePath in request.Urls)
            {
                try
                {
                    var localVirtualPath = $"~/{relativePath.TrimStart('/')}";
                    var fullPath = Server.MapPath(localVirtualPath);

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    else
                    {
                        failedDeletes.Add($"Not found: {relativePath}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"DeleteImages => Error deleting {relativePath}: {ex.Message}");
                    failedDeletes.Add($"Error: {relativePath}");
                }
            }

            if (failedDeletes.Any())
            {
                return Json(new
                {
                    success = false,
                    message = $"Some files failed to delete: {string.Join(", ", failedDeletes)}"
                });
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public PartialViewResult GetImageOptions(string typeName, int? id = null)
        {
            var model = GetDynamicFieldsModel(typeName, new object[] { id });
            return PartialView("Controls/_ImageListItems", model);
        }

        public override string GetObjectName()
        {
            return string.Empty;
        }

        public string RenderPartialViewToString(string viewName, object model)
        {
            var controllerContext = ControllerContext;
            var tempData = controllerContext.Controller.TempData;
            var viewData = new ViewDataDictionary(model);
            var stringWriter = new System.IO.StringWriter();
            var viewEngineResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);

            var viewContext = new ViewContext(controllerContext, viewEngineResult.View, viewData, tempData, stringWriter);
            viewEngineResult.View.Render(viewContext, stringWriter);
            return stringWriter.ToString();
        }

        private IDynamicFieldsModel GetDynamicFieldsModel(string modelTypeName, object[] constructorArgs = null)
        {
            // First, find the Type for the model, e.g., "Article"
            var modelType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    try { return assembly.GetTypes(); }
                    catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null); }
                })
                .FirstOrDefault(t => t.Name == modelTypeName || t.FullName == modelTypeName);

            if (modelType == null)
                throw new Exception($"Model type '{modelTypeName}' not found.");

            // Now, find a class that inherits from DynamicFieldsViewModel<T> with T == modelType
            var dynamicFieldsType = typeof(DynamicFieldsViewModel<>).MakeGenericType(modelType);

            var viewModelType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    try { return assembly.GetTypes(); }
                    catch (ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null); }
                })
                .FirstOrDefault(t =>
                    !t.IsAbstract &&
                    !t.IsInterface &&
                    dynamicFieldsType.IsAssignableFrom(t) &&
                    typeof(IDynamicFieldsModel).IsAssignableFrom(t));

            if (viewModelType == null)
                throw new Exception($"No DynamicFieldsViewModel<{modelTypeName}> implementation found.");

            // Create an instance
            var instance = Activator.CreateInstance(viewModelType, constructorArgs ?? new object[] { }) as IDynamicFieldsModel;

            if (instance == null)
                throw new Exception($"Could not instantiate '{viewModelType.FullName}'.");

            return instance;
        }

        private string UploadImageToStorj(string absoluteFilePath, string relativePath)
        {
            return My.MediaManagementService.UploadToStorj(absoluteFilePath, relativePath);
        }

        private static void SetBetaWarningSessionVariable()
        {
            var numberOfDisplays = Helpers.SessionHelper.GetIntValue(Constants.SessionConstants.BetaWarningDisplay);
            if (numberOfDisplays < 1)
            {
                numberOfDisplays++;
                SessionHelper.SetValue(Constants.SessionConstants.BetaWarningDisplay, numberOfDisplays);
            }
            else
            {
                SessionHelper.SetValue(Constants.SessionConstants.BetaWarningHide, true);
            }
        }
    }

    [EnableCaching]
    public class BaseNineStarKiController<T> : BaseController<T> where T : class, IObjectBase
    {
        public BaseNineStarKiController(IControllerPackage<T> controllerPackage, INineStarKiPackage nineStarPackage)
            : base(controllerPackage)
        {
            My = nineStarPackage;
            UrlHelper = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);

            RecordBeforeCreated += BaseNineStarKiController_RecordBeforeCreated;
            RecordBeforeUpdated += BaseNineStarKiController_RecordBeforeUpdated;
            RecordBeforeUpdate += BaseNineStarKiController_RecordBeforeUpdate;
        }

        public INineStarKiPackage My { get; }

        public UrlHelper UrlHelper { get; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            SetSessionRoles(Current.UserId);
        }

        public void SetSessionRoles(int userId)
        {
            Helpers.SessionHelper.SetCurrentUserRoles(My.RolesRepository, My.UserRolesRepository, userId);
        }

        private void BaseNineStarKiController_RecordBeforeUpdated(object sender, CrudEventArgs e)
        {
            var model = e.Item as T;
            HtmlMarkupHelper.ConvertToHtml(ref model);
        }

        private void BaseNineStarKiController_RecordBeforeCreated(object sender, CrudEventArgs e)
        {
            var model = e.Item as T;
            HtmlMarkupHelper.ConvertToHtml(ref model);
        }

        private void BaseNineStarKiController_RecordBeforeUpdate(object sender, CrudEventArgs e)
        {
            var model = e.Item as T;
            HtmlMarkupHelper.ConvertToCurly(ref model);
        }
    }

}
