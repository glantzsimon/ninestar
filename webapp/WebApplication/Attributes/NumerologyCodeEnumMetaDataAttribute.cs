using K9.SharedLibrary.Extensions;
using System;

namespace K9.WebApplication.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NumerologyCodeEnumMetaDataAttribute : Attribute
    {
        public Type ResourceType { get; set; }
        public string NameKey { get; set; }
        public string DescriptionKey { get; set; }
        public string PurposeKey { get; set; }
        public string Colour { get; set; }

        private string _name;
        public string Name => _name ?? (_name = ResourceType.GetValueFromResource(NameKey));

        private string _description;
        public string Description => _description ?? (_description = ResourceType.GetValueFromResource(DescriptionKey));

        private string _purpose;
        public string Purpose => _purpose ?? (_purpose = ResourceType.GetValueFromResource(PurposeKey));
    }

}