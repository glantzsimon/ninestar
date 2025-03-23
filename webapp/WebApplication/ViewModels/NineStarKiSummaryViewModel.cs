using K9.WebApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace K9.WebApplication.ViewModels
{
    public class NineStarKiSummaryViewModel
    {
        [ScriptIgnore]
        public List<NineStarKiEnergy> CharacterEnergies { get; set; }

        [ScriptIgnore]
        public List<NineStarKiEnergy> MainEnergies { get; set; }

        [ScriptIgnore]
        public NineStarKiModalitySummaryViewModel DynamicEnergies { get; set; }

        [ScriptIgnore]
        public NineStarKiModalitySummaryViewModel StableEnergies { get; set; }
        
        [ScriptIgnore]
        public NineStarKiModalitySummaryViewModel ReflectiveEnergies { get; set; }

        public NineStarKiSummaryViewModel(
            List<NineStarKiModel> mainEnergies,
            List<NineStarKiModel> characterEnergies,
            List<NineStarKiEnergy> dynamicEnergies,
            List<NineStarKiEnergy> stableEnergies,
            List<NineStarKiEnergy> reflectiveEnergies)
        {
            CharacterEnergies = characterEnergies.Select(e => e.CharacterEnergy).OrderBy(e => e.EnergyNumber).ToList();
            MainEnergies = mainEnergies.Select(e => e.MainEnergy).ToList();
            DynamicEnergies = new NineStarKiModalitySummaryViewModel(ENineStarKiModality.Dynamic, dynamicEnergies);
            StableEnergies = new NineStarKiModalitySummaryViewModel(ENineStarKiModality.Stable, stableEnergies); ;
            ReflectiveEnergies = new NineStarKiModalitySummaryViewModel(ENineStarKiModality.Reflective, reflectiveEnergies); ;
        }
    }
}