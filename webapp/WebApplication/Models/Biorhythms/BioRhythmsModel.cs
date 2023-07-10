using System;

namespace K9.WebApplication.Models
{
    public class BioRhythmsModel
    {

        public BioRhythmsModel()
        {
            PersonModel = new PersonModel();
        }

        public BioRhythmsModel(PersonModel personModel, DateTime? date = null)
        {
            date = date ?? DateTime.Today;

            Date = date;
            PersonModel = personModel;
            NineStarKiModel = new NineStarKiModel(personModel, date);
            DaysElapsedSinceBirth = GetDaysElapsedSinceBirth(date.Value);
        }

        public DateTime? Date { get; }

        public PersonModel PersonModel { get; }

        public NineStarKiModel NineStarKiModel { get; }

        public int DaysElapsedSinceBirth { get; }

        public BioRhythmResult IntellectualBiorhythmResult { get; set; }

        public BioRhythmResult EmotionalBiorhythmResult { get; set; }

        public BioRhythmResult PhysicalBiorhythmResult { get; set; }

        public BioRhythmResult SpiritualBiorhythmResult { get; set; }

        private int GetDaysElapsedSinceBirth(DateTime date)
        {
            return (int)date.Subtract(PersonModel.DateOfBirth).TotalDays;
        }
    }
}