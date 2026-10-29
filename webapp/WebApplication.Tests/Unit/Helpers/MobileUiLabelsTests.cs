using K9.WebApplication.Controllers;
using K9.WebApplication.Helpers;
using K9.WebApplication.Packages;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;
using Dictionary = K9.Globalisation.Dictionary;

namespace K9.WebApplication.Tests.Unit.Helpers
{
    public class MobileUiLabelsTests
    {
        private static JObject Catalogue() => JObject.FromObject(MobileUiLabels.Map(),
            JsonSerializer.Create(new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));

        [Fact]
        public void AnonymousCatalogueNeedsNoIdentityDatabaseOrCalculation()
        {
            var package = new Mock<INineStarKiPackage>(MockBehavior.Strict);
            var controller = new MobileMetadataController(package.Object, null);
            var cache = new Mock<HttpCachePolicyBase>();
            var response = new Mock<HttpResponseBase>();
            response.SetupGet(value => value.Cache).Returns(cache.Object);
            var context = new Mock<HttpContextBase>();
            context.SetupGet(value => value.Response).Returns(response.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            var result = Assert.IsType<ContentResult>(controller.Labels());
            var json = JObject.Parse(result.Content);
            Assert.True((bool)json["success"]);
            Assert.Equal(1, (int)json["data"]["version"]);
            Assert.Equal(Dictionary.DateOfBirthLabel, (string)json["data"]["shared"]["calculator"]["fields"]["dateOfBirth"]);
            cache.Verify(value => value.SetNoStore(), Times.Once);
        }

        [Fact]
        public void DateChoicesAndTimezoneHelpUseTheApprovedResources()
        {
            var data = Catalogue();
            var calculator = data["shared"]["calculator"];
            Assert.Equal(Dictionary.CurrentTimeZone, (string)calculator["fields"]["currentTimeZone"]);
            Assert.Equal(Dictionary.CurrentTimeZoneHelp, (string)calculator["help"]["currentTimeZone"]);
            Assert.Equal(new[] { "Now", "SelectedDate" }, calculator["displayDataForPeriods"].Select(option => (string)option["value"]).ToArray());
            Assert.Equal(new[] { Dictionary.Now, Dictionary.SelectedDateAndTime }, calculator["displayDataForPeriods"].Select(option => (string)option["label"]).ToArray());
            Assert.Equal(Dictionary.InvertDailyAndHourlyCycleKiForSouthernHemisphereLabel, (string)calculator["fields"]["invertCycleKi"]);
        }

        [Fact]
        public void ScreenHeadingsStaySeparateFromHelpAndPersonalisedReadings()
        {
            var data = Catalogue();
            Assert.Equal(Dictionary.Trigram, (string)data["screens"]["personalChart"]["facts"]["trigram"]);
            Assert.Equal(Dictionary.Essence, (string)data["screens"]["personalChart"]["menu"]["essence"]);
            Assert.Equal(Dictionary.MainHexagram, (string)data["screens"]["iching"]["mainHexagram"]);
            Assert.Equal(Dictionary.TransformedHexagram, (string)data["screens"]["iching"]["transformedHexagram"]);
            Assert.Equal(Dictionary.YourPersonalChart, (string)data["screens"]["personalChart"]["titles"]["page"]);
            Assert.Null(data["screens"]["personalChart"]["energies"]);
            Assert.Null(data["screens"]["compatibility"]["gauges"]);
            Assert.DoesNotContain(data.Descendants().OfType<JProperty>(), property => property.Name == "htmlBody" || property.Name == "entitlements");
        }

        [Fact]
        public void CatalogueHasNonEmptyStringsAndReportsTheResourceCulture()
        {
            var data = Catalogue();
            Assert.Equal((Dictionary.Culture ?? CultureInfo.CurrentUICulture).Name, (string)data["culture"]);
            Assert.All(data.Descendants().OfType<JValue>().Where(value => value.Type == JTokenType.String),
                value => Assert.False(string.IsNullOrWhiteSpace((string)value), value.Path));
        }
    }
}
