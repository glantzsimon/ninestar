using System;
using K9.SharedLibrary.Extensions;

namespace K9.DataAccess.Attributes
{
	public class NameAttribute : Attribute
	{
		public string Name { get; set; }
		public string PluralName { get; set; }
		public Type ResourceType { get; set; }

		public string GetName()
		{
			return ResourceType.GetValueFromResource(Name);
		}

		public string GetPluralName()
		{
			return ResourceType.GetValueFromResource(PluralName);
		}
	}
}
