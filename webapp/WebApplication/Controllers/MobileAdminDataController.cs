using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels.MobileApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/admin/data")]
    public class MobileAdminDataController : MobileApiControllerBase
    {
        private readonly IMobileAdminDataService _data;
        public MobileAdminDataController(INineStarKiPackage package, IMobileAdminDataService data) : base(package) { _data = data; }

        [HttpGet, Route("")]
        public ActionResult Catalog() => Run(() => _data.Catalog());
        [HttpGet, Route("{entity}/schema")]
        public ActionResult Schema(string entity) => Run(() => _data.Schema(entity));
        [HttpGet, Route("{entity}")]
        public ActionResult List(string entity, int afterId = 0, int limit = 50) => Run(() => _data.List(entity, afterId, limit));
        [HttpGet, Route("{entity}/{id:int}")]
        public ActionResult Find(string entity, int id) => Run(() => _data.Find(entity, id));

        [HttpPost, Route("{entity}/save"), ValidateInput(false)]
        public ActionResult Save(string entity, MobileAdminWriteRequest request) => Run(() => _data.Save(entity, request));

        [HttpPost, Route("{entity}/delete")]
        public ActionResult Delete(string entity, int id) => Run(() => { _data.Delete(entity, id); return new { Deleted = true }; });

        private ActionResult Run(Func<object> operation)
        {
            var error = RequireUser(out var id, admin: true);
            if (error != null) return error;
            if (!ModelState.IsValid) return ApiError(400, "invalid_admin_request");
            try { return ApiSuccess(operation()); }
            catch (KeyNotFoundException) { return ApiError(404, "record_not_found"); }
            catch (ValidationException) { return ApiError(422, "invalid_record"); }
            catch (ArgumentException) { return ApiError(400, "invalid_record_fields"); }
            catch (NotSupportedException) { return ApiError(405, "read_only_contract"); }
            catch (UnauthorizedAccessException) { return ApiError(403, "administrator_required"); }
        }
    }
}
