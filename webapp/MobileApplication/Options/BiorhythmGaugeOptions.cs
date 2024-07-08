namespace K9.WebApplication.Options
{
    public class BiorhythmGaugeOptions : GaugeOptions
    {
        public override int MaxValue { get; set; } = 100;

        public BiorhythmGaugeOptions()
        {
            IsSummary = true;
        }
    }
}