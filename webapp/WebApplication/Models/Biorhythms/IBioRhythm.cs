using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public interface IBiorhythm
    {
        string Name { get; }
        int CycleLength { get; }
        EBiorhythm Biorhythm { get; }
        string Color { get; }
        int LineWidth { get; }
        int Index { get; }
        bool LineShadow { get; }
        bool LineIsDashed { get; }
    }
}