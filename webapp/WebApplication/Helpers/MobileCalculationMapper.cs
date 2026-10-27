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
        public static object CalendarLabels() => new
        {
            Title = K9.Globalisation.Dictionary.PersonalCalendar,
            PreviousMonth = K9.Globalisation.Dictionary.PreviousMonth,
            NextMonth = K9.Globalisation.Dictionary.NextMonth,
            Today = K9.Globalisation.Dictionary.Today,
            Help = K9.Globalisation.Dictionary.CalendarHelp,
            GoogleCalendarLink = K9.Globalisation.Dictionary.GoogleCalendarLink,
            CopyCalendarLink = K9.Globalisation.Dictionary.CopyCalendarLink,
            DisableCalendarLink = K9.Globalisation.Dictionary.DisableCalendarLink,
            Copied = K9.Globalisation.Dictionary.Copied,
            CopyError = K9.Globalisation.Dictionary.FailedToCopyToClipboard,
            Expand = K9.Globalisation.Dictionary.ExpandCalendar,
            Close = K9.Globalisation.Dictionary.CloseCalendar,
            Revoked = K9.Globalisation.Dictionary.CalendarLinkDisabled,
            Error = K9.Globalisation.Dictionary.NetworkError,
            Unavailable = K9.Globalisation.Dictionary.PersonalCalendarUnavailable,
            MyAccount = K9.Base.Globalisation.Dictionary.MyAccountTitle,
            Year = K9.Globalisation.Dictionary.Year,
            Month = K9.Globalisation.Dictionary.Month,
            Day = K9.Globalisation.Dictionary.Day,
            Weekdays = new[]
            {
                K9.Globalisation.Dictionary.Monday, K9.Globalisation.Dictionary.Tuesday,
                K9.Globalisation.Dictionary.Wednesday, K9.Globalisation.Dictionary.Thursday,
                K9.Globalisation.Dictionary.Friday, K9.Globalisation.Dictionary.Saturday,
                K9.Globalisation.Dictionary.Sunday
            }.Select((label, index) => new
            {
                DayOfWeek = index + 1, Label = label,
                ShortLabel = label.Substring(0, System.Math.Min(3, label.Length))
            }).ToArray()
        };

        private static string CalendarEnergyName(int number) =>
            number.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " +
            new NineStarKiEnergy((ENineStarKiEnergy)number).EnergyName;

        private static string CalendarArtworkId(int number) => MobileArtworkRegistry.IdForPath(
            "ninestar/energies/" + new NineStarKiEnergy((ENineStarKiEnergy)number).EnergyLowerCase + ".png");

        public static object CalendarDay(CalendarEntry entry) => new
        {
            Date = entry.Date.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            DateLabel = entry.Date.ToString("D", System.Globalization.CultureInfo.CurrentCulture),
            Day = entry.Date.Day,
            entry.YearHouse, entry.MonthHouse, entry.DayHouse, entry.AfternoonDayHouse,
            entry.Summary, entry.Description,
            Houses = string.Join(" \u00b7 ", new[]
            {
                K9.Globalisation.Dictionary.Year + ": " + CalendarEnergyName(entry.YearHouse),
                K9.Globalisation.Dictionary.Month + ": " + CalendarEnergyName(entry.MonthHouse),
                K9.Globalisation.Dictionary.Day + ": " + CalendarEnergyName(entry.DayHouse)
            }),
            EnergyName = CalendarEnergyName(entry.DayHouse),
            YearHouseLabel = CalendarEnergyName(entry.YearHouse),
            MonthHouseLabel = CalendarEnergyName(entry.MonthHouse),
            DayHouseLabel = CalendarEnergyName(entry.DayHouse),
            AfternoonDayHouseLabel = entry.AfternoonDayHouse.HasValue ? CalendarEnergyName(entry.AfternoonDayHouse.Value) : null,
            ArtworkId = CalendarArtworkId(entry.DayHouse),
            YearArtworkId = CalendarArtworkId(entry.YearHouse),
            MonthArtworkId = CalendarArtworkId(entry.MonthHouse),
            AfternoonArtworkId = entry.AfternoonDayHouse.HasValue ? CalendarArtworkId(entry.AfternoonDayHouse.Value) : null
        };

        public static object Moon(MoonPhase moon) => moon == null ? null : new
        {
            moon.IlluminationPercentage, moon.IsWaxing, moon.IlluminationDisplay,
            Name = moon.GetMoonPhaseName(), Image = moon.GetMoonPhaseImageFile(),
            ArtworkId = MobileArtworkRegistry.IdForPath("astrology/moon-phases/" + moon.GetMoonPhaseImageFile()),
            moon.LunarDay, moon.LunarDayTitle, moon.LunarDayDescription, moon.YinYangComfortDescription
        };

        public static object Cycle(NineStarKiEnergy energy, bool global = false) => energy == null ? null : new
        {
            HouseName = energy.HouseName,
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
            YourPredictions = K9.Globalisation.Dictionary.YourPredictions,
            HousesYouCurrentlyOccupy = K9.Globalisation.Dictionary.HousesYouCurrentlyOccupy,
            Periods = new
            {
                Year = K9.Globalisation.Dictionary.Year, Month = K9.Globalisation.Dictionary.Month,
                Day = K9.Globalisation.Dictionary.Day, Generation = K9.Globalisation.Dictionary.Generation,
                Epoch = K9.Globalisation.Dictionary.Epoch, Hour = K9.Globalisation.Dictionary.Hour
            },
            PredictionTitles = new
            {
                Year = K9.Globalisation.Dictionary.YearlyPrediction,
                Month = K9.Globalisation.Dictionary.MonthlyPrediction,
                Day = K9.Globalisation.Dictionary.DailyPrediction,
                Generation = K9.Globalisation.Dictionary.NineYearlyPrediction,
                Epoch = K9.Globalisation.Dictionary.EightyOneYearlyPrediction,
                Hour = K9.Globalisation.Dictionary.HourlyPrediction
            },
            CycleTitles = new
            {
                Year = K9.Globalisation.Dictionary.YearlyCycle,
                Month = K9.Globalisation.Dictionary.MonthlyCycle,
                Day = K9.Globalisation.Dictionary.DailyCycleEnergy,
                Generation = K9.Globalisation.Dictionary.NineYearlyEnergy,
                Epoch = K9.Globalisation.Dictionary.EightyOneYearlyEnergy,
                Hour = K9.Globalisation.Dictionary.HourlyEnergy
            },
            Groups = new
            {
                Primary = K9.Globalisation.Dictionary.PrimaryCycles,
                Additional = K9.Globalisation.Dictionary.ExtendedCycles,
                Global = K9.Globalisation.Dictionary.GlobalCycles,
                ExtendedGlobal = K9.Globalisation.Dictionary.ExtendedGlobalCycles
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

        public static string CompatibilitySubtitle(int firstEnergy, int secondEnergy)
        {
            if (firstEnergy < 1 || firstEnergy > 9 || secondEnergy < 1 || secondEnergy > 9) return null;
            var pair = System.Math.Min(firstEnergy, secondEnergy) * 10 + System.Math.Max(firstEnergy, secondEnergy);
            switch (pair)
            {
                case 11: return K9.Globalisation.Dictionary.CompatibilitySubtitle11;
                case 12: return K9.Globalisation.Dictionary.CompatibilitySubtitle12;
                case 13: return K9.Globalisation.Dictionary.CompatibilitySubtitle13;
                case 14: return K9.Globalisation.Dictionary.CompatibilitySubtitle14;
                case 15: return K9.Globalisation.Dictionary.CompatibilitySubtitle15;
                case 16: return K9.Globalisation.Dictionary.CompatibilitySubtitle16;
                case 17: return K9.Globalisation.Dictionary.CompatibilitySubtitle17;
                case 18: return K9.Globalisation.Dictionary.CompatibilitySubtitle18;
                case 19: return K9.Globalisation.Dictionary.CompatibilitySubtitle19;
                case 22: return K9.Globalisation.Dictionary.CompatibilitySubtitle22;
                case 23: return K9.Globalisation.Dictionary.CompatibilitySubtitle23;
                case 24: return K9.Globalisation.Dictionary.CompatibilitySubtitle24;
                case 25: return K9.Globalisation.Dictionary.CompatibilitySubtitle25;
                case 26: return K9.Globalisation.Dictionary.CompatibilitySubtitle26;
                case 27: return K9.Globalisation.Dictionary.CompatibilitySubtitle27;
                case 28: return K9.Globalisation.Dictionary.CompatibilitySubtitle28;
                case 29: return K9.Globalisation.Dictionary.CompatibilitySubtitle29;
                case 33: return K9.Globalisation.Dictionary.CompatibilitySubtitle33;
                case 34: return K9.Globalisation.Dictionary.CompatibilitySubtitle34;
                case 35: return K9.Globalisation.Dictionary.CompatibilitySubtitle35;
                case 36: return K9.Globalisation.Dictionary.CompatibilitySubtitle36;
                case 37: return K9.Globalisation.Dictionary.CompatibilitySubtitle37;
                case 38: return K9.Globalisation.Dictionary.CompatibilitySubtitle38;
                case 39: return K9.Globalisation.Dictionary.CompatibilitySubtitle39;
                case 44: return K9.Globalisation.Dictionary.CompatibilitySubtitle44;
                case 45: return K9.Globalisation.Dictionary.CompatibilitySubtitle45;
                case 46: return K9.Globalisation.Dictionary.CompatibilitySubtitle46;
                case 47: return K9.Globalisation.Dictionary.CompatibilitySubtitle47;
                case 48: return K9.Globalisation.Dictionary.CompatibilitySubtitle48;
                case 49: return K9.Globalisation.Dictionary.CompatibilitySubtitle49;
                case 55: return K9.Globalisation.Dictionary.CompatibilitySubtitle55;
                case 56: return K9.Globalisation.Dictionary.CompatibilitySubtitle56;
                case 57: return K9.Globalisation.Dictionary.CompatibilitySubtitle57;
                case 58: return K9.Globalisation.Dictionary.CompatibilitySubtitle58;
                case 59: return K9.Globalisation.Dictionary.CompatibilitySubtitle59;
                case 66: return K9.Globalisation.Dictionary.CompatibilitySubtitle66;
                case 67: return K9.Globalisation.Dictionary.CompatibilitySubtitle67;
                case 68: return K9.Globalisation.Dictionary.CompatibilitySubtitle68;
                case 69: return K9.Globalisation.Dictionary.CompatibilitySubtitle69;
                case 77: return K9.Globalisation.Dictionary.CompatibilitySubtitle77;
                case 78: return K9.Globalisation.Dictionary.CompatibilitySubtitle78;
                case 79: return K9.Globalisation.Dictionary.CompatibilitySubtitle79;
                case 88: return K9.Globalisation.Dictionary.CompatibilitySubtitle88;
                case 89: return K9.Globalisation.Dictionary.CompatibilitySubtitle89;
                case 99: return K9.Globalisation.Dictionary.CompatibilitySubtitle99;
                default: return null;
            }
        }

        private static object CompatibilitySectionLabel(string title, string navigationTitle) => new
        {
            Title = title, NavigationTitle = navigationTitle
        };

        public static object CompatibilityLabels(bool hideSexualChemistry)
        {
            var scores = new Dictionary<string, string>
            {
                { "complementarityScore", K9.Globalisation.Dictionary.ComplementarityScore },
                { "conflictScore", K9.Globalisation.Dictionary.ConflictScore },
                { "harmonyScore", K9.Globalisation.Dictionary.HarmonyScore },
                { "sparkScore", K9.Globalisation.Dictionary.SparkScore },
                { "learningPotentialScore", K9.Globalisation.Dictionary.LearningPotentialScore },
                { "mutualUnderstandingScore", K9.Globalisation.Dictionary.MutualUnderstanding },
                { "supportScore", K9.Globalisation.Dictionary.MutualSupport }
            };
            if (!hideSexualChemistry)
                scores.Add("sexualChemistryScore", K9.Globalisation.Dictionary.SexualChemistry);
            var sections = new Dictionary<string, object>
            {
                { "overview", CompatibilitySectionLabel(K9.Globalisation.Dictionary.CompatibilityOverview, K9.Globalisation.Dictionary.OverviewLabel) },
                { "strengthsAndChallenges", CompatibilitySectionLabel(K9.Globalisation.Dictionary.StrengthsWeaknesses, K9.Globalisation.Dictionary.GiftsTrials) },
                { "inDepth", CompatibilitySectionLabel(K9.Globalisation.Dictionary.InDepthAnalysis, K9.Globalisation.Dictionary.InDepthAnalysis) },
                { "solarElements", CompatibilitySectionLabel(K9.Globalisation.Dictionary.FundamentalElementsAnalysis, K9.Globalisation.Dictionary.SolarElements) },
                { "lunarElements", CompatibilitySectionLabel(K9.Globalisation.Dictionary.CharacterElementsAnalysis, K9.Globalisation.Dictionary.LunarElements) },
                { "allElements", CompatibilitySectionLabel(K9.Globalisation.Dictionary.AllElementsBreakdown, K9.Globalisation.Dictionary.AllElements) },
                { "yinYang", CompatibilitySectionLabel(K9.Globalisation.Dictionary.YinAndYangAnalysis, K9.Globalisation.Dictionary.YinYangLabel) },
                { "birthMonth", CompatibilitySectionLabel(K9.Globalisation.Dictionary.BirthMonthAnalysis, K9.Globalisation.Dictionary.BirthMonth) }
            };
            if (!hideSexualChemistry)
                sections.Add("sexualChemistry", CompatibilitySectionLabel(K9.Globalisation.Dictionary.SexualChemistryDetails, K9.Globalisation.Dictionary.SexualChemistryDetails));
            return new
            {
                Title = K9.Globalisation.Dictionary.RelationshipCompatibilityCalculator,
                Results = K9.Globalisation.Dictionary.CompatibilityResults,
                FirstPerson = K9.Globalisation.Dictionary.FirstPerson,
                SecondPerson = K9.Globalisation.Dictionary.SecondPerson,
                Sections = sections, Scores = scores
            };
        }

        public static string CompatibilityYinYang(bool mainSame, bool characterSame,
            ENineStarKiYinYang mainYinYang, ENineStarKiYinYang characterYinYang, string knowledgeBaseUrl)
        {
            string html;
            if (mainSame && characterSame)
                html = mainYinYang == ENineStarKiYinYang.Yin
                    ? K9.Globalisation.Dictionary.both_gender_same_yin : K9.Globalisation.Dictionary.both_gender_same_yang;
            else
            {
                html = mainSame
                    ? (mainYinYang == ENineStarKiYinYang.Yin ? K9.Globalisation.Dictionary.main_gender_same_yin : K9.Globalisation.Dictionary.main_gender_same_yang)
                    : K9.Globalisation.Dictionary.main_gender_opposite;
                html += "\n" + (characterSame
                    ? (characterYinYang == ENineStarKiYinYang.Yin ? K9.Globalisation.Dictionary.character_gender_same_yin : K9.Globalisation.Dictionary.character_gender_same_yang)
                    : K9.Globalisation.Dictionary.character_gender_opposite);
            }
            return K9.SharedLibrary.Helpers.TemplateParser.Parse(html, new
            {
                SameTransformationLink = knowledgeBaseUrl + "#same-transformation",
                SupportiveTransformationLink = knowledgeBaseUrl + "#supportive-transformation",
                ChallengingTransformationLink = knowledgeBaseUrl + "#challenging-transformation"
            });
        }

        private static object CompatibilityGauge(string key, string label, int value,
            bool isInverted = false, int maxValue = (int)ECompatibilityScore.ExtremelyHigh) => new
        {
            Key = key, Label = label, Value = value, MinValue = 0,
            MaxValue = maxValue, IsInverted = isInverted
        };

        public static object[] CompatibilityGauges(CompatibilityScoreModel score, bool hideSexualChemistry)
        {
            if (score == null) return null;
            var gauges = new List<object>
            {
                CompatibilityGauge("complementarity", K9.Globalisation.Dictionary.ComplementarityScore, (int)score.ComplementarityScore),
                CompatibilityGauge("conflict", K9.Globalisation.Dictionary.ConflictScore, (int)score.ConflictScore, isInverted: true),
                CompatibilityGauge("harmony", K9.Globalisation.Dictionary.HarmonyScore, (int)score.HarmonyScore),
                CompatibilityGauge("spark", K9.Globalisation.Dictionary.SparkScore, (int)score.SparkScore),
                CompatibilityGauge("learningPotential", K9.Globalisation.Dictionary.LearningPotentialScore, (int)score.LearningPotentialScore),
                CompatibilityGauge("mutualUnderstanding", K9.Globalisation.Dictionary.MutualUnderstanding, (int)score.MutualUnderstandingScore),
                CompatibilityGauge("mutualSupport", K9.Globalisation.Dictionary.MutualSupport, (int)score.SupportScore)
            };
            if (!hideSexualChemistry)
                gauges.Insert(4, CompatibilityGauge("sexualChemistry", K9.Globalisation.Dictionary.SexualChemistry,
                    (int)score.SexualChemistryScore, maxValue: (int)ESexualChemistryScore.OffTheCharts));
            return gauges.ToArray();
        }

        public static object CompatibilityTransformation(ETransformationType transformation)
        {
            string kind;
            switch (transformation)
            {
                case ETransformationType.Supports:
                case ETransformationType.IsSupported: kind = "supportive"; break;
                case ETransformationType.Controls:
                case ETransformationType.IsControlled: kind = "challenging"; break;
                case ETransformationType.Sibling: kind = "same"; break;
                default: return null;
            }
            return new
            {
                Type = transformation.ToString(), Kind = kind,
                ArtworkId = MobileArtworkRegistry.IdForPath("ninestar/transformations/transformation-" + kind + ".png")
            };
        }

        public static object[] CompatibilityElementBreakdown(double supportive, double sibling, double challenging) => new object[]
        {
            new { Key = "supportive", Label = K9.Globalisation.Dictionary.SupportiveElements, Percentage = supportive, Value = (int)supportive, Color = "#22BB44" },
            new { Key = "sibling", Label = K9.Globalisation.Dictionary.SiblingElements, Percentage = sibling, Value = (int)sibling, Color = "#0074E4" },
            new { Key = "challenging", Label = K9.Globalisation.Dictionary.ChallengingElements, Percentage = challenging, Value = (int)challenging, Color = "red" }
        };

        private static object[] CompatibilityElementBreakdown(ElementCompatibility elements)
        {
            var fractions = elements.GetTransformationFractions();
            return CompatibilityElementBreakdown(fractions.SupportivePercentage, fractions.SiblingPercentage, fractions.ChallengingPercentage);
        }

        public static object Compatibility(CompatibilityModel model, bool hideSexualChemistry, string knowledgeBaseUrl, bool includeFullProfiles = true)
        {
            var details = model.CompatibilityDetails;
            var score = details?.Score;
            return new
            {
                Labels = CompatibilityLabels(hideSexualChemistry),
                Help = MobileHelpMapper.Compatibility(),
                Gauges = CompatibilityGauges(score, hideSexualChemistry),
                FirstPerson = includeFullProfiles ? (object)MobilePersonalChartMapper.Map(model.NineStarKiModel1, false, includeHelp: false) : MobilePersonalChartMapper.Numbers(model.NineStarKiModel1),
                SecondPerson = includeFullProfiles ? (object)MobilePersonalChartMapper.Map(model.NineStarKiModel2, false, includeHelp: false) : MobilePersonalChartMapper.Numbers(model.NineStarKiModel2),
                Sections = new
                {
                    StrengthsAndChallenges = new
                    {
                        Title = K9.Globalisation.Dictionary.StrengthsWeaknesses,
                        Subtitle = CompatibilitySubtitle(model.NineStarKiModel1.MainEnergy.EnergyNumber,
                            model.NineStarKiModel2.MainEnergy.EnergyNumber),
                        Body = model.FundamentalEnergiesCompatibility
                    }
                },
                model.FundamentalEnergiesCompatibility,
                SexualChemistryDetails = hideSexualChemistry ? null : model.SexualChemistryDetails,
                Details = details == null ? null : new
                {
                    Elements = details.ElementCompatibility == null ? null : new
                    {
                        FundamentalTransformation = CompatibilityTransformation(details.ElementCompatibility.FundamentalEnergiesTransformationType),
                        CharacterTransformation = CompatibilityTransformation(details.ElementCompatibility.CharacterEnergiesTransformationType),
                        Breakdown = CompatibilityElementBreakdown(details.ElementCompatibility),
                        details.ElementCompatibility.FundamentalElementsTransformationDetails,
                        details.ElementCompatibility.CharacterElementsTransformationDetails,
                        details.ElementCompatibility.FundamentalElementsCompatibilityDetails,
                        details.ElementCompatibility.CharacterElementsCompatibilityDetails,
                        details.ElementCompatibility.AllOtherElementsCompatibility
                    },
                    Gender = details.GenderCompatibility == null ? null : new
                    {
                        details.GenderCompatibility.IsFundamtenalGenderSame, details.GenderCompatibility.IsCharacterGenderSame,
                        details.GenderCompatibility.IsBothGenderSame,
                        Description = CompatibilityYinYang(details.GenderCompatibility.IsFundamtenalGenderSame,
                            details.GenderCompatibility.IsCharacterGenderSame,
                            details.GenderCompatibility.FundamentalGenderSameYinYang,
                            details.GenderCompatibility.CharacterGenderSameYinYang, knowledgeBaseUrl)
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
