using K9.WebApplication.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace K9.WebApplication.Models
{
    public class BioRhythmsModel
    {
        public BioRhythmsModel()
        {
            PersonModel = new PersonModel();
            BiorhythmResults = new List<BioRhythmResult>();
            _biorhythmLookup = new Dictionary<EBiorhythm, BioRhythmResult>();
        }

        public BioRhythmsModel(NineStarKiModel nineStarKiModel, DateTime? selectedDate = null)
        {
            SelectedDate = selectedDate ?? DateTime.Today;
            PersonModel = nineStarKiModel.PersonModel;
            NineStarKiModel = nineStarKiModel;
            DaysElapsedSinceBirth = (int)(SelectedDate.Value - PersonModel.DateOfBirth).TotalDays;
            BiorhythmResults = new List<BioRhythmResult>();
            _biorhythmLookup = new Dictionary<EBiorhythm, BioRhythmResult>();
        }

        public DateTime? SelectedDate { get; }

        public DateTime MonthlyPeriodStartsOn { get; set; }
        public DateTime MonthlyPeriodEndsOn { get; set; }

        public int GetTotalDaysInMonthlyPeriod() => (int)MonthlyPeriodEndsOn.Subtract(MonthlyPeriodStartsOn).TotalDays;

        public string GetPeriodTitle() =>
            $"{MonthlyPeriodStartsOn.ToLongDateString()} - {MonthlyPeriodEndsOn.ToLongDateString()}";

        [ScriptIgnore]
        public PersonModel PersonModel { get; }

        [ScriptIgnore]
        public NineStarKiModel NineStarKiModel { get; set; }

        public int DaysElapsedSinceBirth { get; private set; }

        public string Summary { get; set; }

        private List<BioRhythmResult> _biorhythmResults;
        public List<BioRhythmResult> BiorhythmResults
        {
            get { return _biorhythmResults; }
            set
            {
                _biorhythmResults = value;
                _biorhythmLookup = value.ToDictionary(e => e.BioRhythm.Biorhythm, e => e);
            }
        }

        public int MaxCycleLength { get; set; }

        // Optimized List Operations
        public IEnumerable<BioRhythmResult> GetBiorhythmResultsByDisplayIndex() =>
            BiorhythmResults.OrderBy(e => e.BioRhythm.DisplayIndex);

        public BioRhythmResult GetAverageResult() =>
            _biorhythmLookup.TryGetValue(EBiorhythm.Average, out var result) ? result : null;

        public IEnumerable<BioRhythmResult> GetResultsWithoutAverage() =>
            BiorhythmResults.Where(e => e.BioRhythm.Biorhythm != EBiorhythm.Average);

        public BioRhythmResult GetResultByType(EBiorhythm biorhythm) =>
            _biorhythmLookup.TryGetValue(biorhythm, out var result) ? result : null;

        private Dictionary<EBiorhythm, BioRhythmResult> _biorhythmLookup;
    }
}


