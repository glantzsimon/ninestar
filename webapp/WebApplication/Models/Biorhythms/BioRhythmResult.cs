using System;
using System.Collections.Generic;

namespace K9.WebApplication.Models
{
    public class BioRhythmResult 
    {
        public BiorhythmBase BioRhythm { get; set; }
        public int DayInterval { get; set; }
        public double Value { get; set; }
        public List<Tuple<DateTime,double>> RangeValues { get; set; }
    }
}