using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public class SpiritualBiorhythm : BiorhythmBase
    {
        public override string Name => "Spiritual";
        public override int CycleLength => 53;
        public override EBiorhythm Biorhythm { get; } = EBiorhythm.Spiritual;
        public override string Color => "255, 102, 255";
        public override int Index => 2;
        public override int DisplayIndex => 1;
    }
}