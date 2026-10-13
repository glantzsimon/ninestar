using System.ComponentModel.DataAnnotations;
using K9.Globalisation;

namespace K9.WebApplication.Models
{
    public class NineStarKiEnergiesModel
    {
        internal NineStarKiEnergiesModel CreateRequestCopy(CalculationCopyContext context) =>
            context.Copy(this, source => (NineStarKiEnergiesModel)source.MemberwiseClone(), (source, copy) =>
            {
                copy.Epoch = source.Epoch?.CreateRequestCopy(context);
                copy.Generation = source.Generation?.CreateRequestCopy(context);
                copy.Year = source.Year?.CreateRequestCopy(context);
                copy.Month = source.Month?.CreateRequestCopy(context);
                copy.Surface = source.Surface?.CreateRequestCopy(context);
                copy.Day = source.Day?.CreateRequestCopy(context);
                copy.Day2 = source.Day2?.CreateRequestCopy(context);
                copy.DayInverted = source.DayInverted?.CreateRequestCopy(context);
                copy.Day2Inverted = source.Day2Inverted?.CreateRequestCopy(context);
                copy.Hour = source.Hour?.CreateRequestCopy(context);
            });

        /// <summary>
        /// 81 year Period
        /// </summary>
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EpochEnergyLabel)]
        public NineStarKiEnergy Epoch { get; set; }

        /// <summary>
        /// 9-Year Period
        /// </summary>
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.GenerationalEnergyLabel)]
        public NineStarKiEnergy Generation { get; set; }/// <summary>

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.YearlyEnergyLabel)]
        public NineStarKiEnergy Year { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.MonthlyEnergyLabel)]
        public NineStarKiEnergy Month { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SurfaceEnergyLabel)]
        public NineStarKiEnergy Surface { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DailyEnergyLabel)]
        public NineStarKiEnergy Day { get; set; }

        /// <summary>
        /// In many time zones, the daily Ki changes a some point throughout the day, so there are essentially 2 Kis for that day
        /// </summary>
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DailyEnergyLabel)]
        public NineStarKiEnergy Day2 { get; set; }

        /// <summary>
        /// Used for daily ki, where on some days, two values exist and one is inverted
        /// </summary>
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DailyEnergyLabel)]
        public NineStarKiEnergy DayInverted { get; set; }

        /// <summary>
        /// In some time zones the daily Ki changes at a certain time plus two values exist for the day, meaning that there are 4 Kis for that day
        /// </summary>
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DailyEnergyLabel)]
        public NineStarKiEnergy Day2Inverted { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.HourlyEnergyLabel)]
        public NineStarKiEnergy Hour { get; set; }
    }
}