using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Helpers
{
    // Every object below is an explicit projection. Never pass the source graph
    // to JsonConvert: models include back references and presentation getters.
    public static class MobileCalculationMapper
    {
        public static object Moon(MoonPhase moon) => moon == null ? null : new
        {
            moon.IlluminationPercentage, moon.IsWaxing, moon.IlluminationDisplay,
            Name = moon.GetMoonPhaseName(), Image = moon.GetMoonPhaseImageFile(),
            ArtworkId = MobileArtworkRegistry.IdForPath("astrology/moon-phases/" + moon.GetMoonPhaseImageFile()),
            moon.LunarDay, moon.LunarDayTitle, moon.LunarDayDescription, moon.YinYangComfortDescription
        };

        public static object Cycle(NineStarKiEnergy energy, bool global = false) => energy == null ? null : new
        {
            Energy = MobilePersonalChartMapper.Energy(energy),
            energy.CycleDescriptiveName, energy.Season, energy.SeasonDescription,
            Description = global ? energy.GlobalCycleDescription : energy.CycleDescription
        };

        public static object Cycles(NineStarKiEnergiesModel energies, bool global = false) => energies == null ? null : new
        {
            Epoch = Cycle(energies.Epoch, global), Generation = Cycle(energies.Generation, global),
            Year = Cycle(energies.Year, global), Month = Cycle(energies.Month, global),
            Day = Cycle(energies.Day, global), Afternoon = Cycle(energies.Day2, global),
            InvertedDay = Cycle(energies.DayInverted, global), InvertedAfternoon = Cycle(energies.Day2Inverted, global),
            Hour = Cycle(energies.Hour, global)
        };

        public static object Directions(NineStarKiDirections directions) => directions == null ? null : new
        {
            Favourable = directions.FavourableDirections?.Select(e => e.ToString()).ToArray(),
            Unfavourable = directions.UnfavourableDirections?.Where(e => e != null).Select(e => new
            {
                e.Name, e.Title, e.Description, Direction = e.Direction.ToString(), e.Score,
                Energy = MobilePersonalChartMapper.Energy(e.Energy)
            }).ToArray()
        };

        public static object PredictionLabels() => new
        {
            Periods = new
            {
                Year = K9.Globalisation.Dictionary.Year, Month = K9.Globalisation.Dictionary.Month,
                Day = K9.Globalisation.Dictionary.Day, Generation = K9.Globalisation.Dictionary.Generation,
                Epoch = K9.Globalisation.Dictionary.Epoch, Hour = K9.Globalisation.Dictionary.Hour
            },
            Scopes = new
            {
                Personal = K9.Globalisation.Dictionary.PersonalView,
                Lunar = K9.Globalisation.Dictionary.PersonalLunarView,
                Global = K9.Globalisation.Dictionary.GlobalView
            }
        };

        public static object Predictions(NineStarKiModel model, bool birthTimeKnown) => new
        {
            Labels = PredictionLabels(),
            Help = MobileHelpMapper.Predictions(),
            Profile = MobilePersonalChartMapper.Map(model, birthTimeKnown, includeHelp: false),
            Personal = Cycles(model.PersonalHousesOccupiedEnergies),
            Lunar = Cycles(model.PersonalHousesOccupiedLunarEnergies),
            Global = Cycles(model.GlobalCycleEnergies, true), MoonPhase = Moon(model.MoonPhase),
            YearlyDirections = Directions(model.YearlyDirections), MonthlyDirections = Directions(model.MonthlyDirections)
        };

        public static object Combined(CombinedPrediction combined) => combined == null ? null : new
        {
            combined.Year, combined.Month, combined.Day, combined.AfternoonDay, combined.IsGlobal, Description = combined.Html
        };

        public static object Planner(PlannerViewModel planner) => new
        {
            Help = MobileHelpMapper.Predictions(),
            planner.View, planner.Display, planner.HousesDisplay, planner.SelectedDateTime,
            planner.PeriodStartsOn, planner.PeriodEndsOn, planner.ChildView, planner.NextViewUp,
            Energy = Cycle(planner.Energy, planner.Display == EScopeDisplay.GlobalKi),
            SecondEnergy = Cycle(planner.SecondEnergy, planner.Display == EScopeDisplay.GlobalKi),
            Combined = Combined(planner.CombinedPrediction), MoonPhase = Moon(planner.MoonPhase),
            Items = planner.Energies?.Select(e => new
            {
                e.EnergyStartsOn, e.EnergyEndsOn, e.IsActive, e.ChildView,
                Energy = Cycle(e.Energy, planner.Display == EScopeDisplay.GlobalKi),
                SecondEnergy = Cycle(e.SecondEnergy, planner.Display == EScopeDisplay.GlobalKi),
                Combined = Combined(e.CombinedPrediction), MoonPhase = Moon(e.MoonPhase), Directions = Directions(e.Directions),
                MagicSquareNumbers = e.Energy?.GetMagicSquareNumbers()
            }).ToArray()
        };

        public static object Compatibility(CompatibilityModel model, bool hideSexualChemistry, bool includeFullProfiles = true)
        {
            var details = model.CompatibilityDetails;
            var score = details?.Score;
            return new
            {
                Help = MobileHelpMapper.Compatibility(),
                FirstPerson = includeFullProfiles ? (object)MobilePersonalChartMapper.Map(model.NineStarKiModel1, false, includeHelp: false) : MobilePersonalChartMapper.Numbers(model.NineStarKiModel1),
                SecondPerson = includeFullProfiles ? (object)MobilePersonalChartMapper.Map(model.NineStarKiModel2, false, includeHelp: false) : MobilePersonalChartMapper.Numbers(model.NineStarKiModel2),
                model.FundamentalEnergiesCompatibility,
                SexualChemistryDetails = hideSexualChemistry ? null : model.SexualChemistryDetails,
                Details = details == null ? null : new
                {
                    Elements = details.ElementCompatibility == null ? null : new
                    {
                        details.ElementCompatibility.FundamentalElementsTransformationDetails,
                        details.ElementCompatibility.CharacterElementsTransformationDetails,
                        details.ElementCompatibility.FundamentalElementsCompatibilityDetails,
                        details.ElementCompatibility.CharacterElementsCompatibilityDetails,
                        details.ElementCompatibility.AllOtherElementsCompatibility
                    },
                    Gender = details.GenderCompatibility == null ? null : new
                    {
                        details.GenderCompatibility.IsFundamtenalGenderSame, details.GenderCompatibility.IsCharacterGenderSame,
                        details.GenderCompatibility.IsBothGenderSame
                    },
                    Modality = details.ModalityCompatibility == null ? null : new
                    {
                        details.ModalityCompatibility.IsFundamentalModalitySame, details.ModalityCompatibility.IsCharacterModalitySame,
                        details.ModalityCompatibility.IsSurfaceModalitySame
                    },
                    BirthMonthDescription = details.BirthMonthCompatibility?.CompatibilityDetails,
                    Score = score == null ? null : new
                    {
                        score.SupportiveScoreAsPercentage, score.SameScoreAsPercentage, score.ChallengingAsPercentage,
                        score.HarmonyScore, score.ConflictScore, score.SupportScore,
                        score.MutualUnderstandingScore, score.ComplementarityScore, score.SparkScore,
                        score.LearningPotentialScore, score.OverallScore,
                        SexualChemistryScore = hideSexualChemistry ? null : score.SexualChemistryScore.ToString()
                    }
                }
            };
        }

        public static object Code(NumerologyCodeDetails code) => code == null ? null : new
        {
            code.CodeNumber, code.Name, code.Title, code.Colour, code.Purpose, code.Description,
            ArtworkId = MobileArtworkRegistry.IdForPath("numerology/" + code.NumerologyCode.ToString() + ".png"),
            code.StartDate, code.EndDate, code.Type, code.Offset, code.IsCurrent
        };

        public static object Forecast(NumerologyForecast forecast) => forecast == null ? null : new
        {
            forecast.ChartCode, forecast.ForecastType, forecast.TopNumber, forecast.BottomNumber,
            forecast.RowNumber, forecast.Name, forecast.Forecast, forecast.ChartCodesTitle
        };

        private static object NumerologyPeriods(IEnumerable<NumerologyPlannerModel> periods) => periods?.Select(e => new
        {
            e.CodeNumber, e.StartDate, e.EndDate, e.Offset, e.IsCurrent,
            ArtworkId = MobileArtworkRegistry.IdForPath("numerology/" + e.NumerologyCode.ToString() + ".png")
        }).ToArray();

        public static object Numerology(NumerologyModel model) => new
        {
            Help = MobileHelpMapper.Numerology(),
            Primary = Code(model.Primary), Emergence = Code(model.Emergence), Actualisation = Code(model.Actualisation),
            Mastery = Code(model.Mastery), BirthYear = Code(model.BirthYear), CurrentYear = Code(model.CurrentYear), CurrentMonth = Code(model.CurrentMonth),
            MonthlyPlanner = NumerologyPeriods(model.MonthlyPlannerCodes), YearlyPlanner = NumerologyPeriods(model.YearlyPlannerCodes),
            DailyPlanner = NumerologyPeriods(model.DailyPlannerCodes),
            DharmaCodes = model.DharmaCodes?.Select(e => new
            {
                e.Age, e.Year, e.NumerologyCodeNumber, e.DharmaChakraBaseCodeNumber, e.DharmaNumerologyCodeNumber, e.DharmaGroupCodeNumber
            }).ToArray(),
            YearlyForecast = Forecast(model.YearlyForecast), MonthlyForecast = Forecast(model.MonthlyForecast), DailyForecast = Forecast(model.DailyForecast)
        };

        public static object Biorhythms(BioRhythmsModel model) => new
        {
            Help = MobileHelpMapper.Biorhythms(),
            model.SelectedDate, model.MonthlyPeriodStartsOn, model.MonthlyPeriodEndsOn, model.DaysElapsedSinceBirth, model.Summary,
            Results = model.GetBiorhythmResultsByDisplayIndex().Select(e => new
            {
                e.BioRhythm.Biorhythm, e.BioRhythm.FullName, e.BioRhythm.CycleLength, e.BioRhythm.Color,
                e.SelectedDate, e.DayInterval, e.Value,
                Values = e.RangeValues?.Select(v => new { v.Date, v.Value, v.LevelDescription }).ToArray(),
                LongRangeValues = e.LongRangeValues?.Select(v => new { v.Date, v.Value, v.LevelDescription }).ToArray(),
                Trend = e.GetBiorhythmTrendDescription(), NextMaximum = e.GetDaysUntilNextMaximumString(),
                NextMinimum = e.GetDaysUntilNextMinimumString(), NextCritical = e.GetDaysUntilNextCriticalString()
            }).ToArray()
        };

        public static object Hexagram(Hexagram hexagram)
        {
            if (hexagram == null) return null;
            var info = hexagram.GetHexagramInfo();
            return new
            {
                hexagram.Number, hexagram.Name, Lines = hexagram.Lines.Select(e => e.ToString()).ToArray(),
                Info = info == null ? null : new { info.Stage, info.Name, info.Title, info.Summary, info.Details, info.ChangingLinesInterpretation }
            };
        }

        public static object KnowledgeBase(NineStarKiSummaryViewModel model,
            Dictionary<string, K9.WebApplication.ViewModels.MobileApi.MobileHelpText> help = null) => new
        {
            Help = help,
            MainEnergies = model.MainEnergies.Select(MobilePersonalChartMapper.Energy).ToArray(),
            CharacterEnergies = model.CharacterEnergies.Select(MobilePersonalChartMapper.Energy).ToArray(),
            EpochCycles = model.EightyOneYearlyCycleEnergies.Select(e => Cycle(e)).ToArray(),
            GenerationCycles = model.NineYearlyCycleEnergies.Select(e => Cycle(e)).ToArray(),
            YearCycles = model.YearlyCycleEnergies.Select(e => Cycle(e)).ToArray(),
            MonthCycles = model.MonthlyCycleEnergies.Select(e => Cycle(e)).ToArray(),
            DayCycles = model.DailyCycleEnergies.Select(e => Cycle(e)).ToArray(),
            HourCycles = model.HourlyCycleEnergies.Select(e => Cycle(e)).ToArray(),
            Modalities = new[] { model.DynamicEnergies, model.StableEnergies, model.ReflectiveEnergies }
                .Select(e => new { e.ModalityName, e.Title, e.Body, Numbers = e.ModalityEnergies.Select(v => v.EnergyNumber).ToArray() }).ToArray()
        };
    }
}
