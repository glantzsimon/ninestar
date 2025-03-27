using K9.WebApplication.Models;
using System;
using System.Collections.Generic;
using K9.WebApplication.Enums;

namespace K9.WebApplication.ViewModels
{
    public class PlannerViewModel
    {
        public Enums.EPlannerView View { get; set; }
        public NineStarKiEnergy Energy { get; set; }
        public DateTime Lichun { get; set; }
        public DateTime PeriodStarsOn { get; set; }
        public DateTime PeriodEndsOn { get; set; }
        public List<(NineStarKiEnergy Energy, DateTime EnergyStartsOn, DateTime EnergyEndsOn, bool IsSelected)> Energies { get; set; }

        public string PeriodDatesTitle => GetPeriodDatesTitle();

        public string PeriodDatesDetails => GetPeriodDatesDetails();

        public string GetEnergyTitle((NineStarKiEnergy Energy, DateTime EnergyStartsOn, DateTime EnergyEndsOn, bool IsSelected) energy)
        {
            return $"{energy.EnergyStartsOn.ToString("MMM")}";
        }

        public string GetEnergyDatesDetails((NineStarKiEnergy Energy, DateTime EnergyStartsOn, DateTime EnergyEndsOn, bool IsSelected) energy)
        {
            return $"{energy.EnergyStartsOn.ToString("MMM dd")} {energy.EnergyEndsOn.ToString("MMM dd")}";
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
                    return PeriodStarsOn.ToString("ddd MMM dd YYYY");

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
                    var periodStart = PeriodStarsOn.Year.ToString("MMM/dd/yy");
                    var periodEnd = PeriodEndsOn.Year.ToString("MMM/dd/yy");
                    return $"{periodStart} - {periodEnd}";

                default:
                    return string.Empty;
            }
        }
    }
}