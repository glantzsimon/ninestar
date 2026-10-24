using K9.Base.DataAccessLayer.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace K9.WebApplication.Helpers
{
    public static class MobileMetadataHelper
    {
        public static object EnumOptions(Type type)
        {
            if (!type.IsEnum) return null;
            return Enum.GetValues(type).Cast<object>().Select(value => new
            {
                Value = value.ToString(), Number = Convert.ToInt32(value),
                Label = type.GetField(value.ToString()).GetCustomAttribute<EnumDescriptionAttribute>()?.GetDescription()
            }).ToArray();
        }
    }
}
