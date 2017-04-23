
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.DataAccess.Models
{
	public class Country : ObjectBase
	{
		[Index(IsUnique = true)]
		[StringLength(2)]
		public string TwoLetterCountryCode { get; set; }

		[Index(IsUnique = true)]
		[StringLength(3)]
		public string ThreeLetterCountryCode { get; set; }
	}
}
