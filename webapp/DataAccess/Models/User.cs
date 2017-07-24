
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAnnotationsExtensions;
using K9.DataAccess.Attributes;
using K9.Globalisation;
using K9.SharedLibrary.Models;
using WebMatrix.WebData;

namespace K9.DataAccess.Models
{
	[Grammar(ResourceType = typeof(Dictionary), DefiniteArticleName = Strings.Grammar.DefiniteArticleWithApostrophe, IndefiniteArticleName = Strings.Grammar.MasculineIndefiniteArticle, OfPrepositionName = Strings.Grammar.OfPrepositionWithApostrophe)]
	[Name(ResourceType = typeof(Dictionary), Name = Strings.Names.User)]
	public class User : ObjectBase, IUser
	{
		[StringLength(56)]
		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.UserNameLabel)]
		public string Username { get; set; }

		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.FirstNameLabel)]
		public string FirstName { get; set; }

		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.LastNameLabel)]
		public string LastName { get; set; }

		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		[DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.InvalidEmailAddress)]
		[Email(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.InvalidEmailAddress)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EmailAddressLabel)]
		[StringLength(255)]
		public string EmailAddress { get; set; }

		[DataType(DataType.PhoneNumber)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PhoneNumberLabel)]
		[StringLength(255)]
		public string PhoneNumber { get; set; }

		[DataType(DataType.Date, ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.InvalidDate)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.BirthDateLabel)]
		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		public DateTime BirthDate { get; set; }

		[Display(ResourceType = typeof (Dictionary), Name = Strings.Labels.IsUnsubscribedLabel)]
		public bool IsUnsubscribed{ get; set; }

		[NotMapped]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.AccountActivated)]
		public bool AccountActivated { get; set; }

		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.AccountActivated)]
		public bool IsActivated
		{
			get
			{
				return WebSecurity.Initialized && WebSecurity.IsConfirmed(Username);
			}
		}
		
		public override void UpdateName()
		{
			Name = string.Format("{0} {1}", FirstName, LastName);
		}
	}
}
