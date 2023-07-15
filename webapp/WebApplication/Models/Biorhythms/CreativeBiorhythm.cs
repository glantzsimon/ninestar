using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public class CreativeBiorhythm : BiorhythmBase
    {
        public override string Name => "Creative";
        public override int CycleLength => 43;
        public override EBiorhythm Biorhythm { get; } = EBiorhythm.Creative;
        public override string Color => "255, 128, 0";
        public override int Index => 5;
        public override int DisplayIndex => 4;
    }
}