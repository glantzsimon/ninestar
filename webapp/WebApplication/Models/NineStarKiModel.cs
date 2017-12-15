using K9.Base.DataAccessLayer.Extensions;
using K9.Base.Globalisation;
using System;
using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.Models
{


    public class NineStarKiModel
    {
        public PersonModel PersonModel { get; set; }

        [UIHint("Gender")]
        [Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.GenderLabel)]
        public EGender Gender { get; set; }

        [Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.DateOfBirthLabel)]
        public DateTime DateOfBirth { get; set; }

        
    }
}