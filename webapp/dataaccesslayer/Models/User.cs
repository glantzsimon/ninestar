
using System.ComponentModel.DataAnnotations;
using K9.Globalisation;

namespace K9.DataAccess.Models
{
	public class User : ObjectBase
	{
		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		public string Username { get; set; }
		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		public string FirstName { get; set; }
		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		public string LastName { get; set; }
		[DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.InvalidEmailAddress)]
		public string EmailAddress { get; set; }
	}
}
