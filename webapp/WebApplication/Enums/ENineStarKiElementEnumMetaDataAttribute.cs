using K9.Base.DataAccessLayer.Attributes;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Enums;
using System;

namespace K9.WebApplication.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ENineStarKiElementEnumMetaDataAttribute : Attribute
    {
        public Type ResourceType { get; set; }
        public string Name { get; set; }
        public ENineStarKiElement Element { get; set; }
        public ESeason Season { get; set; }
        public EOrgan StrongYinOrgans { get; set; }
        public EOrgan StrongYangOrgans { get; set; }
        public EOrgan[] WeakYinOrgans { get; set; }
        public EOrgan[] WeakYangOrgans { get; set; }
    
        public string GetDescription()
        {
            return ResourceType.GetValueFromResource(Name);
        }

        public string GetElement()
        {
            return Element.GetAttribute<EnumDescriptionAttribute>().GetDescription();
        }
    }
}