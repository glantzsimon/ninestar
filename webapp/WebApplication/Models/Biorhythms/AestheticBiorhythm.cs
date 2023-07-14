using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public class AestheticBiorhythm : BiorhythmBase
    {
        public override string Name => "Aesthetic";
        public override int CycleLength => 43;
        public override EBiorhythm Biorhythm { get; } = EBiorhythm.Aesthetic;
        public override string Color => "107, 138, 188";
        public override int Index => 5;
    }
}