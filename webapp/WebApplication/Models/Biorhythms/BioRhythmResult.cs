using System;
using System.Collections.Generic;
using System.Linq;
using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public class BioRhythmResult
    {
        public BiorhythmBase BioRhythm { get; set; }
        public DateTime SelectedDate { get; set; }
        public int DayInterval { get; set; }
        public double Value { get; set; }

        private List<RangeValue> _rangeValues;
        public List<RangeValue> RangeValues
        {
            get => _rangeValues;

            set
            {
                _rangeValues = value;
                UpdateRangeValues();
            }
        }

        public bool IsUpgradeRequired { get; set; }

        public double? GetMaxValue() => RangeValues?.Max(e => e.Value);
        public double? GetMinValue() => RangeValues?.Min(e => e.Value);

        public string GetBiorhythmTrendHtmlString()
        {
            switch (GetBiorhythmTrend())
            {
                case EBiorhythmTrend.Rising:
                    return "&neArr;";

                case EBiorhythmTrend.Falling:
                    return "&seArr;";

                case EBiorhythmTrend.Maximum:
                    return "&uArr;";

                case EBiorhythmTrend.Minimum:
                    return "&dArr;";
            }

            return "";
        }

        public EBiorhythmTrend GetBiorhythmTrend()
        {
            var selectedItem = RangeValues.FirstOrDefault(e => e.Date == SelectedDate);
            var previousItem = RangeValues.FirstOrDefault(e => e.Date == SelectedDate.AddDays(-1));
            var nextItem = RangeValues.FirstOrDefault(e => e.Date == SelectedDate.AddDays(1));

            if (selectedItem == null)
            {
                throw new Exception("Invalid date");
            }

            if (nextItem != null)
            {
                if (nextItem.Value > selectedItem.Value)
                    return EBiorhythmTrend.Rising;

                if (nextItem.Value < selectedItem.Value)
                    return EBiorhythmTrend.Falling;

                if (nextItem.Value == selectedItem.Value && selectedItem.Value > 50)
                    return EBiorhythmTrend.Maximum;

                if (nextItem.Value == selectedItem.Value && selectedItem.Value < 50)
                    return EBiorhythmTrend.Minimum;

                return EBiorhythmTrend.Undefined;
            }

            if (previousItem != null)
            {
                if (previousItem.Value > selectedItem.Value)
                    return EBiorhythmTrend.Falling;

                if (previousItem.Value < selectedItem.Value)
                    return EBiorhythmTrend.Rising;

                if (previousItem.Value == selectedItem.Value && selectedItem.Value > 50)
                    return EBiorhythmTrend.Maximum;

                if (previousItem.Value == selectedItem.Value && selectedItem.Value < 50)
                    return EBiorhythmTrend.Minimum;
            }

            return EBiorhythmTrend.Undefined;
        }

        public string GetDataValueLevelDescription(double value)
        {
            switch (GetValueLevel(value))
            {
                case EBiorhythmLevel.Unpredictable:
                    return "Unpredictable (prone to fluctuations)";

                case EBiorhythmLevel.ExtremelyLow:
                    return "Extremely Low";

                case EBiorhythmLevel.VeryLow:
                    return "Very Low";

                case EBiorhythmLevel.Low:
                    return "Low";

                case EBiorhythmLevel.Moderate:
                    return "Moderate";

                case EBiorhythmLevel.High:
                    return "High";

                case EBiorhythmLevel.VeryHigh:
                    return "Very High";

                case EBiorhythmLevel.Excellent:
                    return "Excellent";
            }
            return string.Empty;
        }

        public EBiorhythmLevel GetValueLevel(double value)
        {
            var max = GetMaxValue();
            var min = GetMinValue();
            var range = max - min;
            var tength = (range / 10);

            if (value >= (tength * 4) + min && value < (tength * 6) + min)
            {
                return EBiorhythmLevel.Unpredictable;
            }

            if (value < 10)
            {
                return EBiorhythmLevel.ExtremelyLow;
            }
            if (value < 20)
            {
                return EBiorhythmLevel.VeryLow;
            }
            if (value < 40)
            {
                return EBiorhythmLevel.Low;
            }
            if (value < 60)
            {
                return EBiorhythmLevel.Moderate;
            }
            if (value < 80)
            {
                return EBiorhythmLevel.High;
            }
            if (value < 90)
            {
                return EBiorhythmLevel.VeryHigh;
            }
            if (value <= 100)
            {
                return EBiorhythmLevel.Excellent;
            }

            return EBiorhythmLevel.Undefined;
        }

        private void UpdateRangeValues()
        {
            foreach (var value in _rangeValues)
            {
                value.LevelDescription = GetDataValueLevelDescription(value.Value);
            }
        }
    }
}