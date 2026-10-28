using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using Newtonsoft.Json.Linq;
using System.Linq;
using Xunit;
using Dictionary = K9.Globalisation.Dictionary;

namespace K9.WebApplication.Tests.Unit.Helpers
{
    public class MobileTravelMapperTests
    {
        [Fact]
        public void DeniedDetailsDoNotTouchTheCalculatedModel()
        {
            Assert.Null(MobileTravelMapper.Summary(null, false));
            Assert.Null(MobileTravelMapper.MagicSquares(null, false));
        }

        [Theory]
        [InlineData(ENineStarKiDirection.North, 0.5, 1, "South")]
        [InlineData(ENineStarKiDirection.South, 0.5, 0, "North")]
        [InlineData(ENineStarKiDirection.East, 0, 0.5, "West")]
        [InlineData(ENineStarKiDirection.West, 1, 0.5, "East")]
        [InlineData(ENineStarKiDirection.NorthEast, 0, 1, "SouthWest")]
        [InlineData(ENineStarKiDirection.SouthWest, 1, 0, "NorthEast")]
        [InlineData(ENineStarKiDirection.Centre, 0.5, 0.5, "Centre")]
        public void ArrowEndpointsFollowTheWebsiteSouthUpOrientation(ENineStarKiDirection direction, double x, double y, string display)
        {
            var result = JObject.FromObject(MobileTravelMapper.Direction(direction));
            Assert.Equal(x, (double)result["EndX"]);
            Assert.Equal(y, (double)result["EndY"]);
            Assert.Equal(display, (string)result["DisplayDirection"]);
            Assert.Equal(NineStarKiDirection.GetDirectionLabel(direction), (string)result["Label"]);
            Assert.Equal(direction == ENineStarKiDirection.Centre, (bool)result["IsCentre"]);
        }

        [Fact]
        public void AllEightyOneSquaresPreserveWebsiteLayoutAndHouseHighlight()
        {
            var houseOrder = new[] { 4, 9, 2, 3, 5, 7, 8, 1, 6 };
            for (var global = 1; global <= 9; global++)
            for (var personal = 1; personal <= 9; personal++)
            {
                var square = CreateSquare(global, personal);
                var result = JObject.FromObject(MobileTravelMapper.Square(square));
                var cells = (JArray)result["Cells"];
                Assert.Equal(houseOrder, cells.Select(cell => (int)cell["House"]).ToArray());
                Assert.Equal(square.GlobalKi.GetMagicSquareNumbers().OrderBy(n => n), cells.Select(cell => (int)cell["Number"]).OrderBy(n => n));
                var active = Assert.Single(cells.Where(cell => (bool)cell["IsActive"]));
                Assert.Equal(personal, (int)active["House"]);
                Assert.Equal("ninestar.magic-square-plate", (string)result["ArtworkId"]);
                Assert.Equal(4, result["Arrows"].Count());
                Assert.DoesNotContain("{Direction}", result["Arrows"].ToString());
            }
        }

        [Fact]
        public void ExampleSquareMatchesTheWebsiteAndGlobalAndLongCyclesHideArrows()
        {
            var square = CreateSquare(2, 3);
            var result = JObject.FromObject(MobileTravelMapper.Square(square));
            Assert.Equal(new[] { 1, 6, 8, 9, 2, 4, 5, 7, 3 }, result["Cells"].Select(cell => (int)cell["Number"]).ToArray());
            Assert.Equal(Dictionary.YearlyEnergyLabel, (string)result["Title"]);
            square.IsGlobal = true;
            result = JObject.FromObject(MobileTravelMapper.Square(square));
            Assert.False((bool)result["ShowDirections"]);
            Assert.Equal(JTokenType.Null, result["Arrows"].Type);
            Assert.Equal(5, (int)Assert.Single(result["Cells"].Where(cell => (bool)cell["IsActive"]))["House"]);
            square.IsGlobal = false;
            square.GlobalKi.EnergyCycleType = ENineStarKiEnergyCycleType.GenerationalEnergy;
            Assert.False((bool)JObject.FromObject(MobileTravelMapper.Square(square))["ShowDirections"]);
        }

        [Fact]
        public void DualHousesAndSplitDaysHaveSeparateRenderingContracts()
        {
            var square = CreateSquare(2, 3);
            square.IsDualView = true;
            square.SecondMagicSquareViewModel = CreateSquare(2, 7);
            var result = JObject.FromObject(MobileTravelMapper.Square(square));
            Assert.Equal(7, (int)Assert.Single(result["Cells"].Where(cell => (bool)cell["IsLunarActive"]))["House"]);
            Assert.Equal(JTokenType.Null, result["Secondary"].Type);
            square.IsDualView = false;
            square.IsSplit = true;
            result = JObject.FromObject(MobileTravelMapper.Square(square));
            Assert.Equal(2, (int)result["Secondary"]["GlobalNumber"]);
        }

        [Fact]
        public void SummaryReusesWebsiteGroupingGuidanceAndWarningThreshold()
        {
            var year = CreateSquare(2, 3);
            var month = CreateSquare(5, 5);
            month.GlobalKi.EnergyCycleType = ENineStarKiEnergyCycleType.MonthlyCycleEnergy;
            month.PersonalHouseOccupied.EnergyCycleType = ENineStarKiEnergyCycleType.MonthlyCycleEnergy;
            var chart = new NineStarKiDrectionsChartViewModel(new System.Collections.Generic.List<(
                NineStarKiEnergy PersonalHouseOccupied, NineStarKiEnergy GlobalKi,
                ENineStarKiEnergyCycleType CycleType, System.Collections.Generic.List<NineStarKiDirection> Directions)>
            {
                (year.PersonalHouseOccupied, year.GlobalKi, ENineStarKiEnergyCycleType.YearlyCycleEnergy, year.GetDirections().UnfavourableDirections),
                (month.PersonalHouseOccupied, month.GlobalKi, ENineStarKiEnergyCycleType.MonthlyCycleEnergy, month.GetDirections().UnfavourableDirections)
            });
            // Avoid constructor session/astronomy work: only supply the website's calculated chart.
            var model = (NineStarKiModel)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(NineStarKiModel));
            typeof(NineStarKiModel).GetField("_directionsChartViewModel",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(model, chart);
            var expected = chart.GetDirectionsChartData();
            var result = JObject.FromObject(MobileTravelMapper.Summary(model, true));
            Assert.Equal(Dictionary.WorstDirectionsToTravelIn, (string)result["Title"]);
            Assert.Equal(expected.Count, result["Rows"].Count());
            for (var index = 0; index < expected.Count; index++)
            {
                var row = result["Rows"][index];
                Assert.Equal(expected[index].Score, (int)row["Score"]);
                Assert.Equal(expected[index].Guidance, (string)row["Guidance"]);
                Assert.Equal(expected[index].CssClassName, (string)row["Severity"]);
                Assert.Equal(expected[index].Score >= 10, (bool)row["ShowWarning"]);
                Assert.Equal(expected[index].Directions.Count, row["Directions"].Count());
            }
        }

        private static MagicSquareViewModel CreateSquare(int global, int personal) => new MagicSquareViewModel
        {
            GlobalKi = new NineStarKiEnergy((ENineStarKiEnergy)global, ENineStarKiEnergyCycleType.YearlyCycleEnergy),
            PersonalHouseOccupied = new NineStarKiEnergy((ENineStarKiEnergy)personal, ENineStarKiEnergyCycleType.YearlyCycleEnergy),
            PersonalChartEnergy = new NineStarKiEnergy(ENineStarKiEnergy.Thunder)
        };
    }
}
