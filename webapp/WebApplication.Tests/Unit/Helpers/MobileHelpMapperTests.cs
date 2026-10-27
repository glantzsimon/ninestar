using K9.WebApplication.Helpers;
using K9.WebApplication.ViewModels.MobileApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Globalization;
using Xunit;
using Dictionary = K9.Globalisation.Dictionary;

namespace K9.WebApplication.Tests.Unit.Helpers
{
    public class MobileHelpMapperTests
    {
        [Fact]
        public void PredictionLabelsUseShortDictionaryResourcesWithoutReadingContent()
        {
            var labels = JObject.FromObject(MobileCalculationMapper.PredictionLabels(),
                JsonSerializer.Create(new JsonSerializerSettings
                { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            Assert.Equal(Dictionary.YourPredictions, (string)labels["yourPredictions"]);
            Assert.Equal(Dictionary.HousesYouCurrentlyOccupy, (string)labels["housesYouCurrentlyOccupy"]);
            Assert.Equal(Dictionary.Year, (string)labels["periods"]["year"]);
            Assert.Equal(Dictionary.Month, (string)labels["periods"]["month"]);
            Assert.Equal(Dictionary.Day, (string)labels["periods"]["day"]);
            Assert.Equal(Dictionary.Generation, (string)labels["periods"]["generation"]);
            Assert.Equal(Dictionary.Epoch, (string)labels["periods"]["epoch"]);
            Assert.Equal(Dictionary.Hour, (string)labels["periods"]["hour"]);
            Assert.Equal(Dictionary.PersonalView, (string)labels["scopes"]["personal"]);
            Assert.Equal(Dictionary.PersonalLunarView, (string)labels["scopes"]["lunar"]);
            Assert.Equal(Dictionary.GlobalView, (string)labels["scopes"]["global"]);
            Assert.Equal(Dictionary.YearlyCycle, (string)labels["cycleTitles"]["year"]);
            Assert.Equal(Dictionary.MonthlyCycle, (string)labels["cycleTitles"]["month"]);
            Assert.Equal(Dictionary.DailyCycleEnergy, (string)labels["cycleTitles"]["day"]);
            Assert.Equal(Dictionary.NineYearlyEnergy, (string)labels["cycleTitles"]["generation"]);
            Assert.Equal(Dictionary.EightyOneYearlyEnergy, (string)labels["cycleTitles"]["epoch"]);
            Assert.Equal(Dictionary.HourlyEnergy, (string)labels["cycleTitles"]["hour"]);
            Assert.Equal(Dictionary.PrimaryCycles, (string)labels["groups"]["primary"]);
            Assert.Equal(Dictionary.ExtendedCycles, (string)labels["groups"]["additional"]);
            Assert.Equal(Dictionary.YearlyPrediction, (string)labels["predictionTitles"]["year"]);
            Assert.Equal(Dictionary.MonthlyPrediction, (string)labels["predictionTitles"]["month"]);
            Assert.Equal(Dictionary.DailyPrediction, (string)labels["predictionTitles"]["day"]);
            Assert.Equal(Dictionary.NineYearlyPrediction, (string)labels["predictionTitles"]["generation"]);
            Assert.Equal(Dictionary.EightyOneYearlyPrediction, (string)labels["predictionTitles"]["epoch"]);
            Assert.Equal(Dictionary.HourlyPrediction, (string)labels["predictionTitles"]["hour"]);
            Assert.Equal(6, ((JObject)labels["predictionTitles"]).Count);
            Assert.Equal(6, ((JObject)labels["periods"]).Count);
            Assert.Equal(3, ((JObject)labels["scopes"]).Count);
        }

        [Fact]
        public void CalendarPresentationPreservesWebsiteLabelsAndRegisteredArtwork()
        {
            var labels = JObject.FromObject(MobileCalculationMapper.CalendarLabels());
            Assert.Equal(Dictionary.Today, (string)labels["Today"]);
            Assert.Equal(Dictionary.Monday, (string)labels["Weekdays"][0]["Label"]);
            Assert.Equal(Dictionary.Sunday, (string)labels["Weekdays"][6]["Label"]);
            Assert.Equal(7, labels["Weekdays"].Count());
            var entry = new K9.WebApplication.Models.CalendarEntry
            {
                Date = new System.DateTime(2026, 10, 1), YearHouse = 7, MonthHouse = 7,
                DayHouse = 7, AfternoonDayHouse = 4, Summary = "7.7.7", Description = "existing reading"
            };
            var day = JObject.FromObject(MobileCalculationMapper.CalendarDay(entry));
            Assert.Equal("2026-10-01", (string)day["Date"]);
            Assert.Equal(entry.Date.ToString("D", CultureInfo.CurrentCulture), (string)day["DateLabel"]);
            Assert.Equal(entry.Description, (string)day["Description"]);
            Assert.Equal(MobileArtworkRegistry.IdForPath("ninestar/energies/lake.png"), (string)day["ArtworkId"]);
            Assert.False(string.IsNullOrEmpty((string)day["ArtworkId"]));
            Assert.Equal(MobileArtworkRegistry.IdForPath("ninestar/energies/wind.png"), (string)day["AfternoonArtworkId"]);
            entry.AfternoonDayHouse = null;
            day = JObject.FromObject(MobileCalculationMapper.CalendarDay(entry));
            Assert.Equal(JTokenType.Null, day["AfternoonArtworkId"].Type);
            Assert.Equal(JTokenType.Null, day["AfternoonDayHouseLabel"].Type);
        }

        [Fact]
        public void CompatibilityGaugesMatchWebsiteScalesLabelsAndPrivacy()
        {
            var score = new K9.WebApplication.Models.CompatibilityScoreModel
            {
                ConflictScore = K9.WebApplication.Enums.ECompatibilityScore.High,
                HarmonyScore = K9.WebApplication.Enums.ECompatibilityScore.Low,
                SexualChemistryScore = K9.WebApplication.Enums.ESexualChemistryScore.ExtremelyHigh
            };
            var gauges = JArray.FromObject(MobileCalculationMapper.CompatibilityGauges(score, false));
            Assert.Equal(8, gauges.Count);
            Assert.Equal(Dictionary.ComplementarityScore, (string)gauges[0]["Label"]);
            Assert.Equal(Dictionary.ConflictScore, (string)gauges[1]["Label"]);
            Assert.Equal(7, (int)gauges[1]["Value"]);
            Assert.True((bool)gauges[1]["IsInverted"]);
            Assert.Equal(Dictionary.HarmonyScore, (string)gauges[2]["Label"]);
            Assert.Equal(3, (int)gauges[2]["Value"]);
            Assert.Equal(Dictionary.SparkScore, (string)gauges[3]["Label"]);
            Assert.Equal(Dictionary.SexualChemistry, (string)gauges[4]["Label"]);
            Assert.Equal(14, (int)gauges[4]["Value"]);
            Assert.Equal(15, (int)gauges[4]["MaxValue"]);
            Assert.Equal(Dictionary.LearningPotentialScore, (string)gauges[5]["Label"]);
            Assert.Equal(Dictionary.MutualUnderstanding, (string)gauges[6]["Label"]);
            Assert.Equal(Dictionary.MutualSupport, (string)gauges[7]["Label"]);
            Assert.All(gauges.Where(g => (string)g["Key"] != "sexualChemistry"), g => Assert.Equal(9, (int)g["MaxValue"]));
            Assert.All(gauges, g => Assert.Equal(0, (int)g["MinValue"]));
            Assert.All(gauges.Where(g => (string)g["Key"] != "conflict"), g => Assert.False((bool)g["IsInverted"]));
            var hidden = JArray.FromObject(MobileCalculationMapper.CompatibilityGauges(score, true));
            Assert.Equal(7, hidden.Count);
            Assert.DoesNotContain(hidden, g => (string)g["Key"] == "sexualChemistry");
            Assert.Null(MobileCalculationMapper.CompatibilityGauges(null, false));
        }

        [Fact]
        public void CompatibilitySectionsKeepShortNavigationSeparateAndHideChemistry()
        {
            var labels = JObject.FromObject(MobileCalculationMapper.CompatibilityLabels(false));
            Assert.Equal(Dictionary.FirstPerson, (string)labels["FirstPerson"]);
            Assert.Equal(Dictionary.ConflictScore, (string)labels["Scores"]["conflictScore"]);
            Assert.Equal(Dictionary.SecondPerson, (string)labels["SecondPerson"]);
            Assert.Equal(Dictionary.FundamentalElementsAnalysis, (string)labels["Sections"]["solarElements"]["Title"]);
            Assert.Equal(Dictionary.SolarElements, (string)labels["Sections"]["solarElements"]["NavigationTitle"]);
            Assert.Equal(Dictionary.YinAndYangAnalysis, (string)labels["Sections"]["yinYang"]["Title"]);
            Assert.Equal(Dictionary.YinYangLabel, (string)labels["Sections"]["yinYang"]["NavigationTitle"]);
            Assert.NotNull(labels["Sections"]["sexualChemistry"]);
            var hidden = JObject.FromObject(MobileCalculationMapper.CompatibilityLabels(true));
            Assert.Null(hidden["Sections"]["sexualChemistry"]);
            Assert.Null(hidden["Scores"]["sexualChemistryScore"]);
        }

        [Fact]
        public void CompatibilityYinYangUsesWebsiteResourcesAndResolvesLinks()
        {
            const string url = "https://example.test/learn";
            var yin = K9.WebApplication.Models.ENineStarKiYinYang.Yin;
            var yang = K9.WebApplication.Models.ENineStarKiYinYang.Yang;
            var bothYin = MobileCalculationMapper.CompatibilityYinYang(true, true, yin, yin, url);
            Assert.Equal(Dictionary.both_gender_same_yin, bothYin);
            var bothYang = MobileCalculationMapper.CompatibilityYinYang(true, true, yang, yang, url);
            Assert.Equal(Dictionary.both_gender_same_yang, bothYang);
            var opposite = MobileCalculationMapper.CompatibilityYinYang(false, false, yin, yang, url);
            Assert.Contains(url + "#challenging-transformation", opposite);
            Assert.DoesNotContain("{ChallengingTransformationLink}", opposite);
            Assert.DoesNotContain("{SameTransformationLink}", opposite);
            Assert.DoesNotContain("{SupportiveTransformationLink}", opposite);
            Assert.Contains("\n", opposite);
        }

        [Fact]
        public void CompatibilitySubtitlesCoverEveryPairAndRemainSymmetric()
        {
            for (var first = 1; first <= 9; first++)
                for (var second = 1; second <= 9; second++)
                {
                    var subtitle = MobileCalculationMapper.CompatibilitySubtitle(first, second);
                    Assert.False(string.IsNullOrWhiteSpace(subtitle));
                    Assert.Equal(subtitle, MobileCalculationMapper.CompatibilitySubtitle(second, first));
                    Assert.DoesNotContain("<", subtitle);
                }
            Assert.Equal(Dictionary.CompatibilitySubtitle12, MobileCalculationMapper.CompatibilitySubtitle(1, 2));
            Assert.Equal(Dictionary.CompatibilitySubtitle26, MobileCalculationMapper.CompatibilitySubtitle(6, 2));
            Assert.Null(MobileCalculationMapper.CompatibilitySubtitle(0, 1));
            Assert.Null(MobileCalculationMapper.CompatibilitySubtitle(1, 10));
        }

        [Fact]
        public void ChartIntroductionsMatchWebsiteResourcesWithoutTabInstructions()
        {
            var help = MobileHelpMapper.Chart(true, true);
            Assert.Equal(Dictionary.AboutConstitution, help["core"].Title);
            Assert.Equal(Dictionary.MainEnergyDescription, help["core"].HtmlBody);
            Assert.Equal(Dictionary.CharacterEnergyDescription, help["character"].HtmlBody);
            Assert.Equal(Dictionary.SurfaceEnergyDescription, help["socialExpression"].HtmlBody);
            Assert.Equal(Dictionary.DayStarDescription, help["dayStar"].HtmlBody);
            Assert.Equal(Dictionary.GenerationalEnergyDescription, help["generation"].HtmlBody);
            Assert.Equal(Dictionary.EpochEnergyDescription, help["epoch"].HtmlBody);
            Assert.Equal(Dictionary.relationships_description, help["relationships"].HtmlBody);
            Assert.Equal(Dictionary.AboutAdultChildRelationship, help["adultChildRelationship"].Title);
            Assert.Equal(Dictionary.AdultChildRelationship, help["adultChildRelationship"].HtmlBody);
            Assert.Equal(9, help.Count);
        }

        [Fact]
        public void ContextualAboutTitlesStaySeparateFromNavigationAndPreserveExceptions()
        {
            var chart = MobileHelpMapper.Chart(true, true);
            Assert.Equal(Dictionary.AboutPersonalChart, chart["overview"].Title);
            Assert.Equal(Dictionary.AboutCharacterNature, chart["character"].Title);
            Assert.Equal(Dictionary.AboutSocialExpression, chart["socialExpression"].Title);
            Assert.Equal(Dictionary.AboutDayStar, chart["dayStar"].Title);
            Assert.Equal(Dictionary.AboutGenerationalKi, chart["generation"].Title);
            Assert.Equal(Dictionary.AboutEpochKi, chart["epoch"].Title);
            Assert.Equal(Dictionary.AboutRelationshipsAndIntimacy, chart["relationships"].Title);
            var cycles = MobileHelpMapper.Predictions();
            Assert.Equal(Dictionary.AboutDailyPredictions, cycles["day"].Title);
            Assert.Equal(Dictionary.AboutMonthlyPredictions, cycles["month"].Title);
            Assert.Equal(Dictionary.AboutYearlyPredictions, cycles["year"].Title);
            Assert.Equal(Dictionary.AboutNineYearPredictions, cycles["generation"].Title);
            Assert.Equal(Dictionary.AboutEightyOneYearPredictions, cycles["epoch"].Title);
            Assert.Equal(Dictionary.AboutGlobalKi, cycles["globalKiWarning"].Title);
            Assert.Equal(Dictionary.MoreOnCycles, cycles["moreOnCycles"].Title);
            Assert.Equal(Dictionary.WhatIsKarmicNumerology, MobileHelpMapper.Numerology()["overview"].Title);
            Assert.Equal(Dictionary.AboutPersonalCalendar, MobileHelpMapper.Calendar()["overview"].Title);
            Assert.Equal(Dictionary.AboutPersonalCalendar, MobileHelpMapper.Calendar()["subscription"].Title);
            Assert.Equal(Dictionary.AboutCompatibilityCalculator, MobileHelpMapper.Compatibility()["overview"].Title);
            Assert.Equal(Dictionary.PersonalChart, MobileHelpMapper.KnowledgeBase(false, "https://example.test/learn")["personalChart"].Title);
        }

        [Theory]
        [InlineData(false, false, 7)]
        [InlineData(true, false, 8)]
        [InlineData(false, true, 8)]
        [InlineData(true, true, 9)]
        public void AgeAndSectionGrantsFilterChartHelp(bool relationships, bool member, int count)
        {
            var help = MobileHelpMapper.Chart(relationships, member);
            Assert.Equal(count, help.Count);
            Assert.Equal(relationships, help.ContainsKey("relationships"));
            Assert.Equal(member, help.ContainsKey("adultChildRelationship"));
            Assert.DoesNotContain("hour", help.Keys); // No invented website explanation.
        }

        [Fact]
        public void PredictionsKeepGlobalWarningSeparateFromGenericEpochText()
        {
            var help = MobileHelpMapper.Predictions();
            var introductions = MobileHelpMapper.PredictionIntroductions();
            Assert.Equal(2, introductions.Count);
            Assert.Equal(Dictionary.PredictionsOverview, introductions["overview"].Title);
            Assert.Equal(Dictionary.PredictionsIntroduction, introductions["overview"].HtmlBody);
            Assert.Equal(Dictionary.AboutGlobalKi, introductions["globalKi"].Title);
            Assert.Equal(Dictionary.GlobalKiIntroduction, introductions["globalKi"].HtmlBody);
            Assert.Equal(introductions["overview"].HtmlBody, help["overview"].HtmlBody);
            Assert.Equal(introductions["globalKi"].HtmlBody, help["globalKi"].HtmlBody);
            Assert.DoesNotContain("moreOnCycles", introductions.Keys);
            Assert.DoesNotContain("globalKiWarning", introductions.Keys);
            Assert.Equal(Dictionary.day_description, help["day"].HtmlBody);
            Assert.Equal(Dictionary.month_description, help["month"].HtmlBody);
            Assert.Equal(Dictionary.year_description, help["year"].HtmlBody);
            Assert.Equal(Dictionary.generation_description, help["generation"].HtmlBody);
            Assert.Equal(Dictionary.epoch_description, help["epoch"].HtmlBody);
            Assert.Equal(Dictionary.globalki_display_warning, help["globalKiWarning"].HtmlBody);
            Assert.Equal(Dictionary.holograhpic_spirals, help["moreOnCycles"].HtmlBody);
        }

        [Fact]
        public void FeatureHelpUsesTheExistingWebsiteContent()
        {
            Assert.Equal(Dictionary.PersonalCalendarHelp, MobileHelpMapper.Calendar()["overview"].HtmlBody);
            Assert.Equal(Dictionary.CalendarSubscriptionHelp, MobileHelpMapper.Calendar()["subscription"].HtmlBody);
            Assert.Equal(Dictionary.compatibility_subtitle, MobileHelpMapper.Compatibility()["overview"].HtmlBody);
            Assert.Equal(Dictionary.karmic_numerology, MobileHelpMapper.Numerology()["overview"].HtmlBody);
            Assert.Equal(Dictionary.biorhythms, MobileHelpMapper.Biorhythms()["overview"].HtmlBody);
            Assert.Equal(Dictionary.i_ching_info, MobileHelpMapper.IChing()["overview"].HtmlBody);
        }

        [Fact]
        public void AnonymousKnowledgeHelpExcludesAllNewMembershipProtectedArticles()
        {
            var help = MobileHelpMapper.KnowledgeBase(false, "https://example.test/learn");
            Assert.Equal(new[] { "characterEnergies", "lightAndDark", "mainEnergies", "modalities", "month",
                "origins", "personalChart", "relationships", "year", "yinYang" }, help.Keys.OrderBy(e => e).ToArray());
            Assert.Equal(Dictionary.main_energies_short_description, help["mainEnergies"].HtmlBody);
            Assert.Equal(Dictionary.character_energies_short_description, help["characterEnergies"].HtmlBody);
        }

        [Fact]
        public void EligibleKnowledgeArticlesResolveResourceTemplatesWithoutRenderingViews()
        {
            var help = MobileHelpMapper.KnowledgeBase(true, "https://example.test/learn");
            Assert.Equal(18, help.Count);
            Assert.Equal(Dictionary.water_element, help["water"].HtmlBody);
            Assert.DoesNotContain("{BaseImagesPath}", help["elements"].HtmlBody);
            Assert.DoesNotContain("{WaterTree}", help["transformations"].HtmlBody);
            Assert.DoesNotContain("{BaseImagesPath}", help["transformations"].HtmlBody);
            Assert.Contains("https://example.test/learn#yinyang", help["sexuality"].HtmlBody);
            Assert.Contains("https://example.test/learn#chart", help["sexuality"].HtmlBody);
            Assert.DoesNotContain("{AboutTitle}", help["sexuality"].HtmlBody);
            Assert.DoesNotContain("{ChartLink}", help["sexuality"].HtmlBody);
        }

        [Fact]
        public void ProtocolKeysAndTransportShapeAreStableAndResourceCultureIsReported()
        {
            var text = MobileHelpMapper.Chart(false, false)["core"];
            Assert.Equal((Dictionary.Culture ?? CultureInfo.CurrentUICulture).Name, text.Culture);
            var json = JObject.Parse(JsonConvert.SerializeObject(text, new JsonSerializerSettings
            { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            Assert.Equal(new[] { "culture", "htmlBody", "title" }, json.Properties().Select(e => e.Name).OrderBy(e => e).ToArray());
            Assert.Equal(Dictionary.MainEnergyDescription, (string)json["htmlBody"]);
        }

        [Fact]
        public void MutatingOneResponseDoesNotChangeAnotherResponse()
        {
            var first = MobileHelpMapper.Chart(true, true);
            first["core"].HtmlBody = "changed";
            first.Remove("character");
            var second = MobileHelpMapper.Chart(true, true);
            Assert.Equal(Dictionary.MainEnergyDescription, second["core"].HtmlBody);
            Assert.True(second.ContainsKey("character"));
        }
    }
}
