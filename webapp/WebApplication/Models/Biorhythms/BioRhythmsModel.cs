using System;
using System.Collections.Generic;
using System.Linq;
using K9.WebApplication.Enums;

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
            BiorhythmResults = new List<BioRhythmResult>();
        }

        public DateTime? SelectedDate { get; }

        public PersonModel PersonModel { get; }

        public NineStarKiModel NineStarKiModel { get; }

        public int DaysElapsedSinceBirth { get; }

        public List<BioRhythmResult> BiorhythmResults { get; set; }

        public List<BioRhythmResult> GetBiorhythmResultsByDisplayIndex() => BiorhythmResults.OrderBy(e => e.BioRhythm.DisplayIndex).ToList();

        public BioRhythmResult GetAverageResult() =>
            BiorhythmResults.FirstOrDefault(e => e.BioRhythm.Biorhythm == EBiorhythm.Average);

        public List<BioRhythmResult> GetResultsWithoutAverage() =>
            BiorhythmResults.Where(e => e.BioRhythm.Biorhythm != EBiorhythm.Average).ToList();

        public BioRhythmResult GetResultByType(EBiorhythm biorhythm) =>
            BiorhythmResults?.FirstOrDefault(e => e.BioRhythm.Biorhythm == biorhythm);

        private int GetDaysElapsedSinceBirth(DateTime date)
        {
            return (int)date.Subtract(PersonModel.DateOfBirth).TotalDays;
        }
    }
}