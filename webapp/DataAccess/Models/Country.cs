
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9.DataAccess.Attributes;
using K9.Globalisation;

namespace K9.DataAccess.Models
{
	[Grammar(ResourceType = typeof(Dictionary), DefiniteArticleName = Strings.Grammar.CountryDefiniteArticle, IndefiniteArticleName = Strings.Grammar.CountryIndefiniteArticle)]
	[Name(ResourceType = typeof(Dictionary), Name = Strings.Names.Country, PluralName = Strings.Names.Countries, ListName = "Countries")]
	public class Country : ObjectBase
	{
		[Index(IsUnique = true)]
		[StringLength(2)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.TwoLetterCountryCodeLabel)]
		public string TwoLetterCountryCode { get; set; }

		[Index(IsUnique = true)]
		[StringLength(3)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ThreeLetterCountryCodeLabel)]
		public string ThreeLetterCountryCode { get; set; }
	}
}
