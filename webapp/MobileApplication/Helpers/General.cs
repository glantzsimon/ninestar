using K9.Base.DataAccessLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
    }
}