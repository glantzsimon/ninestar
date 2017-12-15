using System;

namespace K9.WebApplication.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NineStarEnumMetaDataAttribute
    {
        public string Symbol { get; set; }
        public string FamilyMember { get; set; }
        public string Element { get; set; }
        public string Colour { get; set; }
    }
}