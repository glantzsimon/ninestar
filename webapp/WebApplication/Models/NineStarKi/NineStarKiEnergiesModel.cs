using System.ComponentModel.DataAnnotations;
using K9.Globalisation;

namespace K9.WebApplication.Models
{
    public class NineStarKiEnergiesModel
    {
        /// <summary>
        /// 81 year Period
        /// </summary>
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EpochEnergyLabel)]
        public NineStarKiEnergy Epoch { get; set; }

        /// <summary>
        /// 9-Year Period
        /// </summary>
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.GenerationalEnergyLabel)]
        public NineStarKiEnergy Generation { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.YearlyEnergyLabel)]
        public NineStarKiEnergy Year { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.MonthlyEnergyLabel)]
        public NineStarKiEnergy Month { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SurfaceEnergyLabel)]
        public NineStarKiEnergy Surface { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DailyEnergyLabel)]
        public NineStarKiEnergy Day { get; set; }

        /// <summary>
        /// Used for daily ki, where on some days, two values exist and one is inverted
        /// </summary>
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DailyEnergyLabel)]
        public NineStarKiEnergy DayInverted { get; set}

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.HourlyEnergyLabel)]
        public NineStarKiEnergy Hour { get; set; }
    }
}