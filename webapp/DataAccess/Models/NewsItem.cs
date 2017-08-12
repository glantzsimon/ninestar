
using System;
using System.ComponentModel.DataAnnotations;
using K9.DataAccess.Attributes;
using K9.DataAccess.Enums;
using K9.DataAccess.Extensions;
using K9.Globalisation;
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Enums;
using K9.SharedLibrary.Models;

namespace K9.DataAccess.Models
{

	[AutoGenerateName]
	[Grammar(ResourceType = typeof(Dictionary), DefiniteArticleName = Strings.Grammar.FeminineDefiniteArticle, IndefiniteArticleName = Strings.Grammar.FeminineIndefiniteArticle)]
	[Name(ResourceType = typeof(Dictionary), Name = Strings.Names.NewsItem)]
	public class NewsItem : ObjectBase
	{
		
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PublishedOnLabel)]
		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		public DateTime PublishedOn { get; set; }

		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PublishedByLabel)]
		public string PublishedBy { get; set; }

		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.LanguageLabel)]
		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		public ELanguage Language { get; set; }

		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SubjectLabel)]
		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		[StringLength(256)]
		public string Subject { get; set; }
		
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.BodyLabel)]
		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		[StringLength(Int32.MaxValue)]
		[DataType(DataType.MultilineText)]
		public string Body { get; set; }

		[FileSourceInfo("Images/news/upload", Filter = EFilesSourceFilter.Images)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Names.UploadImages)]
		public FileSource ImageFileSource { get; set; }

		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.LanguageLabel)]
		public string LanguageName
		{
			get { return Language.GetLocalisedLanguageName(); }
		}

		public string LanguageCode
		{
			get { return Language.GetLanguageCode(); }
		}
	}
}
