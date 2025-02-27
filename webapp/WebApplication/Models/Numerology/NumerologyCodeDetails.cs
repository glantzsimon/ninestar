using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Attributes;
using K9.WebApplication.Enums;
using System;

namespace K9.WebApplication.Models
{
    public class NumerologyCodeDetails
    {
        public ENumerologyCode NumerologyCode { get; set; }

        public bool IsActive { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool ShowDates { get; set; }

        public string Title { get; set; }

        public EForecastType Type { get; set; }

        public int Offset { get; set; }

        public bool IsCurrent => DateTime.Now.IsBetween(StartDate, EndDate);

        public string IsCurrentCssClass => IsCurrent ? "current" : "";

        public string IsActiveCssTabClass => IsActive ? "active" : "";

        public string IsActiveCssPaneClass => IsActive ? "in active" : "";

        public string Name => Attributes?.Name;

        public string FullName => $"{Dictionary.The} {Name}";

        public string Colour => Attributes?.Colour;

        public string Purpose => Attributes?.Purpose;

        public string Description => Attributes?.Description;

        public int CodeNumber => (int)NumerologyCode;

        private NumerologyCodeEnumMetaDataAttribute _attributes;
        private NumerologyCodeEnumMetaDataAttribute Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = NumerologyCode.GetAttribute<NumerologyCodeEnumMetaDataAttribute>();
                }
                return _attributes;
            }
        }


    }
}