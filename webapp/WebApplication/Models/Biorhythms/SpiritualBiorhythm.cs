using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public class SpiritualBiorhythm : BiorhythmBase
    {
        public override string Name => "Spiritual";
        public override int CycleLength => 53;
        public override EBiorhythm Biorhythm { get; } = EBiorhythm.Spiritual;
        public override string Color => "84, 79, 197";
        public override int Index => 2;
    }
}