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
        public List<RangeValue> LongRangeValues { get; set; }

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
        public double? GetLongMaxValue() => LongRangeValues?.Max(e => e.Value);
        public double? GetLongMinValue() => LongRangeValues?.Min(e => e.Value);

        public int GetDaysUntilNextMaximum() => (int)LongRangeValues?.FirstOrDefault(e => e.Value == GetLongMaxValue() && e.Date > SelectedDate).Date.Value.Subtract(SelectedDate).TotalDays;

        public int GetDaysUntilNextMinimum() => (int)LongRangeValues?.FirstOrDefault(e => e.Value == GetLongMinValue() && e.Date > SelectedDate).Date.Value.Subtract(SelectedDate).TotalDays;

        public int GetDaysUntilNextCritical() => (int)LongRangeValues?.FirstOrDefault(e => GetValueLevel(e.Value) == EBiorhythmLevel.Critical && e.Date > SelectedDate).Date.Value.Subtract(SelectedDate).TotalDays;

        public string GetDaysUntilNextMaximumString() => BioRhythm.Biorhythm == EBiorhythm.Average
            ? "N/A"
            : GetDaysUntilNextMaximum().ToString();

        public string GetDaysUntilNextMinimumString() => BioRhythm.Biorhythm == EBiorhythm.Average 
            ? "N/A" 
            : GetDaysUntilNextMinimum().ToString();

        public string GetDaysUntilNextCriticalString() => BioRhythm.Biorhythm == EBiorhythm.Average 
            ? "N/A" 
            : GetDaysUntilNextCritical().ToString();

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

        public string GetBiorhythmTrendDescription()
        {
            switch (GetBiorhythmTrend())
            {
                case EBiorhythmTrend.Rising:
                    return "Rising";

                case EBiorhythmTrend.Falling:
                    return "Falling";

                case EBiorhythmTrend.Maximum:
                    return "Maximum";

                case EBiorhythmTrend.Minimum:
                    return "Minimum";
            }

            return "";
        }

        public EBiorhythmTrend  GetBiorhythmTrend()
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

        public string GetValueLevelDescription(double value)
        {
            switch (GetValueLevel(value))
            {
                case EBiorhythmLevel.Critical:
                    return Globalisation.Dictionary.Unpredictable;

                case EBiorhythmLevel.ExtremelyLow:
                    return Globalisation.Dictionary.ExtremelyLow;

                case EBiorhythmLevel.VeryLow:
                    return Globalisation.Dictionary.VeryLow;

                case EBiorhythmLevel.Low:
                    return Globalisation.Dictionary.Low;

                case EBiorhythmLevel.Moderate:
                    return Globalisation.Dictionary.Moderate;

                case EBiorhythmLevel.High:
                    return Globalisation.Dictionary.High;

                case EBiorhythmLevel.VeryHigh:
                    return Globalisation.Dictionary.VeryHigh;

                case EBiorhythmLevel.Excellent:
                    return Globalisation.Dictionary.Excellent;
            }
            return Globalisation.Dictionary.Unavailable;
        }

        public EBiorhythmLevel GetValueLevel(double value)
        {
            var max = GetMaxValue();
            var min = GetMinValue();
            var range = max - min;
            var tength = (range / 10);

            if (BioRhythm.Biorhythm != EBiorhythm.Average)
            {
                if (value >= (tength * 4) + min && value < (tength * 6) + min)
                {
                    return EBiorhythmLevel.Critical;
                }
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
                value.LevelDescription = GetValueLevelDescription(value.Value);
            }
        }
    }
}