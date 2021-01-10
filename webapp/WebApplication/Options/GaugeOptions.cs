using K9.WebApplication.Enums;

namespace K9.WebApplication.Options
{
    public class GaugeOptions
    {
        public int Value { get; set; }
        public int MaxValue { get; set; } = (int)ECompatibilityScore.ExtremelyHigh;
        public bool IsInverted { get; set; }
        public bool IsSecret { get; set; }
    }
}