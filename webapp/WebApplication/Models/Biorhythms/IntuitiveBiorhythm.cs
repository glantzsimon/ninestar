using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public class IntuitiveBiorhythm : BiorhythmBase
    {
        public override string Name => "Intuitive";
        public override int CycleLength => 38;
        public override EBiorhythm Biorhythm { get; } = EBiorhythm.Intuitive;
        public override string Color => "204, 0, 102";
        public override int Index => 6;
    }
}