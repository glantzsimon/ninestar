using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public class AverageBiorhythm : BiorhythmBase
    {
        public override string Name => "Average";
        public override int CycleLength => 0;
        public override EBiorhythm Biorhythm { get; } = EBiorhythm.Average;
        public override string Color => "128, 128, 128";
        public override int LineWidth => 3;
        public override int Index => 6;
    }
}