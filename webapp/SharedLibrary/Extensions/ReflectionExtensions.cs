using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace K9.SharedLibrary.Extensions
{
	public static class Extensions
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

		public static PropertyInfo[] GetProperties(this Object item)
		{
			return item.GetType().GetProperties();
		}

		/// <summary>
		/// Return a list of properties which are decorated with the specified attribute
		/// </summary>
		/// <param name="item"></param>
		/// <param name="attributeType"></param>
		/// <returns></returns>
		public static PropertyInfo[] GetPropertiesWithAttribute(this Object item, Type attributeType)
		{
			var list = new List<PropertyInfo>();
			foreach (var prop in item.GetType().GetProperties())
			{
				object[] attributes = prop.GetCustomAttributes(attributeType, true);
				if (attributes.Any())
				{
					list.Add(prop);
				}
			}
			return list.ToArray();
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
					if (value == null)
					{
						formattedValue = null;
					}
					else
					{
						formattedValue = Convert.ChangeType(value, propertyInfo.PropertyType.GetGenericArguments()[0]);
					}
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
			return info.GetCustomAttributes(typeof(KeyAttribute), false).Count() == 1;
		}

		public static int GetStringLength(this PropertyInfo info)
		{
			var attr = info.GetCustomAttributes(typeof(StringLengthAttribute), false).FirstOrDefault();
			if (attr != null)
			{
				return ((StringLengthAttribute)(attr)).MaximumLength;
			}

			return 0;
		}

	}
}
