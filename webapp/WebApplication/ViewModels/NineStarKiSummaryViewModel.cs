using K9.WebApplication.Models;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.ViewModels
{
    public class NineStarKiSummaryViewModel
    {
        public List<NineStarKiEnergy> CharacterEnergies { get; set; }
        public List<NineStarKiEnergy> MainEnergies { get; set; }

        public NineStarKiSummaryViewModel(List<NineStarKiModel> mainEnergies, List<NineStarKiModel> characterEnergies)
        {
            CharacterEnergies = characterEnergies.Select(e => e.EmotionalEnergy).ToList();
            MainEnergies = mainEnergies.Select(e => e.MainEnergy).ToList();
        }
    }
}