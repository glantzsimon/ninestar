using System;
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
			return ResourceType.GetValueFromResource(IndefiniteArticleName);
		}

		public string GetDefiniteArticle()
		{
			return ResourceType.GetValueFromResource(DefiniteArticleName);
		}

		public string GetOfPreposition()
		{
			return ResourceType.GetValueFromResource(OfPrepositionName);
		}
	}
}
