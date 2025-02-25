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

        private string _descriptiveTitle;
        private string _yearlyDescription;
        private string _monthlyDescription;
        private string _seasonDescription;

        public string DescriptiveTitle
        {
            get
            {
                if (_descriptiveTitle == null)
                {
                    _descriptiveTitle = GetEnumDescription(DescriptiveName);
                }
                return _descriptiveTitle;
            }
        }

        public string YearlyDescription
        {
            get
            {
                if (_yearlyDescription == null)
                {
                    _yearlyDescription = GetResourceValue(YearlyDescriptionName);
                }
                return _yearlyDescription;
            }
        }

        public string MonthlyDescription
        {
            get
            {
                if (_monthlyDescription == null)
                {
                    _monthlyDescription = GetResourceValue(MonthlyDescriptionName);
                }
                return _monthlyDescription;
            }
        }

        public string SeasonDescription
        {
            get
            {
                if (_seasonDescription == null)
                {
                    _seasonDescription = GetResourceValue(Season);
                }
                return _seasonDescription;
            }
        }

        private string GetEnumDescription(Enum value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var attr = value.GetAttribute<EnumDescriptionAttribute>();
            return attr != null ? attr.GetDescription() : string.Empty;
        }

        private string GetResourceValue(string resourceKey)
        {
            return !string.IsNullOrEmpty(resourceKey) ? ResourceType.GetValueFromResource(resourceKey) : string.Empty;
        }
    }
}
