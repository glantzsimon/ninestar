using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public class BiorhythmFactor
    {
        public ENineStarKiEnergy Energy { get; set; }
        public EBiorhythm Biorhythm { get; set; }
        public double Value { get; set; }

        public BiorhythmFactor(EBiorhythm biorhythm, double value, ENineStarKiEnergy energy = ENineStarKiEnergy.Unspecified)
        {
            Biorhythm = biorhythm;
            Value = value;
            Energy = energy;
        }
    }
}