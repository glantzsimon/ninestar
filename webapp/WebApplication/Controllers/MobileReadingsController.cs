using K9.DataAccessLayer.Models;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels.MobileApi;
using System;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api/mobile/v1/readings")]
    public class MobileReadingsController : MobileApiControllerBase
    {
        private readonly INineStarKiService _profiles;
        private readonly INumerologyService _numerology;
        private readonly DbContext _database;
        public MobileReadingsController(INineStarKiPackage package, INineStarKiService profiles,
            INumerologyService numerology, DbContext database) : base(package)
        { _profiles = profiles; _numerology = numerology; _database = database; }

        [HttpPost, Route("personal-chart")]
        public ActionResult PersonalChart(MobileCalculationRequest request) => Consume("personal-chart", request,
            () => MobilePersonalChartMapper.FreeProfile(CalculateProfile(_profiles, request)));

        [HttpPost, Route("predictions")]
        public ActionResult Predictions(MobileCalculationRequest request) => Consume("predictions", request,
            () => FreePredictions(CalculateProfile(_profiles, request, true, true)));

        [HttpPost, Route("compatibility")]
        public ActionResult Compatibility(MobileCompatibilityRequest request) => Consume("compatibility", request,
            () => ComplementaryCompatibility(request));

        [HttpPost, Route("numerology")]
        public ActionResult Numerology(MobilePersonRequest request) => Consume("numerology", request,
            () => MobileCalculationMapper.Numerology(_numerology.Calculate(new NumerologyModel(request.ToPerson(false)))));

        [HttpGet, Route("predictions/me/free")]
        public ActionResult OwnPredictions(DateTime? selectedDateTime = null)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            var request = ReadOwnCalculationRequest(id);
            if (request != null) request.SelectedDateTime = selectedDateTime;
            if (!ValidInput(request)) return ApiError(409, "birth_details_required");
            return ApiSuccess(FreePredictions(CalculateProfile(_profiles, request, true, true, true)));
        }

        [HttpPost, Route("{section}/me/alchemy")]
        public async Task<ActionResult> OwnAlchemy(string section)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            var request = ReadOwnCalculationRequest(id);
            if (!ValidInput(request)) return ApiError(409, "birth_details_required");
            return await Alchemy(section, request, own: true);
        }

        [HttpPost, Route("{section}/last/alchemy")]
        public async Task<ActionResult> LastAlchemy(string section)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            if (section != "personal-chart" && section != "predictions") return ApiError(404, "reading_not_found");
            var request = ReadLastRequest<MobileCalculationRequest>("complementary-" + section);
            if (!ValidInput(request)) return ApiError(404, "reading_not_found");
            return await Alchemy(section, request);
        }

        private async Task<ActionResult> Alchemy(string section, MobileCalculationRequest request, bool own = false)
        {
            if (section != "personal-chart" && section != "predictions") return ApiError(404, "reading_not_found");
            if (!Package.SystemSettings.IsEnabledAlchemy) return ApiSuccess(new { Enabled = false });
            var profile = CalculateProfile(_profiles, request, section == "predictions", own: own);
            var result = section == "personal-chart" ? await _profiles.GetNineStarKiPersonalChartAlchemy(profile)
                : await _profiles.GetNineStarKiPredictionsAlchemy(profile);
            return ApiSuccess(new { Enabled = true, result.AlchemisedDescription, result.AlchemisedSummary });
        }

        [HttpGet, Route("{section}/last")]
        public ActionResult Last(string section)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            // This key exists only after a successful, charged complementary
            // operation in this session. Paid calculation inputs use other keys.
            switch (section)
            {
                case "personal-chart":
                case "predictions":
                    var request = ReadLastRequest<MobileCalculationRequest>("complementary-" + section);
                    if (!ValidInput(request)) return ApiError(404, "reading_not_found");
                    return ApiSuccess(section == "personal-chart"
                        ? MobilePersonalChartMapper.FreeProfile(CalculateProfile(_profiles, request))
                        : FreePredictions(CalculateProfile(_profiles, request, true, true)));
                case "compatibility":
                    var compatibility = ReadLastRequest<MobileCompatibilityRequest>("complementary-compatibility");
                    return !ValidInput(compatibility) ? (ActionResult)ApiError(404, "reading_not_found") : ApiSuccess(ComplementaryCompatibility(compatibility));
                case "numerology":
                    var person = ReadLastRequest<MobilePersonRequest>("complementary-numerology");
                    return !ValidInput(person) ? (ActionResult)ApiError(404, "reading_not_found")
                        : ApiSuccess(MobileCalculationMapper.Numerology(_numerology.Calculate(new NumerologyModel(person.ToPerson(false)))));
                default: return ApiError(404, "reading_not_found");
            }
        }

        private ActionResult Consume<T>(string section, T request, Func<object> calculate)
        {
            var error = RequireUser(out var id);
            if (error != null) return error;
            if (!ValidInput(request)) return ApiError(400, "invalid_reading_request");
            // Shared scoped DbContext: lock the membership read/decrement across
            // requests and workers, rather than trusting a client credit balance.
            using (var transaction = _database.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                var membership = Package.MembershipService.GetActiveUserMembership(id);
                if (membership?.MembershipOption?.IsFree != true) return ApiError(409, "use_paid_calculation");
                if (Remaining(membership, section) <= 0) return ApiError(403, "complementary_readings_exhausted");
                var data = calculate();
                switch (section)
                {
                    case "personal-chart": Package.MembershipService.UseComplementaryPersonalChartReading(id); break;
                    case "predictions": Package.MembershipService.UseComplementaryPredictionsReading(id); break;
                    case "compatibility": Package.MembershipService.UseComplementaryCompatibilityReading(id); break;
                    case "numerology": Package.MembershipService.UseComplementaryKarmicReading(id); break;
                    default: throw new ArgumentException(nameof(section));
                }
                var result = ApiSuccess(data); // Serialize successfully before committing the debit.
                transaction.Commit();
                SaveLastRequest("complementary-" + section, request);
                return result;
            }
        }

        private static int Remaining(UserMembership membership, string section)
        {
            switch (section)
            {
                case "personal-chart": return membership.ComplementaryPersonalChartReadingCount;
                case "predictions": return membership.ComplementaryPredictionsReadingCount;
                case "compatibility": return membership.ComplementaryCompatibilityReadingCount;
                case "numerology": return membership.ComplementaryKarmicReadingCount;
                default: throw new ArgumentException(nameof(section));
            }
        }

        private object ComplementaryCompatibility(MobileCompatibilityRequest request)
        {
            var first = request.FirstPerson.ToPerson();
            var second = request.SecondPerson.ToPerson();
            var hide = request.HideSexualChemistry || !first.IsSixteenOrOver() || !second.IsSixteenOrOver();
            return MobileCalculationMapper.Compatibility(_profiles.CalculateCompatibility(first, second, hide,
                request.CalculationMethod, request.CalculatorType), hide, includeFullProfiles: false);
        }

        private static object FreeYear(NineStarKiEnergy energy, bool global = false) => energy == null ? null : new
        {
            HouseName = energy.HouseName,
            Number = energy.EnergyNumber, Title = energy.CycleDescriptiveName,
            energy.Season, energy.SeasonDescription,
            Description = global ? energy.GlobalCycleDescription : energy.CycleDescription
        };

        private static object FreePredictions(NineStarKiModel model) => new
        {
            Labels = MobileCalculationMapper.PredictionLabels(),
            Help = MobileHelpMapper.PredictionIntroductions(),
            model.SelectedDate, Profile = MobilePersonalChartMapper.Numbers(model),
            Year = FreeYear(model.PersonalHousesOccupiedEnergies.Year),
            LunarYear = FreeYear(model.PersonalHousesOccupiedLunarEnergies?.Year),
            GlobalYear = FreeYear(model.GlobalCycleEnergies.Year, global: true)
        };
    }
}
