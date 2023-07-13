namespace K9.WebApplication.Models
{
    public abstract class BiorhythmBase : IBioRhythm
    {
        /// <summary>
        /// Values of the sinusoid with the amplitude of 1.
        /// </summary>
        private double[] values;

        /// <summary>
        /// Gets the name of the biorhythm.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the period of the sinusoidal biorhythm.
        /// </summary>
        public abstract int CycleLength { get; }

        public string FullName => $"{Name} {Globalisation.Dictionary.Biorhythm}";
    }
}