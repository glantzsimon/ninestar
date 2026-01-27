using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using System;
using System.Runtime.Serialization;

namespace K9.WebApplication.ViewModels
{
    public class PlannerViewModelItem
    {
        public PlannerViewModelItem()
        {
        }

        public PlannerViewModelItem(NineStarKiEnergy energy, NineStarKiEnergy secondEnergy, DateTime energyStartsOn, DateTime energyEndsOn,
            bool isActive, EPlannerView childView, MoonPhase moonPhase = null)
        {
            Energy = energy;
            EnergySummary = new NineStarKiEnergySummary(energy);
            SecondEnergy = secondEnergy;
            EnergyStartsOn = energyStartsOn;
            EnergyEndsOn = energyEndsOn;
            IsActive = isActive;
            ChildView = childView;
            MoonPhase = moonPhase;
        }

        public NineStarKiEnergySummary EnergySummary { get; set; }

        [IgnoreDataMember]
        public NineStarKiEnergy Energy { get; set; }

        [IgnoreDataMember]
        public NineStarKiEnergy SecondEnergy { get; set; }

        public DateTime EnergyStartsOn { get; set; }

        public DateTime EnergyEndsOn { get; set; }

        [IgnoreDataMember]
        public bool IsActive { get; set; }

        [IgnoreDataMember]
        public EPlannerView ChildView { get; set; }

        [IgnoreDataMember]
        public MoonPhase MoonPhase { get; set; }

        [IgnoreDataMember]
        public bool IsSplitEnergy => Energy.EnergyNumber != SecondEnergy.EnergyNumber;

        public string EnergyName => Energy.EnergyName;

        [IgnoreDataMember]
        public string EnergyStartsOnDateTimeString => GetEnergyStartsOnString();

        [IgnoreDataMember]
        public string ImageSrc => $"{MediaService.BaseImagesPath}/ninestar/energies/{Energy.EnergyUIName}.png";

        [IgnoreDataMember]
        public string ImageAlt => $"{Dictionary.NineStarKiAstrologyFreeCalculator} {Energy.EnergyTitle}";

        [IgnoreDataMember]
        public string SelectedCssClass => IsActive ? "active current" : "";

        [IgnoreDataMember]
        public string SecondEnergyName => SecondEnergy.EnergyName;

        [IgnoreDataMember]
        public string SecondImageSrc => $"{MediaService.BaseImagesPath}/ninestar/energies/{SecondEnergy.EnergyUIName}.png";

        [IgnoreDataMember]
        public string SecondImageAlt => $"{Dictionary.NineStarKiAstrologyFreeCalculator} {SecondEnergy.EnergyTitle}";

        [IgnoreDataMember]
        public string TimeSlotString => $"{EnergyStartsOn.TimeOfDay.ToString(@"hh\:mm")} - {EnergyEndsOn.TimeOfDay.ToString(@"hh\:mm")}";

        private string GetEnergyStartsOnString()
        {
            switch (ChildView)
            {
                case EPlannerView.EightyOneYear:
                case EPlannerView.NineYear:
                case EPlannerView.Year:
                case EPlannerView.Month:
                    // To account for time zone differences, add two days (so you don't get the previous period
                    return EnergyStartsOn.AddDays(2).ToAjaxDateTimeString();

                default:
                    return EnergyStartsOn.ToAjaxDateTimeString();
            }
        }
    }
}