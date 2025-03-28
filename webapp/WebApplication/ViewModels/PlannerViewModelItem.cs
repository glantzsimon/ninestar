using K9.WebApplication.Models;
using System;
using K9.Globalisation;
using K9.WebApplication.Config;

namespace K9.WebApplication.ViewModels
{
    public class PlannerViewModelItem
    {
        public NineStarKiEnergy Energy { get; set; }
        public NineStarKiEnergy SecondEnergy { get; set; }
        public DateTime EnergyStartsOn { get; set; }
        public DateTime EnergyEndsOn { get; set; }
        public bool IsSelected { get; set; }

        public bool IsSplitEnergy => Energy.EnergyNumber != SecondEnergy.EnergyNumber;

        public string EnergyName => Energy.EnergyName;
        public string ImageSrc => $"{DefaultValuesConfiguration.Instance.BaseImagesPath}/ninestar/energies/{Energy.EnergyUIName}.png";
        public string ImageAlt => $"{Dictionary.NineStarKiAstrologyFreeCalculator} {Energy.EnergyTitle}";
        public string SelectedCssClass => IsSelected ? "selected" : "";

        public string SecondEnergyName => SecondEnergy.EnergyName;
        public string SecondImageSrc => $"{DefaultValuesConfiguration.Instance.BaseImagesPath}/ninestar/energies/{SecondEnergy.EnergyUIName}.png";
        public string SecondImageAlt => $"{Dictionary.NineStarKiAstrologyFreeCalculator} {SecondEnergy.EnergyTitle}";
        
        public PlannerViewModelItem()
        {
        }

        public PlannerViewModelItem(NineStarKiEnergy energy, NineStarKiEnergy secondEnergy, DateTime energyStartsOn, DateTime energyEndsOn,
            bool isSelected)
        {
            Energy = energy;
            SecondEnergy = secondEnergy;
            EnergyStartsOn = energyStartsOn;
            EnergyEndsOn = energyEndsOn;
            IsSelected = isSelected;
        }
    }
}