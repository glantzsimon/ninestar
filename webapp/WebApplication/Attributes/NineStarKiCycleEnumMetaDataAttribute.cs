using K9.Base.DataAccessLayer.Attributes;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Models;
using System;

namespace K9.WebApplication.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NineStarKiCycleEnumMetaDataAttribute : Attribute
    {
        public ENineStarKiElement Element { get; set; }
        public ENineStarKiCycleDescriptiveName DescriptiveName { get; set; }
        public Type ResourceType { get; set; }
        public string Season { get; set; }
        public string YearlyDescriptionName { get; set; }
        public string MonthlyDescriptionName { get; set; }

        public string DescriptiveTitle => GetDescriptiveTitle();
        public string YearlyDescription => GetYearlyDescription();
        public string MonthlyDescription => GetMonthlyDescription();
        public string SeasonDescription => GetSeason();
        
        public string GetDescriptiveTitle()
        {
            var attr = DescriptiveName.GetAttribute<EnumDescriptionAttribute>();
            return attr.GetDescription();
        }
       
        public string GetSeason()
        {
            return !string.IsNullOrEmpty(Season) ? ResourceType.GetValueFromResource(Season) : string.Empty;
        }

        public string GetYearlyDescription()
        {
            return ResourceType.GetValueFromResource(YearlyDescriptionName);
        }

        public string GetMonthlyDescription()
        {
            return ResourceType.GetValueFromResource(MonthlyDescriptionName);
        }
    }
}