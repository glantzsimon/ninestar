using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public abstract class BiorhythmBase : IBiorhythm
    {
        /// <summary>
        /// Gets the name of the biorhythm.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the period of the sinusoidal biorhythm.
        /// </summary>
        public abstract int CycleLength { get; }

        public abstract EBiorhythm Biorhythm { get; }
        
        public abstract string Color { get; }
        
        public abstract int Index { get; }
        
        public abstract int DisplayIndex { get; }

        public virtual int LineWidth => 2;
        
        public virtual bool LineShadow => false;
        
        public virtual bool LineIsDashed => false;

        public string LineDashString => LineIsDashed ? "dash" : string.Empty;
        
        public string FullName => $"{Name} {Globalisation.Dictionary.Biorhythm}";
    }
}