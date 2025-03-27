using K9.WebApplication.Models;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace K9.WebApplication.ViewModels
{
    public class NineStarKiSummaryViewModel
    {
        [ScriptIgnore]
        public List<NineStarKiEnergy> MainEnergies { get; }

        [ScriptIgnore]
        public List<NineStarKiEnergy> CharacterEnergies { get; }

        [ScriptIgnore]
        public List<NineStarKiEnergy> EightyOneYearlyCycleEnergies { get; }

        [ScriptIgnore]
        public List<NineStarKiEnergy> NineYearlyCycleEnergies { get; }

        [ScriptIgnore]
        public List<NineStarKiEnergy> YearlyCycleEnergies { get; }

        [ScriptIgnore]
        public List<NineStarKiEnergy> MonthlyCycleEnergies { get; }

        [ScriptIgnore]
        public List<NineStarKiEnergy> DailyCycleEnergies { get; }

        [ScriptIgnore]
        public List<NineStarKiEnergy> HourlyCycleEnergies { get; }

        [ScriptIgnore]
        public NineStarKiModalitySummaryViewModel DynamicEnergies { get; }

        [ScriptIgnore]
        public NineStarKiModalitySummaryViewModel StableEnergies { get; }

        [ScriptIgnore]
        public NineStarKiModalitySummaryViewModel ReflectiveEnergies { get; }

        public NineStarKiSummaryViewModel(
            List<NineStarKiEnergy> eightyOneYearlyEnergies,
            List<NineStarKiEnergy> nineYearlyEnergies,
            List<NineStarKiEnergy> yearEnergies,
            List<NineStarKiEnergy> monthEnergies,
            List<NineStarKiEnergy> dailyEnergies,
            List<NineStarKiEnergy> hourlyEnergies,
            List<NineStarKiEnergy> dynamicEnergies,
            List<NineStarKiEnergy> stableEnergies,
            List<NineStarKiEnergy> reflectiveEnergies)
        {
            MainEnergies = yearEnergies;
            CharacterEnergies = monthEnergies;

            EightyOneYearlyCycleEnergies = eightyOneYearlyEnergies;
            NineYearlyCycleEnergies = nineYearlyEnergies;
            YearlyCycleEnergies = yearEnergies;
            MonthlyCycleEnergies = monthEnergies;
            DailyCycleEnergies = dailyEnergies;
            HourlyCycleEnergies = hourlyEnergies;

            DynamicEnergies = new NineStarKiModalitySummaryViewModel(ENineStarKiModality.Dynamic, dynamicEnergies);
            StableEnergies = new NineStarKiModalitySummaryViewModel(ENineStarKiModality.Stable, stableEnergies); ;
            ReflectiveEnergies = new NineStarKiModalitySummaryViewModel(ENineStarKiModality.Reflective, reflectiveEnergies); ;
        }
    }
}