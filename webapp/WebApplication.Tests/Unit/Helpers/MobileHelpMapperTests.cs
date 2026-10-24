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
        public void ChartIntroductionsMatchWebsiteResourcesWithoutTabInstructions()
        {
            var help = MobileHelpMapper.Chart(true, true);
            Assert.Equal(Dictionary.MainEnergyLabel, help["core"].Title);
            Assert.Equal(Dictionary.MainEnergyDescription, help["core"].HtmlBody);
            Assert.Equal(Dictionary.CharacterEnergyDescription, help["character"].HtmlBody);
            Assert.Equal(Dictionary.SurfaceEnergyDescription, help["socialExpression"].HtmlBody);
            Assert.Equal(Dictionary.DayStarDescription, help["dayStar"].HtmlBody);
            Assert.Equal(Dictionary.GenerationalEnergyDescription, help["generation"].HtmlBody);
            Assert.Equal(Dictionary.EpochEnergyDescription, help["epoch"].HtmlBody);
            Assert.Equal(Dictionary.relationships_description, help["relationships"].HtmlBody);
            Assert.Equal(Dictionary.AdultChildRelationship, help["adultChildRelationship"].HtmlBody);
            Assert.Equal(9, help.Count);
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
