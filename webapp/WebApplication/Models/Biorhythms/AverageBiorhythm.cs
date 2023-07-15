using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public class AverageBiorhythm : BiorhythmBase
    {
        public override string Name => "Combined";
        public override int CycleLength => 0;
        public override EBiorhythm Biorhythm { get; } = EBiorhythm.Average;
        public override string Color => "88, 88, 88";
        public override int LineWidth => 3;
        public override int Index => 0;
        public override int DisplayIndex => 6;
        public override bool LineShadow => false;
        public override bool LineIsDashed => true;
    }
}