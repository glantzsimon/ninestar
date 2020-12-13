using K9.WebApplication.Enums;
using System.Collections.Generic;

namespace K9.WebApplication.Extensions
{
    public static class Extensions
    {
        public static bool IsYin(this EGender gender)
        {
            return new List<EGender>
            {
                EGender.Female,
                EGender.TransFemale,
                EGender.Hermaphrodite
            }.Contains(gender);
        }
    }
}
