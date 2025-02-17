using K9.WebApplication.Models;
using System.Collections.Generic;

namespace K9.WebApplication.ViewModels
{
    public class NineStarKiSummaryKbViewModel
    {
        public List<NineStarKiEnergy> CharacterEnergies { get; set; }
        public List<NineStarKiEnergy> MainEnergies { get; set; }
        public NineStarKiModalitySummaryViewModel DynamicEnergies { get; set; }
        public NineStarKiModalitySummaryViewModel StableEnergies { get; set; }
        public NineStarKiModalitySummaryViewModel FlexibleEnergies { get; set; }

        public NineStarKiSummaryKbViewModel(NineStarKiSummaryViewModel model)
        {
            CharacterEnergies = model.CharacterEnergies;
            MainEnergies = model.MainEnergies;
            DynamicEnergies = model.DynamicEnergies;
            StableEnergies = model.StableEnergies;
            FlexibleEnergies = model.FlexibleEnergies;
        }
    }
}