using K9.WebApplication.Models;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.ViewModels
{
    public class NineStarKiSummaryKbViewModel
    {
        public List<NineStarKiEnergySummary> CharacterEnergies { get; set; }
        public List<NineStarKiEnergySummary> MainEnergies { get; set; }
        public NineStarKiModalitySummaryViewModel DynamicEnergies { get; set; }
        public NineStarKiModalitySummaryViewModel StableEnergies { get; set; }
        public NineStarKiModalitySummaryViewModel FlexibleEnergies { get; set; }

        public NineStarKiSummaryKbViewModel(NineStarKiSummaryViewModel model)
        {
            CharacterEnergies =
                new List<NineStarKiEnergySummary>(model.CharacterEnergies.Select(e => new NineStarKiEnergySummary(e)));
            MainEnergies =
                new List<NineStarKiEnergySummary>(model.MainEnergies.Select(e => new NineStarKiEnergySummary(e)));
            DynamicEnergies = model.DynamicEnergies;
            StableEnergies = model.StableEnergies;
            FlexibleEnergies = model.FlexibleEnergies;
        }
    }
}