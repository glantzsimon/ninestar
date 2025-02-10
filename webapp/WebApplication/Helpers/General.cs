using K9.Base.DataAccessLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using K9.Base.DataAccessLayer.Attributes;
using K9.SharedLibrary.Extensions;

namespace K9.WebApplication.Helpers
{
    public static class Methods
    {
        public static readonly Random RandomGenerator = new Random();

        public static EGender GetRandomGender()
        {
            var random = RandomGenerator.Next(1, 3);

            if (random == 3)
            {
                random = 2;
            }

            return (EGender)random;
        }

        public static List<Type> GetClassesThatDeriveFrom<T>()
        {
            var results = new List<Type>();
            var types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (var type in types)
            {
                if (type.GetInterfaces().Contains(typeof(T)))
                {
                    results.Add(type);
                }
                else if (type.IsSubclassOf(typeof(T)))
                {
                    results.Add(type);
                }
            }

            return results;
        }

        public static bool TryGetEnumProperty<TEnum>(string input, string propertyName, out TEnum result)
            where TEnum : struct, Enum
        {
            var type = typeof(TEnum);

            foreach (var field in type.GetFields())
            {
                var attribute = field.GetCustomAttribute<EnumDescriptionAttribute>();
                if (attribute != null && attribute.GetProperty(propertyName).ToString().Equals(input, StringComparison.OrdinalIgnoreCase))
                {
                    result = (TEnum)field.GetValue(null);
                    return true;
                }
            }

            result = default(TEnum);
            return false;
        }
    }
}