using System;
using K9.SharedLibrary.Extensions;

namespace K9.DataAccess.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class EnumDescriptionAttribute : Attribute
	{
		public string LanguageCode { get; set; }
		public string Name { get; set; }
		public Type ResourceType { get; set; }

		public string GetDescription()
		{
			return ResourceType.GetValueFromResource(Name);
		}
		
	}
}
