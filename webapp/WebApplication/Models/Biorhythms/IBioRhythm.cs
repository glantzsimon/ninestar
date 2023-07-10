namespace K9.WebApplication.Models
{
    public interface IBioRhythm
    {
        string Name { get; }
        int CycleLength { get; }

        /// <summary>
        /// Returns the value of the biorhythm for the specified day.
        /// </summary>
        /// <param name="dayIndex">The index of the day for which to return the value of the biorhythm.</param>
        /// <returns>The value of the biorhythm for the specified day.</returns>
        double GetValue(int dayIndex);
    }
}