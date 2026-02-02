using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace K9.WebApplication.ViewModels
{
    public class PlannerViewModel
    {
        [IgnoreDataMember]
        [UIHint("PlannerView")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ViewLabel)]
        public EPlannerView View { get; set; }

        [IgnoreDataMember]
        [UIHint("ScopeDisplay")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DisplayLabel)]
        public EScopeDisplay Display { get; set; }

        [IgnoreDataMember]
        public EHousesDisplay HousesDisplay { get; set; }

        [IgnoreDataMember]
        public NineStarKiModel NineStarKiModel { get; set; }

        [IgnoreDataMember]
        public NineStarKiEnergy Energy { get; set; }

        [IgnoreDataMember]
        public NineStarKiEnergy SecondEnergy { get; set; }

        [IgnoreDataMember]
        public DateTime Lichun { get; set; }

        public DateTime SelectedDateTime { get; set; }
        public DateTime PeriodStartsOn { get; set; }
        public DateTime PeriodEndsOn { get; set; }
        public List<PlannerViewModelItem> Energies { get; set; }
        
        [IgnoreDataMember]
        public string UpdateParentUrl { get; set; }
        
        [IgnoreDataMember]
        public string UpdateChildUrl { get; set; }

        [IgnoreDataMember]
        public MoonPhase MoonPhase { get; set; }

        [IgnoreDataMember]
        public string SelectedDateTimeString => SelectedDateTime.ToAjaxDateTimeString();

        [IgnoreDataMember]
        public string PeriodStartsOnDateTimeString => GetPeriodStartsOnString();

        [IgnoreDataMember]
        public string EnergyName => Energy.EnergyName;

        [IgnoreDataMember]
        public EPlannerView NextViewUp => GetNextViewUp();

        [IgnoreDataMember]
        public string ImgSrc => $"{MediaService.BaseImagesPath}/ninestar/energies/{Energy.EnergyUIName}.png";

        [IgnoreDataMember]
        public string ImgAlt => $"{Dictionary.NineStarKiAstrologyFreeCalculator} {Energy.EnergyTitle}";

        [IgnoreDataMember]
        public string PeriodDatesTitle => GetPeriodDatesTitle();

        [IgnoreDataMember]
        public string PeriodDatesDetails => GetPeriodDatesDetails();

        [IgnoreDataMember]
        public string PeriodAgesDetails => GetPeriodAgeDetails();

        [IgnoreDataMember]
        public EPlannerView ChildView => GetChildView();
        
        public string GetEnergyTitle(PlannerViewModelItem energy)
        {
            switch (View)
            {
                case EPlannerView.EightyOneYear:
                case EPlannerView.NineYear:
                    return $"{energy.EnergyStartsOn.ToString("yyyy")} - {energy.EnergyEndsOn.ToString("yyyy")}";

                case EPlannerView.Year:
                    return $"{energy.EnergyStartsOn.ToString("MMM")}";

                case EPlannerView.Month:
                    return $"{energy.EnergyStartsOn.ToString("MMM dd")}";

                default:
                    return $"{energy.EnergyStartsOn.ToString("MMM")}";
            }
        }

        public string GetEnergyDatesDetails(PlannerViewModelItem energy)
        {
            switch (View)
            {
                case EPlannerView.EightyOneYear:
                case EPlannerView.NineYear:
                    return $"{energy.EnergyStartsOn.ToString("MMM d")} - {energy.EnergyEndsOn.ToString("MMM d")}";
                    
                default:
                    var startMonthLetter = energy.EnergyStartsOn.ToString("MMM");
                    var finishMonthLetter = energy.EnergyEndsOn.ToString("MMM");
                    return $"{startMonthLetter} {energy.EnergyStartsOn.Day}-{finishMonthLetter} {energy.EnergyEndsOn.Day}";
            }
        }

        public string GetEnergAgeDetails(PlannerViewModelItem energy)
        {
            var startAge = NineStarKiModel.PersonModel.DateOfBirth.GetAgeInYearsAsOf(energy.EnergyStartsOn);
            var endAge = NineStarKiModel.PersonModel.DateOfBirth.GetAgeInYearsAsOf(energy.EnergyEndsOn);

            return ToAgeString(startAge, endAge);   
        }

        private EPlannerView GetNextViewUp()
        {
            switch (View)
            {
                case EPlannerView.EightyOneYear:
                case EPlannerView.NineYear:
                    return EPlannerView.EightyOneYear;

                case EPlannerView.Year:
                    return EPlannerView.NineYear;

                case EPlannerView.Month:
                    return EPlannerView.Year;

                case EPlannerView.Day:
                    return EPlannerView.Month;

                default:
                    return EPlannerView.Year;
            }
        }

        private string GetPeriodStartsOnString()
        {
            switch (View)
            {
                case EPlannerView.EightyOneYear:
                case EPlannerView.NineYear:
                case EPlannerView.Year:
                case EPlannerView.Month:
                    // To account for time zone differences, add two days (so you don't get the previous period
                    return PeriodStartsOn.AddDays(2).ToAjaxDateTimeString();

                default:
                    return PeriodStartsOn.ToAjaxDateTimeString();
            }
        }

        private EPlannerView GetChildView()
        {
            switch (View)
            {
                case EPlannerView.EightyOneYear:
                    return EPlannerView.NineYear;

                case EPlannerView.NineYear:
                    return EPlannerView.Year;

                case EPlannerView.Year:
                    return EPlannerView.Month;

                case EPlannerView.Month:
                    return EPlannerView.Day;

                default:
                    return EPlannerView.Day;
            }
        }

        private string GetPeriodDatesTitle()
        {
            switch (View)
            {
                case EPlannerView.EightyOneYear:
                case EPlannerView.NineYear:
                case EPlannerView.Year:
                    var periodStart = PeriodStartsOn.Year.ToString();
                    var periodEnd = PeriodEndsOn.Year.ToString();
                    return $"{periodStart} - {periodEnd}";

                case EPlannerView.Month:
                    if (PeriodStartsOn.Year == PeriodEndsOn.Year)
                    {
                        periodStart = PeriodStartsOn.ToString("MMM");
                        periodEnd = PeriodEndsOn.ToString("MMM");
                        return $"{periodStart} - {periodEnd} {PeriodStartsOn.Year}";
                    }
                    else
                    {
                        periodStart = PeriodStartsOn.ToString("MMM yyyy");
                        periodEnd = PeriodEndsOn.ToString("MMM yyyy");
                        return $"{periodStart} - {periodEnd}";
                    }

                case EPlannerView.Day:
                    return PeriodStartsOn.ToString("ddd MMM dd yyyy");

                default:
                    return string.Empty;
            }
        }

        private string GetPeriodDatesDetails()
        {
            switch (View)
            {
                case EPlannerView.EightyOneYear:
                case EPlannerView.NineYear:
                case EPlannerView.Year:
                case EPlannerView.Month:
                    var periodStart = PeriodStartsOn.ToString("MMM d-yy");
                    var periodEnd = PeriodEndsOn.ToString("MMM d-yy");
                    return $"{periodStart} - {periodEnd}";

                default:
                    return string.Empty;
            }
        }

        private string GetPeriodAgeDetails()
        {
            var startAge = NineStarKiModel.PersonModel.DateOfBirth.GetAgeInYearsAsOf(PeriodStartsOn);
            var endAge = NineStarKiModel.PersonModel.DateOfBirth.GetAgeInYearsAsOf(PeriodEndsOn);

            if (startAge < 0)
            {
                startAge = 0;
            }

            if (endAge < 0)
            {
                return "";
            }

            return ToAgeString(startAge, endAge);
        }

        private static string ToAgeString(int startAge, int endAge)
        {
            return startAge == endAge ? $"Age {startAge}" : $"Age {startAge} - {endAge}";
        }
    }
}