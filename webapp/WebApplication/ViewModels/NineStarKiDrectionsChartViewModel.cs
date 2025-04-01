using K9.WebApplication.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;

namespace K9.WebApplication.ViewModels
{
    public class NineStarKiDrectionsChartViewModel
    {
        private List<NineStarKiDirection> _directions { get; }

        public NineStarKiDrectionsChartViewModel(List<NineStarKiDirection> directions)
        {
            _directions = directions;
            AddDirectionsForCentre();
        }

        public List<(ENineStarKiDirection Direction, string DirectionName, string CssClassName, string Guidance, int Score)> GetDirectionsChartData()
        {
            return _directions
                .Where(e => e.Direction != ENineStarKiDirection.Centre)
                .OrderBy(e => e.Direction.GetAttribute<DisplayAttribute>().Order)
                .GroupBy(e => e.Direction)
                .SelectMany(g =>
                {
                    var totalScore = g.Sum(x => x.Score);
                    return g.Select(e => (
                        Direction: e.Direction,
                        DirectionName: e.GetDirectionName(),
                        CssClass: GetCssClassName(totalScore),
                        Guidance: GetGuidance(totalScore),
                        Score: totalScore
                    ));
                }).OrderByDescending(e => e.Score).ThenBy(e => e.DirectionName).ToList();
        }

        private void AddDirectionsForCentre()
        {
            var newDirections = new List<NineStarKiDirection>();

            foreach (var direction in _directions)
            {
                if (direction.Direction == ENineStarKiDirection.Centre)
                {
                    newDirections.AddRange(new List<NineStarKiDirection>
                    {
                        new NineStarKiDirection("", "", new NineStarKiEnergy(ENineStarKiEnergy.Water) { EnergyCycleType =  direction.Energy.EnergyCycleType }),
                        new NineStarKiDirection("", "", new NineStarKiEnergy(ENineStarKiEnergy.Soil) { EnergyCycleType =  direction.Energy.EnergyCycleType }),
                        new NineStarKiDirection("", "", new NineStarKiEnergy(ENineStarKiEnergy.Thunder) { EnergyCycleType =  direction.Energy.EnergyCycleType }),
                        new NineStarKiDirection("", "", new NineStarKiEnergy(ENineStarKiEnergy.Wind) { EnergyCycleType =  direction.Energy.EnergyCycleType }),
                        new NineStarKiDirection("", "", new NineStarKiEnergy(ENineStarKiEnergy.Heaven) { EnergyCycleType =  direction.Energy.EnergyCycleType }),
                        new NineStarKiDirection("", "", new NineStarKiEnergy(ENineStarKiEnergy.Lake) { EnergyCycleType =  direction.Energy.EnergyCycleType }),
                        new NineStarKiDirection("", "", new NineStarKiEnergy(ENineStarKiEnergy.Mountain) { EnergyCycleType =  direction.Energy.EnergyCycleType }),
                        new NineStarKiDirection("", "", new NineStarKiEnergy(ENineStarKiEnergy.Fire) { EnergyCycleType =  direction.Energy.EnergyCycleType })
                    });
                }
            }

            _directions.AddRange(newDirections);
        }

        private string GetCssClassName(int score)
        {
            if (score == 11) return "purple-warning";
            if (score == 10) return "red-warning";
            if (score == 8) return "red";
            if (score == 7) return "light-red";
            if (score == 5) return "orange";
            if (score == 3) return "yellow";
            if (score == 1) return "light-yellow";
            return "green";
        }

        private string GetGuidance(int score)
        {
            if (score == 11) return Dictionary.AvoidAllTravel;
            if (score == 10) return Dictionary.AvoidAllTravel;
            if (score == 8) return Dictionary.AvoidTravel;
            if (score == 7) return Dictionary.KeepTravelShortAndInfrequent;
            if (score == 5) return Dictionary.TravelWithGreatCare;
            if (score == 3) return Dictionary.TravelWithCare;
            if (score == 1) return Dictionary.TravelWithCare;
            return "OK";
        }

    }
}