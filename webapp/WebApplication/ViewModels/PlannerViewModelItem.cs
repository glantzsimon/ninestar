using K9.WebApplication.Models;
using System;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Config;
using K9.WebApplication.Enums;

namespace K9.WebApplication.ViewModels
{
    public class PlannerViewModelItem
    {
        public PlannerViewModelItem()
        {
        }

        public PlannerViewModelItem(NineStarKiEnergy energy, NineStarKiEnergy secondEnergy, DateTime energyStartsOn, DateTime energyEndsOn,
            bool isActive, EPlannerView childView)
        {
            Energy = energy;
            SecondEnergy = secondEnergy;
            EnergyStartsOn = energyStartsOn;
            EnergyEndsOn = energyEndsOn;
            IsActive = isActive;
            ChildView = childView;
        }

        public NineStarKiEnergy Energy { get; set; }

        public NineStarKiEnergy SecondEnergy { get; set; }

        public DateTime EnergyStartsOn { get; set; }

        public DateTime EnergyEndsOn { get; set; }

        public bool IsActive { get; set; }

        public EPlannerView ChildView { get; set; }

        public bool IsSplitEnergy => Energy.EnergyNumber != SecondEnergy.EnergyNumber;

        public string EnergyName => Energy.EnergyName;

        public string EnergyStartsOnDateTimeString => GetEnergyStartsOnString();

        public string ImageSrc => $"{DefaultValuesConfiguration.Instance.BaseImagesPath}/ninestar/energies/{Energy.EnergyUIName}.png";

        public string ImageAlt => $"{Dictionary.NineStarKiAstrologyFreeCalculator} {Energy.EnergyTitle}";

        public string SelectedCssClass => IsActive ? "active" : "";

        public string SecondEnergyName => SecondEnergy.EnergyName;

        public string SecondImageSrc => $"{DefaultValuesConfiguration.Instance.BaseImagesPath}/ninestar/energies/{SecondEnergy.EnergyUIName}.png";

        public string SecondImageAlt => $"{Dictionary.NineStarKiAstrologyFreeCalculator} {SecondEnergy.EnergyTitle}";

        private string GetEnergyStartsOnString()
        {
            switch (ChildView)
            {
                case EPlannerView.EightyOneYear:
                case EPlannerView.NineYear:
                case EPlannerView.Year:
                case EPlannerView.Month:
                    return EnergyStartsOn.AddDays(2).ToAjaxDateTimeString();

                default:
                    return EnergyStartsOn.ToAjaxDateTimeString();
            }
        }
    }
}