using K9.Base.DataAccessLayer.Attributes;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Enums;
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
        
        public string EightyOneYearDescriptionName { get; set; }
        public string NineYearDescriptionName { get; set; }
        public string YearlyDescriptionName { get; set; }
        public string MonthlyDescriptionName { get; set; }
        public string DailyDescriptionName { get; set; }

        private string _descriptiveTitle;
        private string _eightyOneYearDescription;
        private string _nineYearDescription;
        private string _yearlyDescription;
        private string _monthlyDescription;
        private string _dailyDescription;
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

        public string EightyOneYearDescription
        {
            get
            {
                if (_eightyOneYearDescription == null)
                {
                    _eightyOneYearDescription = GetResourceValue(EightyOneYearDescriptionName);
                }
                return _eightyOneYearDescription;
            }
        }

        public string NineYearDescription
        {
            get
            {
                if (_nineYearDescription == null)
                {
                    _nineYearDescription = GetResourceValue(NineYearDescriptionName);
                }
                return _nineYearDescription;
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

        public string DailyDescription
        {
            get
            {
                if (_dailyDescription == null)
                {
                    _dailyDescription = GetResourceValue(DailyDescriptionName);
                }
                return _dailyDescription;
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
