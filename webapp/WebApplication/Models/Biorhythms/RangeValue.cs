using System;

namespace K9.WebApplication.Models
{
    public class RangeValue
    {
        public DateTime? Date { get; set; }
        public double Value {get; set; }
        public string LevelDescription { get; set; }
        public string FormattedDate => Date?.ToString(Constants.FormatConstants.SessionDateTimeFormat);

        public RangeValue(DateTime? date, double value, string levelDescription = "")
        {
            Date = date;
            Value = value;
            LevelDescription = levelDescription;
        }
    }
}