using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public class IntuitiveBiorhythm : BiorhythmBase
    {
        public override string Name => "Intuitive";
        public override int CycleLength => 38;
        public override EBiorhythm Biorhythm { get; } = EBiorhythm.Intuitive;
        public override string Color => "102, 102, 255";
        public override int Index => 6;
        public override int DisplayIndex => 5;
    }
}