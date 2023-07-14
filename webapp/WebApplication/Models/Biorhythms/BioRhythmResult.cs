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
        public List<Tuple<string, double, DateTime>> RangeValues { get; set; }
        public bool IsUpgradeRequired { get; set; }

        public double? GetMaxValue() => RangeValues?.Max(e => e.Item2);
        public double? GetMinValue() => RangeValues?.Min(e => e.Item2);

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
            var selectedItem = RangeValues.FirstOrDefault(e => e.Item3 == SelectedDate);
            var previousItem = RangeValues.FirstOrDefault(e => e.Item3 == SelectedDate.AddDays(-1));
            var nextItem = RangeValues.FirstOrDefault(e => e.Item3 == SelectedDate.AddDays(1));

            if (selectedItem == null)
            {
                throw new Exception("Invalid date");
            }

            if (nextItem != null)
            {
                if (nextItem.Item2 > selectedItem.Item2)
                    return EBiorhythmTrend.Rising;

                if (nextItem.Item2 < selectedItem.Item2)
                    return EBiorhythmTrend.Falling;

                if (nextItem.Item2 == selectedItem.Item2 && selectedItem.Item2 > 50)
                    return EBiorhythmTrend.Maximum;

                if (nextItem.Item2 == selectedItem.Item2 && selectedItem.Item2 < 50)
                    return EBiorhythmTrend.Minimum;

                return EBiorhythmTrend.Undefined;
            }

            if (previousItem != null)
            {
                if (previousItem.Item2 > selectedItem.Item2)
                    return EBiorhythmTrend.Falling;

                if (previousItem.Item2 < selectedItem.Item2)
                    return EBiorhythmTrend.Rising;

                if (previousItem.Item2 == selectedItem.Item2 && selectedItem.Item2 > 50)
                    return EBiorhythmTrend.Maximum;

                if (previousItem.Item2 == selectedItem.Item2 && selectedItem.Item2 < 50)
                    return EBiorhythmTrend.Minimum;
            }

            return EBiorhythmTrend.Undefined;
        }
    }
}