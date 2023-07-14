using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public class EmotionalBiorhythm : BiorhythmBase
    {
        public override string Name => "Emotional";
        public override int CycleLength => 28;
        public override EBiorhythm Biorhythm { get; } = EBiorhythm.Emotional;
        public override string Color => "0, 226, 114";
        public override int Index => 3;
    }
}