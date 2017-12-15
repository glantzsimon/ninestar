using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using System;
using System.ComponentModel.DataAnnotations;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(K9.Globalisation.Dictionary), ListName = Globalisation.Strings.Names.Donations, PluralName = Globalisation.Strings.Names.Donations, Name = Globalisation.Strings.Names.Donation)]
    public class Donation : ObjectBase
	{
	    [Required(ErrorMessageResourceType = typeof(K9.Base.Globalisation.Dictionary), ErrorMessageResourceName = K9.Base.Globalisation.Strings.ErrorMessages.FieldIsRequired)]
        [StringLength(128)]
		[Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.CustomerLabel)]
		public string Customer { get; set; }

	    [Required(ErrorMessageResourceType = typeof(K9.Base.Globalisation.Dictionary), ErrorMessageResourceName = K9.Base.Globalisation.Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.AmountDonatedLabel)]
	    [DataType(DataType.Currency)]
	    public double DonationAmount { get; set; }

	    [Required(ErrorMessageResourceType = typeof(K9.Base.Globalisation.Dictionary), ErrorMessageResourceName = K9.Base.Globalisation.Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.DonatedOnLabel)]
        public DateTime DonatedOn { get; set; }

	    [Required(ErrorMessageResourceType = typeof(K9.Base.Globalisation.Dictionary), ErrorMessageResourceName = K9.Base.Globalisation.Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.CurrencyLabel)]
        public string Currency { get; set; }

	    [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DescriptionLabel)]
        public string DonationDescription { get; set; }

	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.CustomerLabel)]
        public string CustomerEmail { get; set; }

    }
}
