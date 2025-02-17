using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.SharedLibrary.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(K9.Globalisation.Dictionary), ListName = Globalisation.Strings.Names.Contacts, PluralName = Globalisation.Strings.Names.Contacts, Name = Globalisation.Strings.Names.Contact)]
    public class Contact : ObjectBase
	{
	    [ForeignKey("User")]
	    public int? UserId { get; set; }

	    public virtual User User { get; set; }

	    [LinkedColumn(LinkedTableName = "User", LinkedColumnName = "Username")]
	    public string UserName { get; set; }

	    [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.StripeCustomerIdLabel)]
	    public string StripeCustomerId { get; set; }

	    [Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
	    [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.NameLabel)]
        [StringLength(128)]
        public string FullName { get; set; }
        
	    [Index(IsUnique = true)]
	    [Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
	    [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.InvalidEmailAddress)]
	    [EmailAddress(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.InvalidEmailAddress)]
	    [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EmailAddressLabel)]
	    [StringLength(255)]
	    public string EmailAddress { get; set; }
        
	    [DataType(DataType.PhoneNumber)]
	    [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PhoneNumberLabel)]
	    [StringLength(255)]
	    public string PhoneNumber { get; set; }

	    [Display(ResourceType = typeof(K9.Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.CompanyLabel)]
	    [StringLength(255)]
	    public string CompanyName { get; set; }
        
	    [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.IsUnsubscribedLabel)]
        public bool IsUnsubscribed { get; set; }

	    [Display(ResourceType = typeof(Globalisation.Dictionary),
	        Name = Globalisation.Strings.Labels.AreMarketingEmailsAllowedLabel)]
	    public bool AreMarketingEmailsAllowed => !IsUnsubscribed;

	    public string FirstName => FullName.Split(' ').FirstOrDefault();

	}
}
