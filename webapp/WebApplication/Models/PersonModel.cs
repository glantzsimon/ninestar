using System;
using System.ComponentModel.DataAnnotations;
using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Extensions;
using K9.Base.Globalisation;

namespace K9.WebApplication.Models
{
    public enum EGender
    {
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = K9.Globalisation.Strings.Names.Female)]
        Female,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = K9.Globalisation.Strings.Names.Male)]
        Male,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = K9.Globalisation.Strings.Names.TransFemale)]
        TransFemale,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = K9.Globalisation.Strings.Names.TransMale)]
        TransMale,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = K9.Globalisation.Strings.Names.Hermaphrodite)]
        Hermaphrodite,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = K9.Globalisation.Strings.Names.Other)]
        Other
    }

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