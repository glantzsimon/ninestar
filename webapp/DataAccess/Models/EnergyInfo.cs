using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Extensions;
using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Enums;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using K9.Base.DataAccessLayer.Enums;
using K9.Base.Globalisation;
using K9.DataAccessLayer.Attributes;
using K9.SharedLibrary.Extensions;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(K9.Globalisation.Dictionary), ListName = Globalisation.Strings.Names.Donations, PluralName = Globalisation.Strings.Names.Donations, Name = Globalisation.Strings.Names.Donation)]
    public class EnergyInfo : ObjectBase
	{
        [UIHint("Energy")]
        [Required(ErrorMessageResourceType = typeof(K9.Base.Globalisation.Dictionary), ErrorMessageResourceName = K9.Base.Globalisation.Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.EnergyLabel)]
		public ENineStarEnergy Energy { get; set; }

	    [UIHint("EnergyType")]
	    [Required(ErrorMessageResourceType = typeof(K9.Base.Globalisation.Dictionary), ErrorMessageResourceName = K9.Base.Globalisation.Strings.ErrorMessages.FieldIsRequired)]
	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.EnergyLabel)]
	    public EEnergyType EnergyType { get; set; }

	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.EnergyLabel)]
	    public string EnergyName => MetaData.GetDescription();

	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.EnergyTypeLabel)]
	    public string EnergyTypeName => EnergyType.GetLocalisedLanguageName();

        [AllowHtml]
	    [DataType(DataType.Html)]
        [StringLength(int.MaxValue)]
        [Required(ErrorMessageResourceType = typeof(K9.Base.Globalisation.Dictionary), ErrorMessageResourceName = K9.Base.Globalisation.Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.TrigramLabel)]
        public string Trigram { get; set; }

	    [AllowHtml]
	    [DataType(DataType.Html)]
        [StringLength(int.MaxValue)]
	    [Required(ErrorMessageResourceType = typeof(K9.Base.Globalisation.Dictionary), ErrorMessageResourceName = K9.Base.Globalisation.Strings.ErrorMessages.FieldIsRequired)]
	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.EnergyDescriptionLabel)]
	    public string EnergyDescription { get; set; }

	    [AllowHtml]
	    [DataType(DataType.Html)]
        [StringLength(int.MaxValue)]
	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.ChildhoodLabel)]
	    public string Childhood { get; set; }

	    [AllowHtml]
	    [DataType(DataType.Html)]
        [StringLength(int.MaxValue)]
	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.HealthLabel)]
	    public string Health { get; set; }

	    [AllowHtml]
	    [DataType(DataType.Html)]
        [StringLength(int.MaxValue)]
	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.OccupationsLabel)]
	    public string Occupations { get; set; }

	    [AllowHtml]
	    [DataType(DataType.Html)]
        [StringLength(int.MaxValue)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.PersonalDevelopemntLabel)]
        public string PersonalDevelopemnt { get; set; }

	    [AllowHtml]
	    [DataType(DataType.Html)]
        [StringLength(int.MaxValue)]
	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.ExamplesLabel)]
	    public string Examples { get; set; }

	    [UIHint("Language")]
	    [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.LanguageLabel)]
	    [Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
	    public ELanguage Language { get; set; }

	    [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.LanguageLabel)]
	    public string LanguageName => Language.GetLocalisedLanguageName();

        public NineStarKiEnergy NineStarKiEnergy => new NineStarKiEnergy(Energy);

	    private NineStarEnumMetaDataAttribute MetaData => Energy.GetAttribute<NineStarEnumMetaDataAttribute>();
    }
}
