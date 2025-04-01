using K9.WebApplication.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Enums;

namespace K9.WebApplication.ViewModels
{
    public class NineStarKiDrectionsChartViewModel
    {
        private List<(NineStarKiEnergy PersonalHouseOccupied, NineStarKiEnergy GlobalKi, ENineStarKiEnergyCycleType CycleType, List<NineStarKiDirection> Directions)> _directionModels { get; }

        public NineStarKiDrectionsChartViewModel(List<(NineStarKiEnergy PersonalHouseOccupied, NineStarKiEnergy GlobalKi, ENineStarKiEnergyCycleType CycleType, List<NineStarKiDirection> Directions)> directionModels)
        {
            _directionModels = directionModels;
            AddDirectionsForCentre();
        }

        public List<(ENineStarKiDirection Direction, string DirectionName, string CssClassName, string Guidance, int Score)> GetDirectionsChartData()
        {
            return _directionModels.SelectMany(e => e.Directions)
                .Where(e => e.Direction != ENineStarKiDirection.Centre)
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
                }).Distinct().OrderByDescending(e => e.Score).ThenBy(e => e.Direction.GetAttribute<DisplayAttribute>().Order).ToList();
        }

        private void AddDirectionsForCentre()
        {
            foreach (var item in _directionModels)
            {
                var newDirections = new List<NineStarKiDirection>();
                if (item.PersonalHouseOccupied.EnergyNumber == 5 || item.GlobalKi.EnergyNumber == 5)
                {
                    newDirections.AddRange(new List<NineStarKiDirection>
                    {
                        new NineStarKiDirection(EUnfavourableDirection.Unspecified, "", new NineStarKiEnergy(ENineStarKiEnergy.Water) { EnergyCycleType = item.CycleType }),
                        new NineStarKiDirection(EUnfavourableDirection.Unspecified, "", new NineStarKiEnergy(ENineStarKiEnergy.Soil) { EnergyCycleType =  item.CycleType }),
                        new NineStarKiDirection(EUnfavourableDirection.Unspecified, "", new NineStarKiEnergy(ENineStarKiEnergy.Thunder) { EnergyCycleType =  item.CycleType }),
                        new NineStarKiDirection(EUnfavourableDirection.Unspecified, "", new NineStarKiEnergy(ENineStarKiEnergy.Wind) { EnergyCycleType =  item.CycleType }),
                        new NineStarKiDirection(EUnfavourableDirection.Unspecified, "", new NineStarKiEnergy(ENineStarKiEnergy.Heaven) { EnergyCycleType =  item.CycleType }),
                        new NineStarKiDirection(EUnfavourableDirection.Unspecified, "", new NineStarKiEnergy(ENineStarKiEnergy.Lake) { EnergyCycleType =  item.CycleType }),
                        new NineStarKiDirection(EUnfavourableDirection.Unspecified, "", new NineStarKiEnergy(ENineStarKiEnergy.Mountain) { EnergyCycleType =  item.CycleType }),
                        new NineStarKiDirection(EUnfavourableDirection.Unspecified, "", new NineStarKiEnergy(ENineStarKiEnergy.Fire) { EnergyCycleType =  item.CycleType })
                    });
                }

                item.Directions.AddRange(newDirections);
            }
        }

        private string GetCssClassName(int score)
        {
            if (score >= 11) return "purple-warning";
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
            if (score >= 11) return Dictionary.AvoidAllTravel;
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