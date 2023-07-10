using System;

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

        protected BiorhythmBase()
        {
            RegenerateValues();
        }

        private void RegenerateValues()
        {
            int period = CycleLength;

            values = new double[period];

            for (int i = 0; i < period; i++)
                values[i] = Math.Sin(i * 2 * Math.PI / period);
        }

        /// <summary>
        /// Returns the value of the biorhythm for the specified day.
        /// </summary>
        /// <param name="dayIndex">The index of the day for which to return the value of the biorhythm.</param>
        /// <returns>The value of the biorhythm for the specified day.</returns>
        public double GetValue(int dayIndex)
        {
            int index = dayIndex % CycleLength;

            if (index < 0)
                index += CycleLength;

            return values[index];
        }
    }
}