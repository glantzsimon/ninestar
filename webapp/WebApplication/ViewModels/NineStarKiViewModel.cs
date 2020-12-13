using K9.DataAccessLayer.Models;
using K9.WebApplication.Models;

namespace K9.WebApplication.ViewModels
{
    public class NineStarKiViewModel
    {
        public PersonModel PersonModel { get; set; }
        public NineStarKiModel NineStarKiModel { get; set; }
        public NineStarKiPersonalProfile MainEnergyInfo { get; set; }
        public NineStarKiPersonalProfile EmotionalEnergyInfo { get; set; }
        public NineStarKiPersonalProfile SurfaceEnergyInfo { get; set; }
    }
}