using System;

namespace K9.WebApplication.Models
{
    public class BioRhythmsModel
    {

        public BioRhythmsModel()
        {
            PersonModel = new PersonModel();
        }

        public BioRhythmsModel(NineStarKiModel nineStarKiModel, DateTime? selectedDate = null)
        {
            selectedDate = selectedDate ?? DateTime.Today;

            SelectedDate = selectedDate;
            PersonModel = nineStarKiModel.PersonModel;
            NineStarKiModel = nineStarKiModel;
            DaysElapsedSinceBirth = GetDaysElapsedSinceBirth(selectedDate.Value);
        }

        public DateTime? SelectedDate { get; }

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