using K9.WebApplication.Enums;

namespace K9.WebApplication.Options
{
    public class GaugeOptions
    {
        public string Title { get; set; }
        public int Value { get; set; }
        public virtual int MaxValue { get; set; } = (int)ECompatibilityScore.ExtremelyHigh;
        public bool IsInverted { get; set; }
        public bool IsSecret { get; set; }
        public bool IsSummary { get; set; }
        public string BackgroundColor { get; set; }
        public string StrokeColor { get; set; }
        public string Units { get; set; }
    }
}