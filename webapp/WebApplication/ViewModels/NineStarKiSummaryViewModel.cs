using K9.WebApplication.Models;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.ViewModels
{
    public class NineStarKiSummaryViewModel
    {
        public List<NineStarKiEnergy> CharacterEnergies { get; set; }
        public List<NineStarKiEnergy> MainEnergies { get; set; }
        public NineStarKiModalitySummaryViewModel DynamicEnergies { get; set; }
        public NineStarKiModalitySummaryViewModel StableEnergies { get; set; }
        public NineStarKiModalitySummaryViewModel FlexibleEnergies { get; set; }

        public NineStarKiSummaryViewModel(
            List<NineStarKiModel> mainEnergies,
            List<NineStarKiModel> characterEnergies,
            List<NineStarKiEnergy> dynamicEnergies,
            List<NineStarKiEnergy> stableEnergies,
            List<NineStarKiEnergy> flexibleEnergies)
        {
            CharacterEnergies = characterEnergies.Select(e => e.CharacterEnergy).OrderBy(e => e.EnergyNumber).ToList();
            MainEnergies = mainEnergies.Select(e => e.MainEnergy).ToList();
            DynamicEnergies = new NineStarKiModalitySummaryViewModel(ENineStarKiModality.Dynamic, dynamicEnergies);
            StableEnergies = new NineStarKiModalitySummaryViewModel(ENineStarKiModality.Stable, stableEnergies); ;
            FlexibleEnergies = new NineStarKiModalitySummaryViewModel(ENineStarKiModality.Flexible, flexibleEnergies); ;
        }
    }
}