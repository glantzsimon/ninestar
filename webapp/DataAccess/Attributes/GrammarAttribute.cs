using System;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;

namespace K9.DataAccess.Attributes
{
	public class GrammarAttribute : Attribute
	{
		public string IndefiniteArticleName { get; set; }
		public string DefiniteArticleName { get; set; }
		public string OfPrepositionName { get; set; }
		public Type ResourceType { get; set; }

		public string GetIndefiniteArticle()
		{
			return string.IsNullOrEmpty(IndefiniteArticleName) ? Dictionary.MasculineIndefiniteArticle : ResourceType.GetValueFromResource(IndefiniteArticleName);
		}

		public string GetDefiniteArticle()
		{
			return string.IsNullOrEmpty(DefiniteArticleName) ? Dictionary.MasculineDefiniteArticle : ResourceType.GetValueFromResource(DefiniteArticleName);
		}

		public string GetOfPreposition()
		{
			return string.IsNullOrEmpty(OfPrepositionName) ? Dictionary.OfPreposition : ResourceType.GetValueFromResource(OfPrepositionName);
		}
	}
}
