using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Models;
using Microsoft.Ajax.Utilities;

namespace K9.SharedLibrary.Extensions
{
	public static class ReflectionExtensions
	{

		/// <summary>
		/// Copies all property values from one object to another
		/// </summary>
		/// <param name="objectToUpdate"></param>
		/// <param name="newObject"></param>
		/// <param name="updatePrimaryKey"></param>        
		public static void MapTo(this object newObject, object objectToUpdate)
		{
			foreach (var propInfo in objectToUpdate.GetType().GetProperties())
			{
				try
				{
					objectToUpdate.SetProperty(propInfo, newObject.GetProperty(propInfo.Name));
				}
				catch (Exception)
				{
				}
			}
		}

		public static List<PropertyInfo> GetProperties(this Object item)
		{
			return item.GetType().GetProperties().ToList();
		}

		/// <summary>
		/// Return a list of properties which are decorated with the specified attribute
		/// </summary>
		/// <param name="item"></param>
		/// <param name="attributeType"></param>
		/// <returns></returns>
		public static List<PropertyInfo> GetPropertiesWithAttribute(this Object item, Type attributeType)
		{
			return (from prop in item.GetType().GetProperties() let attributes = prop.GetCustomAttributes(attributeType, true) where attributes.Any() select prop).ToList();
		}

		public static Dictionary<T, PropertyInfo> GetPropertiesAndAttributesWithAttribute<T>(this IEnumerable<PropertyInfo> propertyInfos) where T : Attribute
		{
			var dictionary = new Dictionary<T, PropertyInfo>();
			propertyInfos.Select(p =>
			{
				var a = p.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
				return new
				{
					Property = p,
					Attribute = a
				};
			}).Where(x => x.Attribute != null)
			.ForEach(_ =>
			{
				dictionary.Add(_.Attribute, _.Property);
			});
			return dictionary;
		}

		public static Dictionary<T, PropertyInfo> GetPropertiesAndAttributesWithAttribute<T>(this Type type) where T : Attribute
		{
			return type.GetProperties().GetPropertiesAndAttributesWithAttribute<T>();
		}

		public static Dictionary<T, PropertyInfo> GetPropertiesAndAttributesWithAttribute<T>(this Object item) where T : Attribute
		{
			return item.GetType().GetProperties().GetPropertiesAndAttributesWithAttribute<T>();
		}

		/// <summary>
		/// Return a list of properties which are decorated with the specified attribute
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="attributeTypes"></param>
		/// <returns></returns>
		public static List<PropertyInfo> GetPropertiesWithAttributes(this List<PropertyInfo> properties, params Type[] attributeTypes)
		{
			var items = new List<PropertyInfo>();
			foreach (var attributeType in attributeTypes)
			{
				items.AddRange(from prop in properties let attributes = prop.GetCustomAttributes(attributeType, true) where attributes.Any() select prop);
			}
			return items.ToList();
		}

		public static object GetProperty(this object obj, string propertyName)
		{
			return obj.GetType().InvokeMember(propertyName, BindingFlags.GetProperty, null, obj, new object[] { });
		}

		public static bool HasProperty(this object obj, string propertyName)
		{
			return obj.GetProperties().Any(p => p.Name == propertyName);
		}

		public static bool HasAttribute(this Type type, Type attributeType)
		{
			return type.GetCustomAttributes(attributeType, true).Any();
		}

		public static void SetProperty(this object obj, string propertyName, object value)
		{
			var propInfo = obj.GetType().GetProperties().FirstOrDefault(p => p.Name == propertyName);
			SetProperty(obj, propInfo, value);
		}

		public static void SetProperty(this object obj, PropertyInfo propertyInfo, object value)
		{
			if (propertyInfo != null)
			{
				object formattedValue;

				// Check if the type is Nullable
				if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					// Get underlying type, e.g. "int"
					formattedValue = value == null ? null : Convert.ChangeType(value, propertyInfo.PropertyType.GetGenericArguments()[0]);
				}
				else
				{
					formattedValue = Convert.ChangeType(value, propertyInfo.PropertyType);
				}

				propertyInfo.SetValue(obj, formattedValue, null);
			}
		}

		public static bool IsPrimaryKey(this PropertyInfo info)
		{
			return info.GetCustomAttributes(typeof(KeyAttribute), false).Any();
		}

		public static bool IsForeignKey(this PropertyInfo info)
		{
			return info.GetCustomAttributes(typeof(ForeignKeyAttribute), false).Any();
		}

		public static bool IsVirtualCollection(this PropertyInfo info)
		{
			return info.GetGetMethod().IsVirtual && info.PropertyType.IsGenericType &&
				   info.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>);
		}

		public static bool IsVirtual(this PropertyInfo info)
		{
			var methodInfo = info.GetGetMethod();
			return methodInfo.IsVirtual && !methodInfo.IsFinal;
		}

		public static int GetStringLength(this PropertyInfo info)
		{
			var attr = info.GetCustomAttributes(typeof(StringLengthAttribute), false).FirstOrDefault();
			if (attr != null)
			{
				return ((StringLengthAttribute)attr).MaximumLength;
			}

			return 0;
		}

		/// <summary>
		/// If the property has a DisplayName attribute, return the value of this, else return the property name
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		public static string GetDisplayName(this PropertyInfo info)
		{
			var attr = info.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault();
			return attr == null ? info.Name : ((DisplayAttribute)attr).GetName();
		}

		public static bool IsDataBound(this PropertyInfo info)
		{
			return (!info.GetCustomAttributes(typeof(NotMappedAttribute), false).Any() || info.GetCustomAttributes(typeof(LinkedColumnAttribute), false).Any()) && info.CanWrite;
		}

		public static Type GetLinkedPropertyType(this Type type, string propertyName)
		{
			return type.GetProperty(propertyName).PropertyType;
		}

		public static string GetLinkedForeignTableName(this Type type, string foreignKeyColumn)
		{
			var firstOrDefault = type.GetProperties().FirstOrDefault(p => p.Name == foreignKeyColumn);
			if (firstOrDefault != null)
			{
				var attribute = firstOrDefault.GetCustomAttributes(typeof(ForeignKeyAttribute), true).FirstOrDefault() as ForeignKeyAttribute;
				if (attribute == null)
				{
					throw new Exception(string.Format("No ForeignKey attribute is set on property {0}", foreignKeyColumn));
				}
				return type.GetLinkedPropertyType(attribute.Name).Name;
			}
			throw new Exception(string.Format("Invalid property name {0}", foreignKeyColumn));
		}

		public static bool LimitedByUser(this Type type)
		{
			return type.GetCustomAttributes(typeof(LimitByUserIdAttribute), true).Any();
		}

		public static bool ImplementsIUserData(this Type type)
		{
			return typeof(IUserData).IsAssignableFrom(type);
		}

		public static string GetForeignKeyName(this Type type)
		{
			return string.Format("{0}Id", type.Name);
		}

		public static T GetAttribute<T>(this Type type)
			where T : Attribute
		{
			return type.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
		}

		public static T GetAttribute<T>(this PropertyInfo propertyInfo)
			where T : Attribute
		{
			return propertyInfo.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
		}

		public static T GetAttribute<T>(this Enum value)
		where T : Attribute
		{
			var type = value.GetType();
			var name = Enum.GetName(type, value);
			return type.GetField(name)
				.GetCustomAttributes(false)
				.OfType<T>()
				.SingleOrDefault();
		}

	}
}
