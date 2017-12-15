using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.Models
{
    public enum ENineStarEnergy
    {
        Unspecified,
        Water,
        Soil,
        Thunder,
        Wind,
        CoreEarth,
        Heaven,
        Lake,
        Mountain,
        Fire
    }

    public class NineStarKiEnergy
    {
        public ENineStarEnergy Energy { get; set; }

        public int EnergyNumber => (int) Energy;

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.GenderLabel)]
        public EGender Gender { get; set; }

        

        
    }
}