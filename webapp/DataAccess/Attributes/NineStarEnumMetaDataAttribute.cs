﻿using K9.Base.DataAccessLayer.Attributes;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;
using System;

namespace K9.DataAccessLayer.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NineStarEnumMetaDataAttribute : Attribute
    {
        public ENineStarKiFamilyMember FamilyMember { get; set; }
        public ENineStarKiElenement Element { get; set; }
        public ENineStarKiColour Colour { get; set; }
        public ENineStarKiDirection Direction { get; set; }
        public ENineStarKiYinYang YinYang { get; set; }
        public Type ResourceType { get; set; }
        public string TrigramName { get; set; }
        public string Name { get; set; }

        public string GetDescription()
        {
            return ResourceType.GetValueFromResource(Name);
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