
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
			var articlesAttribute = type.GetCustomAttributes(typeof(GrammarAttribute), true).FirstOrDefault() as GrammarAttribute;
			return articlesAttribute == null ? typeof(Dictionary).GetValueFromResource(Strings.Grammar.MasculineDefiniteArticle) : articlesAttribute.GetDefiniteArticle();
		}

		public static string GetIndefiniteArticle(this Type type)
		{
			var articlesAttribute = type.GetCustomAttributes(typeof(GrammarAttribute), true).FirstOrDefault() as GrammarAttribute;
			return articlesAttribute == null ? typeof(Dictionary).GetValueFromResource(Strings.Grammar.MasculineDefiniteArticle) : articlesAttribute.GetIndefiniteArticle();
		}

		public static string GetDefiniteArticle(this PropertyInfo info)
		{
			var articlesAttribute = info.GetCustomAttributes(typeof(GrammarAttribute), true).FirstOrDefault() as GrammarAttribute;
			return articlesAttribute == null ? typeof(Dictionary).GetValueFromResource(Strings.Grammar.MasculineDefiniteArticle) : articlesAttribute.GetDefiniteArticle();
		}

		public static string GetIndefiniteArticle(this PropertyInfo info)
		{
			var articlesAttribute = info.GetCustomAttributes(typeof(GrammarAttribute), true).First() as GrammarAttribute;
			return articlesAttribute == null ? typeof(Dictionary).GetValueFromResource(Strings.Grammar.MasculineDefiniteArticle) : articlesAttribute.GetIndefiniteArticle();
		}

		public static string GetOfPreposition(this PropertyInfo info)
		{
			var articlesAttribute = info.GetCustomAttributes(typeof(GrammarAttribute), true).First() as GrammarAttribute;
			return articlesAttribute == null ? typeof(Dictionary).GetValueFromResource(Strings.Grammar.OfPreposition) : articlesAttribute.GetOfPreposition();
		}

		public static string GetOfPreposition(this Type type)
		{
			var articlesAttribute = type.GetCustomAttributes(typeof(GrammarAttribute), true).First() as GrammarAttribute;
			return articlesAttribute == null ? typeof(Dictionary).GetValueFromResource(Strings.Grammar.OfPreposition) : articlesAttribute.GetOfPreposition();
		}

		public static string GetName(this Type type)
		{
			var namettribute = type.GetCustomAttributes(typeof(NameAttribute), true).FirstOrDefault() as NameAttribute;
			return namettribute == null ? type.Name : namettribute.GetName();
		}

		public static string GetPluralName(this Type type)
		{
			var namettribute = type.GetCustomAttributes(typeof(NameAttribute), true).FirstOrDefault() as NameAttribute;
			return namettribute == null ? string.Format("{0}s", type.Name) : namettribute.GetPluralName();
		}

		public static string GetListName(this Type type)
		{
			var namettribute = type.GetCustomAttributes(typeof(NameAttribute), true).FirstOrDefault() as NameAttribute;
			return namettribute == null ? string.Format("{0}s", type.Name) : namettribute.GetPluralName();
		}

	}
}