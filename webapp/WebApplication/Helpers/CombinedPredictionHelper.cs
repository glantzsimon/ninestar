using K9.Base.DataAccessLayer.Enums;
using K9.DataAccessLayer.Enums;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using System;
using System.Globalization;
using System.Linq;
using K9.WebApplication.Extensions;

namespace K9.WebApplication.Helpers
{
    public static class CombinedPredictionHelper
    {
        public static string GetCalendarText(int year, int month, int day)
        {
            // Use the same energy validation as website readings, but a separate
            // personal resource family and plain-text presentation for ICS feeds.
            new CombinedPrediction(year, month, day);
            var key = string.Format(CultureInfo.InvariantCulture, "CalendarCombinedPrediction_{0}_{1}_{2}", year, month, day);
            var template = K9.Globalisation.Dictionary.ResourceManager.GetString(key, K9.Globalisation.Dictionary.Culture);
            if (string.IsNullOrWhiteSpace(template))
                throw new InvalidOperationException("Calendar prediction resource not found: " + key);
            return string.Format(K9.Globalisation.Dictionary.Culture ?? CultureInfo.CurrentUICulture, template,
                K9.Globalisation.Dictionary.CalendarIdealForHeading,
                K9.Globalisation.Dictionary.CalendarBeMindfulOfHeading).TrimEnd();
        }

        public static string GetHtml(int year, int month, int? day = null, int? afternoonDay = null, bool isGlobal = false)
        {
            // Also validate callers outside a CombinedPrediction instance.
            var context = new CombinedPrediction(year, month, day, afternoonDay, isGlobal);
            var key = day.HasValue
                ? string.Format(CultureInfo.InvariantCulture, "_{0}_{1}_{2}", year, month, day)
                : string.Format(CultureInfo.InvariantCulture, "CombinedPrediction_{0}_{1}", year, month);
            if (isGlobal)
                key = "GlobalCombinedPrediction_" + year.ToString(CultureInfo.InvariantCulture) + "_" + month.ToString(CultureInfo.InvariantCulture) +
                    (day.HasValue ? "_" + day.Value.ToString(CultureInfo.InvariantCulture) : "");
            var html = K9.Globalisation.Dictionary.ResourceManager.GetString(key, K9.Globalisation.Dictionary.Culture);
            if (string.IsNullOrWhiteSpace(html))
                throw new InvalidOperationException("Combined prediction resource not found: " + key);
            if (context.AfternoonDay.HasValue)
                html += "<h5>" + K9.Globalisation.Dictionary.AfternoonEnergyLabel + "</h5>" +
                    GetHtml(year, month, context.AfternoonDay.Value, isGlobal: isGlobal);
            return html;
        }

        public static CombinedPrediction ForEnergies(NineStarKiEnergiesModel energies, bool daily, bool isGlobal = false)
        {
            return new CombinedPrediction(energies.Year.EnergyNumber, energies.Month.EnergyNumber,
                daily ? (int?)energies.Day.EnergyNumber : null,
                daily ? energies.Day2?.EnergyNumber : null, isGlobal);
        }

        public static void PopulateYearPlanner(PlannerViewModel planner)
        {
            if (planner.View != EPlannerView.Year)
                return;
            foreach (var row in planner.Energies)
                row.CombinedPrediction = new CombinedPrediction(planner.Energy.EnergyNumber, row.Energy.EnergyNumber,
                    isGlobal: planner.Display == EScopeDisplay.GlobalKi);
        }

        // Decorate only request-owned UI planners. Calendar exports and the generic
        // calculation cache do not acquire presentation text or extra astronomy work.
        public static void PopulatePlanner(PlannerViewModel planner, IAstronomyService astronomy)
        {
            PopulateYearPlanner(planner);
            if (planner.View != EPlannerView.Month && planner.View != EPlannerView.Day)
                return;

            var profile = planner.NineStarKiModel;
            var personal = planner.Display == EScopeDisplay.PersonalKi;
            var date = DateTime.SpecifyKind(planner.PeriodStartsOn.Date, DateTimeKind.Unspecified);
            var probe = planner.View == EPlannerView.Month ? date.AddDays(3) : date;
            var invertMonths = personal && profile.CalculationMethod == ECalculationMethod.Chinese && profile.PersonModel.Gender.IsYin();
            var months = astronomy.GetNineStarKiMonthlyPeriods(probe, profile.UserTimeZoneId, invertMonths);
            // Exact-instant Lichun selection can return the previous year's rows.
            if (!months.Any(e => e.PeriodStartsOn.Date <= date && e.PeriodEndsOn.Date >= date))
                months = astronomy.GetNineStarKiMonthlyPeriods(date.AddDays(3), profile.UserTimeZoneId, invertMonths);
            var month = months.Single(e => e.PeriodStartsOn.Date <= date && e.PeriodEndsOn.Date >= date);
            var year = astronomy.GetNineStarKiYearlyPeriod(month.PeriodStartsOn.AddDays(3), profile.UserTimeZoneId);
            var yearEnergy = personal
                ? profile.GetPersonalCycleEnergy(year.YearlyKi, ENineStarKiEnergyCycleType.YearlyCycleEnergy)
                : profile.GetGlobalCycleEnergy(year.YearlyKi, ENineStarKiEnergyCycleType.YearlyCycleEnergy);
            var monthEnergy = personal
                ? profile.GetPersonalCycleEnergy(month.MonthlyKi, ENineStarKiEnergyCycleType.MonthlyCycleEnergy)
                : profile.GetGlobalCycleEnergy(month.MonthlyKi, ENineStarKiEnergyCycleType.MonthlyCycleEnergy);

            if (planner.View == EPlannerView.Month)
            {
                planner.CombinedPrediction = new CombinedPrediction(yearEnergy.EnergyNumber, monthEnergy.EnergyNumber, isGlobal: !personal);
                foreach (var row in planner.Energies)
                    row.CombinedPrediction = new CombinedPrediction(yearEnergy.EnergyNumber, monthEnergy.EnergyNumber,
                        row.Energy.EnergyNumber, row.SecondEnergy?.EnergyNumber, !personal);
            }
            else
            {
                // One existing cached month batch, never a full profile per day.
                var daily = astronomy.GetNineStarKiDailyEnergiesForMonth(month.PeriodStartsOn.AddDays(3), profile.UserTimeZoneId)
                    .Single(e => e.Day.Date == date);
                int MapDay(int ki)
                {
                    if (profile.InvertDailyAndHourlyCycleKiForSouthernHemisphere)
                        ki = NineStarKiModel.GetOppositeEnergyInMagicSquare(ki);
                    return (personal ? profile.GetPersonalCycleEnergy(ki, ENineStarKiEnergyCycleType.DailyEnergy)
                        : profile.GetGlobalCycleEnergy(ki, ENineStarKiEnergyCycleType.DailyEnergy)).EnergyNumber;
                }
                planner.CombinedPrediction = new CombinedPrediction(yearEnergy.EnergyNumber, monthEnergy.EnergyNumber,
                    MapDay(daily.MorningKi), daily.AfternoonKi.HasValue ? (int?)MapDay(daily.AfternoonKi.Value) : null, !personal);
            }
        }
    }
}
