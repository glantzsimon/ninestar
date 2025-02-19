using K9.SharedLibrary.Extensions;
using System;

namespace K9.DataAccessLaye.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DiscountAttribute : Attribute
    {
        public string Name { get; set; }
        public int DiscountPercent { get; set; }
        public Type ResourceType { get; set; }
        public string Description => GetDescription();

        public string GetDescription()
        {
            return $"{ResourceType?.GetValueFromResource(Name)} ({DiscountPercent}%)";
        }
    }
}
