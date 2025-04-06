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
        public string HourlyDescriptionName { get; set; }

        public string GlobalEightyOneYearDescriptionName { get; set; }
        public string GlobalNineYearDescriptionName { get; set; }
        public string GlobalYearlyDescriptionName { get; set; }
        public string GlobalMonthlyDescriptionName { get; set; }
        public string GlobalDailyDescriptionName { get; set; }
        public string GlobalHourlyDescriptionName { get; set; }

        private string _descriptiveTitle;
        private string _seasonDescription;

        private string _eightyOneYearDescription;
        private string _nineYearDescription;
        private string _yearlyDescription;
        private string _monthlyDescription;
        private string _dailyDescription;
        private string _hourlyDescription;

        private string _globalEightyOneYearDescription;
        private string _globalNineYearDescription;
        private string _globalYearlyDescription;
        private string _globalMonthlyDescription;
        private string _globalDailyDescription;
        private string _globalHourlyDescription;

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

        public string HourlyDescription
        {
            get
            {
                if (_hourlyDescription == null)
                {
                    _hourlyDescription = GetResourceValue(HourlyDescriptionName);
                }
                return _hourlyDescription;
            }
        }

        public string GlobalEightyOneYearDescription
        {
            get
            {
                if (_globalEightyOneYearDescription == null)
                {
                    _globalEightyOneYearDescription = GetResourceValue(GlobalEightyOneYearDescriptionName);
                }
                return _globalEightyOneYearDescription;
            }
        }

        public string GlobalNineYearDescription
        {
            get
            {
                if (_globalNineYearDescription == null)
                {
                    _globalNineYearDescription = GetResourceValue(GlobalNineYearDescriptionName);
                }
                return _globalNineYearDescription;
            }
        }

        public string GlobalYearlyDescription
        {
            get
            {
                if (_globalYearlyDescription == null)
                {
                    _globalYearlyDescription = GetResourceValue(GlobalYearlyDescriptionName);
                }
                return _globalYearlyDescription;
            }
        }

        public string GlobalMonthlyDescription
        {
            get
            {
                if (_globalMonthlyDescription == null)
                {
                    _globalMonthlyDescription = GetResourceValue(GlobalMonthlyDescriptionName);
                }
                return _globalMonthlyDescription;
            }
        }

        public string GlobalDailyDescription
        {
            get
            {
                if (_globalDailyDescription == null)
                {
                    _globalDailyDescription = GetResourceValue(GlobalDailyDescriptionName);
                }
                return _globalDailyDescription;
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
