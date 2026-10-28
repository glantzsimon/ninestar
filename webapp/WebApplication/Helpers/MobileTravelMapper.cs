using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Attributes;
using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using System.Linq;

namespace K9.WebApplication.Helpers
{
    public static class MobileTravelMapper
    {
        // Website order: south at the top, north at the bottom.
        private static readonly int[] Houses = { 4, 9, 2, 3, 5, 7, 8, 1, 6 };

        public static object Direction(ENineStarKiDirection direction)
        {
            var display = direction.GetAttribute<NineStarKiDirectionAttribute>().DisplayDirection;
            var index = System.Array.IndexOf(Houses, (int)direction);
            return new
            {
                Key = direction.ToString(), Label = NineStarKiDirection.GetDirectionLabel(direction),
                DisplayDirection = display.ToString(),
                // Endpoints on a unit square, origin at top left. Centre is a ring, not an arrow.
                EndX = (index % 3) / 2.0, EndY = (index / 3) / 2.0,
                IsCentre = direction == ENineStarKiDirection.Centre
            };
        }

        public static object Summary(NineStarKiModel model, bool allowed)
        {
            if (!allowed) return null;
            return new
            {
                Periods = new[] { "year", "month", "day" },
                Title = Dictionary.WorstDirectionsToTravelIn,
                Instructions = Dictionary.DirectionsInstructions,
                Labels = new { Direction = Dictionary.Direction, Score = Dictionary.Score, Guidance = Dictionary.Guidance },
                Rows = model.GetDrectionsChartViewModel().GetDirectionsChartData().Select(row => new
                {
                    row.Score, Severity = row.CssClassName,
                    Color = SummaryColor(row.CssClassName), ShowWarning = row.Score >= 10,
                    row.Guidance,
                    Directions = row.Directions.Select(direction => Direction(direction.Direction)).ToArray()
                }).ToArray()
            };
        }

        public static object MagicSquares(NineStarKiModel model, bool allowed)
        {
            if (!allowed) return null;
            var personal = model.GetCycleMagicSquares();
            var global = model.GetGlobalCycleMagicSquares();
            return new
            {
                Personal = new
                {
                    Epoch = Square(personal.Epoch), Generation = Square(personal.Generation),
                    Year = Square(personal.Year), Month = Square(personal.Month),
                    Day = Square(personal.Day), Hour = Square(personal.Hour)
                },
                Global = new
                {
                    Epoch = Square(global.Epoch), Generation = Square(global.Generation),
                    Year = Square(global.Year), Month = Square(global.Month),
                    Day = Square(global.Day), Hour = Square(global.Hour)
                }
            };
        }

        public static object Square(MagicSquareViewModel square)
        {
            if (square?.GlobalKi == null || square.PersonalHouseOccupied == null) return null;
            var numbers = square.GlobalKi.GetMagicSquareNumbers();
            var cycle = square.GlobalKi.EnergyCycleType;
            var showDirections = !square.IsGlobal && !square.IsPersonalChart
                && cycle != ENineStarKiEnergyCycleType.EpochEnergy
                && cycle != ENineStarKiEnergyCycleType.GenerationalEnergy;
            var directions = showDirections ? square.GetDirections() : null;
            return new
            {
                Title = SquareTitle(cycle), HouseName = square.IsGlobal ? square.GlobalKi.HouseName : square.PersonalHouseOccupied.HouseName,
                GlobalNumber = square.GlobalKi.EnergyNumber, PersonalHouse = square.PersonalHouseOccupied.EnergyNumber,
                square.IsGlobal, square.IsSplit, square.IsDualView, ShowDirections = showDirections,
                ArtworkId = MobileArtworkRegistry.IdForPath("ninestar/magic-square-plate.png"),
                Cells = Houses.Select((house, index) => new
                {
                    Row = index / 3, Column = index % 3, House = house, Number = numbers[house - 1],
                    Color = NumberColor(numbers[house - 1]),
                    IsActive = square.IsGlobal || square.IsPersonalChart ? house == 5 : house == square.PersonalHouseOccupied.EnergyNumber,
                    IsLunarActive = !square.IsGlobal && !square.IsPersonalChart && square.IsDualView
                        && house == square.SecondMagicSquareViewModel?.PersonalHouseOccupied?.EnergyNumber
                }).ToArray(),
                Arrows = directions == null ? null : new[]
                {
                    Arrow("five-yellow-killing", "#ffff00", directions.FiveYelloKilling, directions.FiveYellowKillingDescription),
                    Arrow("dark-sword-killing", "#333333", directions.DarkSwordKilling, directions.DarkSwordKillingDescription),
                    Arrow("self-life-killing", "#ff0000", directions.SelfLifeKilling, directions.SelfLifeKillingDescription),
                    Arrow("target-killing", "#0000ff", directions.TargetKilling, directions.TargetKillingDescription)
                },
                // A dual square shares the grid; a split day renders a second grid.
                SecondaryHouseName = square.SecondMagicSquareViewModel?.PersonalHouseOccupied?.HouseName,
                Secondary = square.IsSplit ? Square(square.SecondMagicSquareViewModel) : null
            };
        }

        private static object Arrow(string key, string color, NineStarKiDirection direction, string description) => new
        {
            Key = key, Color = color, direction.Title, Description = description,
            direction.Score, Direction = Direction(direction.Direction)
        };

        private static string SquareTitle(ENineStarKiEnergyCycleType cycle)
        {
            switch (cycle)
            {
                case ENineStarKiEnergyCycleType.EpochEnergy: return Dictionary.EpochEnergyLabel;
                case ENineStarKiEnergyCycleType.GenerationalEnergy: return Dictionary.GenerationalEnergyLabel;
                case ENineStarKiEnergyCycleType.YearlyCycleEnergy: return Dictionary.YearlyEnergyLabel;
                case ENineStarKiEnergyCycleType.MonthlyCycleEnergy: return Dictionary.MonthlyEnergyLabel;
                case ENineStarKiEnergyCycleType.DailyEnergy: return Dictionary.DailyEnergyLabel;
                default: return Dictionary.HourlyEnergyLabel;
            }
        }

        private static string NumberColor(int number)
        {
            switch (number)
            {
                case 2: return "#111111";
                case 3: return "#4cff00";
                case 4: return "#008040";
                case 5: return "#ffff00";
                case 7: return "#ff0000";
                case 9: return "#a000ff";
                default: return "#ffffff";
            }
        }

        private static string SummaryColor(string severity)
        {
            switch (severity)
            {
                case "purple-warning": return "#800080";
                case "red-warning": return "#cc0000";
                case "red": return "#ff0000";
                case "light-red": return "#ff6666";
                case "orange": return "#ffa500";
                case "yellow": return "#ffff00";
                case "light-yellow": return "#ffffcc";
                default: return "#90ee90";
            }
        }
    }
}
