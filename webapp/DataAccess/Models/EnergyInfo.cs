using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Extensions;
using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Enums;
using System.ComponentModel.DataAnnotations;

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
	    public string EnergyName => Energy.GetLocalisedLanguageName();

	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.EnergyTypeLabel)]
	    public string EnergyTypeName => EnergyType.GetLocalisedLanguageName();

	    [StringLength(int.MaxValue)]
	    [DataType(DataType.MultilineText)]
        [Required(ErrorMessageResourceType = typeof(K9.Base.Globalisation.Dictionary), ErrorMessageResourceName = K9.Base.Globalisation.Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.TrigramLabel)]
        public string Trigram { get; set; }

	    [StringLength(int.MaxValue)]
	    [DataType(DataType.MultilineText)]
        [Required(ErrorMessageResourceType = typeof(K9.Base.Globalisation.Dictionary), ErrorMessageResourceName = K9.Base.Globalisation.Strings.ErrorMessages.FieldIsRequired)]
	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.EnergyDescriptionLabel)]
	    public string EnergyDescription { get; set; }

	    [StringLength(int.MaxValue)]
	    [DataType(DataType.MultilineText)]
	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.ChildhoodLabel)]
	    public string Childhood { get; set; }

        [StringLength(int.MaxValue)]
	    [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.EnergyDescriptionLabel)]
	    public string Health { get; set; }

	    [StringLength(int.MaxValue)]
	    [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.EnergyDescriptionLabel)]
	    public string Occupations { get; set; }

        [StringLength(int.MaxValue)]
        [DataType(DataType.MultilineText)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.PersonalDevelopemntLabel)]
        public string PersonalDevelopemnt { get; set; }

	    [StringLength(int.MaxValue)]
	    [DataType(DataType.MultilineText)]
	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.ExamplesLabel)]
	    public string Examples { get; set; }

    }
}
