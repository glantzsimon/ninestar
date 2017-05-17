
using System;
using System.Reflection;

namespace K9.WebApplication.Extensions
{
	public static class Extensions
	{

		public static string GetTableName(this Type type)
		{
			return string.Format("{0}Table", type.Name);
		}

		public static string GetDefaultDataUrl(this Type type)
		{
			return string.Format("/{0}s/List", type.Name);
		}

		public static string GetDataTableType(this PropertyInfo property)
		{
			if (property.PropertyType == typeof(int))
			{
				return "num";
			}
			if (property.PropertyType == typeof(DateTime))
			{
				return "date";
			}
			
			return "string";
		}

	}
}