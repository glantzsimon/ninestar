using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Configuration;
using System.Reflection;
using System.Security.Principal;

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
		public static void MapTo(this object newObject, object objectToUpdate, bool updatePrimaryKey)
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

		public static bool IsVirtualCollection(this PropertyInfo info)
		{
			return info.GetGetMethod().IsVirtual && info.PropertyType.IsGenericType &&
				   info.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>);
		}

		public static bool IsVirtual(this PropertyInfo info)
		{
			return info.GetGetMethod().IsVirtual;
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
			return !info.GetCustomAttributes(typeof(NotMappedAttribute), false).Any() && info.CanWrite;
		}

	}
}
