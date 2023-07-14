using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public class IntellectualBiorhythm : BiorhythmBase
    {
        public override string Name => "Intellectual";
        public override int CycleLength => 33;
        public override EBiorhythm Biorhythm { get; } = EBiorhythm.Intellectual;
        public override string Color => "44, 175, 254";
        public override int Index => 1;
    }
}