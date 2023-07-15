using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public class PhysicalBiorhythm : BiorhythmBase
    {
        public override string Name => "Physical";
        public override int CycleLength => 23;
        public override EBiorhythm Biorhythm { get; } = EBiorhythm.Physical;
        public override string Color => "255, 51, 51";
        public override int Index => 4;
        public override int DisplayIndex => 3;
    }
}