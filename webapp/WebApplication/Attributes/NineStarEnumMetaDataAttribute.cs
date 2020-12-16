using K9.Base.DataAccessLayer.Attributes;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Models;
using System;

namespace K9.WebApplication.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NineStarEnumMetaDataAttribute : Attribute
    {
        public ENineStarKiFamilyMember FamilyMember { get; set; }
        public ENineStarKiElenement Element { get; set; }
        public ENineStarKiColour Colour { get; set; }
        public ENineStarKiDirection Direction { get; set; }
        public ENineStarKiYinYang YinYang { get; set; }
        public ENineStarKiDescriptiveName DescriptiveName { get; set; }
        public Type ResourceType { get; set; }
        public string TrigramName { get; set; }
        public string Name { get; set; }

        public string GetDescription()
        {
            return ResourceType.GetValueFromResource(Name);
        }

        public string GetDescriptiveTitle()
        {
            var attr = DescriptiveName.GetAttribute<EnumDescriptionAttribute>();
            return attr.GetDescription();
        }

        public string GetYinYang()
        {
            var attr = YinYang.GetAttribute<EnumDescriptionAttribute>();
            return attr.GetDescription();
        }

        public string GetFamilyMember()
        {
            var attr = FamilyMember.GetAttribute<EnumDescriptionAttribute>();
            return attr.GetDescription();
        }

        public string GetElement()
        {
            var attr = Element.GetAttribute<EnumDescriptionAttribute>();
            return attr.GetDescription();
        }

        public string GetElementDescription()
        {
            switch (Element)
            {
                case ENineStarKiElenement.Earth:
                    return Globalisation.Dictionary.earth_element;
                case ENineStarKiElenement.Fire:
                    return Globalisation.Dictionary.fire_element;
                case ENineStarKiElenement.Metal:
                    return Globalisation.Dictionary.metal_element;
                case ENineStarKiElenement.Tree:
                    return Globalisation.Dictionary.tree_element;
                case ENineStarKiElenement.Water:
                    return Globalisation.Dictionary.water_element;
                default:
                    return string.Empty;
            }
        }

        public string GetColour()
        {
            var attr = Colour.GetAttribute<EnumDescriptionAttribute>();
            return attr.GetDescription();
        }

        public string GetDirection()
        {
            var attr = Direction.GetAttribute<EnumDescriptionAttribute>();
            return attr.GetDescription();
        }

        public string GetTrigram()
        {
            return ResourceType.GetValueFromResource(TrigramName);
        }
        
    }

}