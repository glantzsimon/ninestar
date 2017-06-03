using System;
using K9.SharedLibrary.Extensions;

namespace K9.DataAccess.Attributes
{
	public class ArticlesAttribute : Attribute
	{
		public string IndefiniteArticleName { get; set; }
		public string DefiniteArticleName { get; set; }
		public Type ResourceType { get; set; }

		public string GetIndefiniteArticle()
		{
			return ResourceType.GetValueFromResource(IndefiniteArticleName);
		}

		public string GetDefiniteArticle()
		{
			return ResourceType.GetValueFromResource(DefiniteArticleName);
		}
	}
}
