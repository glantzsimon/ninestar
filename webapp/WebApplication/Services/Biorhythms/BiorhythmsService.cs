using K9.WebApplication.Models;
using System;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public class BiorhythmsService : IBiorhythmsService
    {
        public BioRhythmsModel Calculate(PersonModel personModel, DateTime date)
        {
            var model = new BioRhythmsModel(personModel, date);

            model.IntellectualBiorhythmResult = GetBioRhythmResult(new IntellectualBiorhythm(), model);
            model.EmotionalBiorhythmResult = GetBioRhythmResult(new EmotionalBiorhythm(), model);
            model.PhysicalBiorhythmResult = GetBioRhythmResult(new PhysicalBiorhythm(), model);
            model.SpiritualBiorhythmResult = GetBioRhythmResult(new SpiritualBiorhythm(), model);

            return model;
        }

        private BioRhythmResult GetBioRhythmResult(BiorhythmBase biorhythm, BioRhythmsModel biorhythmsModel)
        {
            var dayInterval = GetDayInterval(biorhythm, biorhythmsModel.DaysElapsedSinceBirth);
            return new BioRhythmResult
            {
                BioRhythm = biorhythm,
                DayInterval = dayInterval,
                Value = CalculateValue(biorhythm, dayInterval),
                RangeValues = CalculateCosineRangeValues(biorhythm, biorhythmsModel)
            };
        }

        private List<Tuple<DateTime, double>> CalculateCosineRangeValues(IBioRhythm biorhythm, BioRhythmsModel bioRhythmsModel, int numberOfWeeks = 4)
        {
            var period = numberOfWeeks * 7;
            var list = new List<Tuple<DateTime, double>>();

            for (int i = 0; i < period; i++)
            {
                var factor = -(bioRhythmsModel.DaysElapsedSinceBirth / 2) + i;
                var dayInterval = GetDayInterval(biorhythm, bioRhythmsModel.DaysElapsedSinceBirth + factor);
                list.Add(new Tuple<DateTime, double>(bioRhythmsModel.Date.Value.AddDays(factor), CalculateValue(biorhythm, dayInterval)));
            }

            return list;
        }

        private double CalculateValue(IBioRhythm bioRhythm, int dayInterval)
        {
            return 50 + (50 * Math.Sin((2 * Math.PI * dayInterval) / bioRhythm.CycleLength));
        }

        private int GetDayInterval(IBioRhythm bioRhythm, int daysElapsedSinceBirth)
        {
            return Math.Abs(daysElapsedSinceBirth % bioRhythm.CycleLength);
        }
    }
}