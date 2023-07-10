namespace K9.WebApplication.Models
{
    public class PhysicalBiorhythm : BiorhythmBase
    {
        public override string Name => "Physical";
        public override int CycleLength => 23;
    }
}