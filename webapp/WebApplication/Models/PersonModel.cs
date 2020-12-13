using K9.Base.DataAccessLayer.Extensions;
using K9.Base.Globalisation;
using System;
using System.ComponentModel.DataAnnotations;
using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public class PersonModel
    {
        [UIHint("Gender")]
        [Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.GenderLabel)]
        public EGender Gender { get; set; }

        [Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.DateOfBirthLabel)]
        public DateTime DateOfBirth { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.LanguageLabel)]
        public string GenderName => Gender.GetLocalisedLanguageName();
    }
}