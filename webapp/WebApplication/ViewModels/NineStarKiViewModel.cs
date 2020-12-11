using K9.DataAccessLayer.Models;
using K9.WebApplication.Models;

namespace K9.WebApplication.ViewModels
{
    public class NineStarKiViewModel
    {
        public PersonModel PersonModel { get; set; }
        public NineStarKiModel NineStarKiModel { get; set; }
        public EnergyInfo MainEnergyInfo { get; set; }
        public EnergyInfo EmotionalEnergyInfo { get; set; }
        public EnergyInfo SurfaceEnergyInfo { get; set; }
    }
}