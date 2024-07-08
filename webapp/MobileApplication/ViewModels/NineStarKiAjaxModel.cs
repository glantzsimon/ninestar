using K9.WebApplication.Models;

namespace K9.WebApplication.ViewModels
{
    public class NineStarKiAjaxModel
    {
        public ENineStarKiEnergy MainEnergy { get; set; }
        public ENineStarKiEnergy CharacterEnergy { get; set; }
        public ENineStarKiEnergy SurfaceEnergy { get; set; }
        public ENineStarKiEnergy YearlyCycleEnergy { get; set; }
        public ENineStarKiEnergy MonthlyCycleEnergy { get; set; }

        public string HealthAdvice { get; set; }
    }
}