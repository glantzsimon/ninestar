using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.Models
{

    public class NineStarKiModel
    {
        public PersonModel PersonModel { get; set; }

        [UIHint("Energy")]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.MainEnergyLabel)]
        public NineStarKiEnergy MainEnergy { get; set; }

        [UIHint("Energy")]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.CharacterEnergyLabel)]
        public NineStarKiEnergy CharacterEnergy { get; set; }

        [UIHint("Energy")]
        [Required(ErrorMessageResourceType = typeof(K9.Base.Globalisation.Dictionary), ErrorMessageResourceName = K9.Base.Globalisation.Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.RisingEnergyLabel)]
        public NineStarKiEnergy RisingEnergy { get; set; }


    }
}