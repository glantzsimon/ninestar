using K9.Globalisation;
using K9.WebApplication.Config;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using K9.SharedLibrary.Extensions;

namespace K9.WebApplication.ViewModels
{
    public class PlannerViewModel
    {
        [UIHint("PlannerView")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ViewLabel)]
        public EPlannerView View { get; set; }

        [UIHint("ScopeDisplay")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DisplayLabel)]
        public EScopeDisplay Display { get; set; }

        public EHousesDisplay HousesDisplay { get; set; }

        public NineStarKiModel NineStarKiModel { get; set; }
        public NineStarKiEnergy Energy { get; set; }
        public NineStarKiEnergy SecondEnergy { get; set; }
        public DateTime Lichun { get; set; }
        public DateTime SelectedDateTime { get; set; }
        public DateTime PeriodStarsOn { get; set; }
        public DateTime PeriodEndsOn { get; set; }
        public List<PlannerViewModelItem> Energies { get; set; }
        public string UpdateParentUrl { get; set; }
        public string UpdateChildUrl { get; set; }
        public MoonPhase MoonPhase { get; set; }

        public string SelectedDateTimeString => SelectedDateTime.ToAjaxDateTimeString();

        public string PeriodStartsOnDateTimeString => GetPeriodStartsOnString();

        public string EnergyName => Energy.EnergyName;

        public EPlannerView NextViewUp => GetNextViewUp();

        public string ImgSrc => $"{DefaultValuesConfiguration.Instance.BaseImagesPath}/ninestar/energies/{Energy.EnergyUIName}.png";

        public string ImgAlt => $"{Dictionary.NineStarKiAstrologyFreeCalculator} {Energy.EnergyTitle}";

        public string PeriodDatesTitle => GetPeriodDatesTitle();

        public string PeriodDatesDetails => GetPeriodDatesDetails();

        public string PeriodAgesDetails => GetPeriodAgeDetails();

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
                    return $"{energy.EnergyStartsOn.ToString("M/dd")} {energy.EnergyEndsOn.ToString("M/dd")}";
                    
                default:
                    return $"{energy.EnergyStartsOn.ToString("MMM/dd")} {energy.EnergyEndsOn.ToString("MMM/dd")}";
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
                    return PeriodStarsOn.AddDays(2).ToAjaxDateTimeString();

                default:
                    return PeriodStarsOn.ToAjaxDateTimeString();
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
                    var periodStart = PeriodStarsOn.Year.ToString();
                    var periodEnd = PeriodEndsOn.Year.ToString();
                    return $"{periodStart} - {periodEnd}";

                case EPlannerView.Month:
                    if (PeriodStarsOn.Year == PeriodEndsOn.Year)
                    {
                        periodStart = PeriodStarsOn.ToString("MMM");
                        periodEnd = PeriodEndsOn.ToString("MMM");
                        return $"{periodStart} - {periodEnd} {PeriodStarsOn.Year}";
                    }
                    else
                    {
                        periodStart = PeriodStarsOn.ToString("MMM yyyy");
                        periodEnd = PeriodEndsOn.ToString("MMM yyyy");
                        return $"{periodStart} - {periodEnd}";
                    }

                case EPlannerView.Day:
                    return PeriodStarsOn.ToString("ddd MMM dd yyyy");

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
                    var periodStart = PeriodStarsOn.ToString("MMM/dd/yy");
                    var periodEnd = PeriodEndsOn.ToString("MMM/dd/yy");
                    return $"{periodStart} - {periodEnd}";

                default:
                    return string.Empty;
            }
        }

        private string GetPeriodAgeDetails()
        {
            var startAge = NineStarKiModel.PersonModel.DateOfBirth.GetAgeInYearsAsOf(PeriodStarsOn);
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