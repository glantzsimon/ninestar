
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAnnotationsExtensions;
using K9.Globalisation;

namespace K9.DataAccess.Models
{
	public class UserAccount
	{
		public class LocalPasswordModel
		{
			[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
			[DataType(DataType.Password)]
			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.CurrentPasswordLabel)]
			public string OldPassword { get; set; }

			[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
			[StringLength(100, ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.PasswordMinLengthError, MinimumLength = 8)]
			[DataType(DataType.Password)]
			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.NewPasswordLabel)]
			public string NewPassword { get; set; }

			[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
			[DataType(DataType.Password)]
			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ConfirmNewPasswordLabel)]
			[EqualTo("NewPassword", ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.NewPasswordMatchError)]
			public string ConfirmPassword { get; set; }
		}

		public class LoginModel
		{
			[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.UserNameLabel)]
			public string UserName { get; set; }

			[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
			[DataType(DataType.Password)]
			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PasswordLabel)]
			public string Password { get; set; }

			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.RememberMe)]
			public bool RememberMe { get; set; }
		}

		public class RegisterModel
		{
			[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.UserNameLabel)]
			public string UserName { get; set; }

			[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
			[StringLength(100, ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.PasswordMinLengthError, MinimumLength = 8)]
			[DataType(DataType.Password)]
			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PasswordLabel)]
			public string Password { get; set; }

			[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
			[DataType(DataType.Password)]
			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ConfirmPasswordLabel)]
			[EqualTo("Password", ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.PasswordMatchError)]
			public string ConfirmPassword { get; set; }

			[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
			[DataType(DataType.Text)]
			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.FirstNameLabel)]
			public string FirstName { get; set; }

			[NotMapped]
			public string FullName
			{
				get
				{
					return string.Format("{0} {1}", FirstName, LastName);
				}
			}

			[DataType(DataType.Text)]
			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.LastNameLabel)]
			public string LastName { get; set; }

			[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
			[Email(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.InvalidEmailAddress)]
			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EmailAddressLabel)]
			public string EmailAddress { get; set; }

			[DataType(DataType.PhoneNumber)]
			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PhoneNumberLabel)]
			public string PhoneNumber { get; set; }

			[DataType(DataType.Date)]
			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.BirthDateLabel)]
			public DateTime BirthDate { get; set; }
		}

		public class PasswordResetRequestModel
		{
			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.UserNameLabel)]
			public string UserName { get; set; }

			[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
			[Email(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.InvalidEmailAddress)]
			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EmailAddressLabel)]
			public string EmailAddress { get; set; }
		}

		public class ResetPasswordModel
		{
			[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.UserNameLabel)]
			public string UserName { get; set; }

			[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
			[StringLength(100, ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.PasswordMinLengthError, MinimumLength = 8)]
			[DataType(DataType.Password)]
			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.NewPasswordLabel)]
			public string NewPassword { get; set; }

			[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
			[DataType(DataType.Password)]
			[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ConfirmPasswordLabel)]
			[EqualTo("NewPassword", ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.PasswordMatchError)]
			public string ConfirmPassword { get; set; }

			public string Token { get; set; }
		}

	}
}
