using K9.Base.DataAccessLayer.Enums;
using K9.DataAccessLayer.Enums;
using K9.WebApplication.Enums;
using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using TimeZone = K9.DataAccessLayer.Models.TimeZone;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/metadata")]
    public class MobileMetadataController : MobileApiControllerBase
    {
        private readonly DbContext _database;
        public MobileMetadataController(INineStarKiPackage package, DbContext database) : base(package) { _database = database; }

        [HttpGet, Route("options")]
        public ActionResult Options() => ApiSuccess(new
        {
            Culture = CultureInfo.CurrentUICulture.Name,
            CalculationMethods = MobileMetadataHelper.EnumOptions(typeof(ECalculationMethod)),
            CalculatorTypes = MobileMetadataHelper.EnumOptions(typeof(ECalculatorType)),
            HouseDisplays = MobileMetadataHelper.EnumOptions(typeof(EHousesDisplay)),
            EnergyDisplays = MobileMetadataHelper.EnumOptions(typeof(EEnergyDisplay)),
            PredictionControls = MobileCalculationMapper.PredictionControls(),
            Genders = MobileMetadataHelper.EnumOptions(typeof(EGender)),
            PlannerViews = MobileMetadataHelper.EnumOptions(typeof(EPlannerView)),
            Scopes = MobileMetadataHelper.EnumOptions(typeof(EScopeDisplay)),
            ForecastTypes = MobileMetadataHelper.EnumOptions(typeof(EForecastType)),
            ConsultationDurations = MobileMetadataHelper.EnumOptions(typeof(EConsultationDuration))
        });

        [HttpGet, Route("timezones")]
        public ActionResult TimeZones()
        {
            // Mirror the extended timezone selector, preserving its IANA/Windows
            // identifier rather than asking a native app to invent a mapping.
            var zones = _database.Set<TimeZone>().AsNoTracking().Where(e => !e.IsDeleted)
                .OrderBy(e => e.FriendlyName).Select(e => new { e.FriendlyName, e.TimeZoneId }).ToList();
            return ApiSuccess(zones.GroupBy(e => e.FriendlyName).Select(e => new
            { Label = e.Key, Value = e.First().TimeZoneId }).ToArray());
        }
    }
}
