using K9.WebApplication.Models;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.ViewModels
{
    public class NineStarKiDrectionsChartViewModel
    {
        private List<NineStarKiDirection> _directions { get; }

        public NineStarKiDrectionsChartViewModel(List<NineStarKiDirection> directions)
        {
            _directions = directions;
        }

        public List<(ENineStarKiDirection Direction, string DirectionName, int Score, string CssClassName, string Guidance)> GetDirectionsChartData()
        {
            return _directions
                .GroupBy(e => e.Score)
                .OrderByDescending(g => g.Key)
                .SelectMany(g => g.Select(e => (e.Direction, e.GetDirectionName(), e.Score, GetCssClassName(e.Score), GetGuidance(e.Score))))
                .ToList();
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
            if (score == 11) return "AvoidAllTravel";
            if (score == 10) return "AvoidAllTravel";
            if (score == 8) return "AvoidTravel";
            if (score == 7) return "KeepTravelShortAndInfrequent";
            if (score == 5) return "TravelWithGreatCare";
            if (score == 3) return "TravelWithCare";
            if (score == 1) return "TravelWithCare";
            return "green";
        }

    }
}