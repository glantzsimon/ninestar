
using System;
using System.Linq;
using System.Reflection;
using K9.DataAccess.Attributes;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;

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
			return string.Format("/{0}/List", type.GetPluralName());
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

		public static string GetDefiniteArticle(this Type type)
		{
			var articlesAttribute = type.GetCustomAttributes(typeof(ArticlesAttribute), true).FirstOrDefault() as ArticlesAttribute;
			return articlesAttribute == null ? typeof(Dictionary).GetValueFromResource(Strings.Articles.MasculineDefiniteArticle) : articlesAttribute.GetDefiniteArticle();
		}

		public static string GetIndefiniteArticle(this Type type)
		{
			var articlesAttribute = type.GetCustomAttributes(typeof(ArticlesAttribute), true).FirstOrDefault() as ArticlesAttribute;
			return articlesAttribute == null ? typeof(Dictionary).GetValueFromResource(Strings.Articles.MasculineDefiniteArticle) : articlesAttribute.GetIndefiniteArticle();
		}

		public static string GetDefiniteArticle(this PropertyInfo info)
		{
			var articlesAttribute = info.GetCustomAttributes(typeof(ArticlesAttribute), true).FirstOrDefault() as ArticlesAttribute;
			return articlesAttribute == null ? typeof(Dictionary).GetValueFromResource(Strings.Articles.MasculineDefiniteArticle) : articlesAttribute.GetDefiniteArticle();
		}

		public static string GetIndefiniteArticle(this PropertyInfo info)
		{
			var articlesAttribute = info.GetCustomAttributes(typeof(ArticlesAttribute), true).First() as ArticlesAttribute;
			return articlesAttribute == null ? typeof(Dictionary).GetValueFromResource(Strings.Articles.MasculineDefiniteArticle) : articlesAttribute.GetIndefiniteArticle();
		}

		public static string GetName(this Type type)
		{
			var namettribute = type.GetCustomAttributes(typeof(NameAttribute), true).FirstOrDefault() as NameAttribute;
			return namettribute == null ? type.Name : namettribute.GetName();
		}

		public static string GetPluralName(this Type type)
		{
			var namettribute = type.GetCustomAttributes(typeof(NameAttribute), true).FirstOrDefault() as NameAttribute;
			return namettribute == null ? string.Format("{0}s", type.Name) : namettribute.PluralName;
		}

	}
}