
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9.Globalisation;

namespace K9.DataAccess.Models
{
	public class User : ObjectBase
	{
		[StringLength(56)]
		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.UserNameLabel)]
		public string Username { get; set; }
		
		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.FirstNameLabel)]
		public string FirstName { get; set; }
		
		[NotMapped]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.NameLabel)]
		public new string Name
		{
			get
			{
				return string.Format("{0} {1}", FirstName, LastName);
			}
		}

		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.LastNameLabel)]
		public string LastName { get; set; }

		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		[DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.InvalidEmailAddress)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EmailAddressLabel)]
		[StringLength(255)]
		public string EmailAddress { get; set; }

		[DataType(DataType.PhoneNumber)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PhoneNumberLabel)]
		[StringLength(255)]
		public string PhoneNumber { get; set; }
	}
}
