using System;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Models
{
    public class BioRhythmResult 
    {
        public BiorhythmBase BioRhythm { get; set; }
        public int DayInterval { get; set; }
        public double Value { get; set; }
        public List<Tuple<string,double>> RangeValues { get; set; }
        public bool IsUpgradeRequired { get; set; }
        
        public double? GetMaxValue() => RangeValues?.Max(e => e.Item2);
        public double? GetMinValue() => RangeValues?.Min(e => e.Item2);
    }
}