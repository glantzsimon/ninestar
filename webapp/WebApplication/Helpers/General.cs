using K9.Base.DataAccessLayer.Enums;
using System;

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
    }
}