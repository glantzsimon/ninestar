using K9.SharedLibrary.Models;
using K9.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Enums;

namespace K9.WebApplication.Services
{
    public class BiorhythmsService : IBiorhythmsService
    {
        private readonly IRoles _roles;
        private readonly IMembershipService _membershipService;
        private readonly IAuthentication _authentication;

        public BiorhythmsService(IRoles roles, IMembershipService membershipService, IAuthentication authentication)
        {
            _roles = roles;
            _membershipService = membershipService;
            _authentication = authentication;
        }

        public BioRhythmsResultSet Calculate(NineStarKiModel nineStarKiModel, DateTime date)
        {
            var biorhythmsModel = new BioRhythmsModel(nineStarKiModel, date);
            var nineStarBiorhythmsModel = new BioRhythmsModel(nineStarKiModel, date);
            var nineStarKiBiorhythmsFactors = new NineStarKiBiorhythmsFactors(nineStarKiModel);

            biorhythmsModel.BiorhythmResults = GetBioRhythmResults(biorhythmsModel);
            nineStarBiorhythmsModel.BiorhythmResults = GetBioRhythmResults(nineStarBiorhythmsModel, nineStarKiBiorhythmsFactors);

            biorhythmsModel.Summary = GetSummary(biorhythmsModel);
            biorhythmsModel.Summary = GetSummary(biorhythmsModel);

            nineStarKiModel.BiorhythmResultSet.BioRhythms = biorhythmsModel;
            nineStarKiModel.BiorhythmResultSet.NineStarKiBioRhythms = nineStarBiorhythmsModel;

            return nineStarKiModel.BiorhythmResultSet;
        }

        private string GetSummary(BioRhythmsModel biorhythmsModel)
        {
            var sb = new StringBuilder();

            foreach (var biorhythm in biorhythmsModel.GetResultsWithoutAverage())
            {
                sb.Append(TemplateProcessor.PopulateTemplate(Globalisation.Dictionary.biorhythms_summary, new
                {
                    BiorhythmName = biorhythm.BioRhythm.FullName,
                    BiorhythmSummary = GetSummaryBiorhythmSummary(biorhythm)
                }));
                sb.AppendLine("</br>");
            }

            var average = biorhythmsModel.GetAverageResult();

            sb.Append(TemplateProcessor.PopulateTemplate(Globalisation.Dictionary.biorhythms_summary, new
            {
                BiorhythmName = average.BioRhythm.FullName,
                BiorhythmSummary = GetSummaryBiorhythmSummary(average)
            }));
            sb.AppendLine("</br>");

            return sb.ToString();
        }

        private string GetSummaryBiorhythmSummary(BioRhythmResult biorhythm)
        {
            switch (biorhythm.BioRhythm.Biorhythm)
            {
                case EBiorhythm.Intellectual:
                    return "";

            }

            return string.Empty;
        }

        private static List<BiorhythmBase> GetBiorhythms() => Helpers.Methods.GetClassesThatDeriveFrom<BiorhythmBase>().Select(e => (BiorhythmBase)Activator.CreateInstance(e)).OrderBy(e => e.Index).ToList();

        private List<BioRhythmResult> GetBioRhythmResults(BioRhythmsModel biorhythmsModel, NineStarKiBiorhythmsFactors factors = null)
        {
            var bioRhythms = GetBiorhythms();
            var results = new List<BioRhythmResult>();
            double nineStarKiFactor = 0;
            double stabilityFactor = 0;

            foreach (var biorhythm in bioRhythms.Where(e => e.Biorhythm != EBiorhythm.Average))
            {
                if (factors != null)
                {
                    nineStarKiFactor = factors.GetFactor(biorhythm.Biorhythm);
                    stabilityFactor = factors.StabilityFactor;
                }
                results.Add(GetBioRhythmResult(biorhythm, biorhythmsModel, nineStarKiFactor, stabilityFactor));
            }

            var average = bioRhythms.Where(e => e.Biorhythm == EBiorhythm.Average).First();
            var averageRangeValues = new List<RangeValue>();
            var firstResult = results.First();

            for (int i = 0; i < firstResult.RangeValues.Count; i++)
            {
                var date = firstResult.RangeValues[i].Date;
                averageRangeValues.Add(new RangeValue(date, results.Where(e => e.BioRhythm.Biorhythm != EBiorhythm.Creative).Select(e => e.RangeValues[i].Value).Average()));
            }

            results.Insert(0, new BioRhythmResult
            {
                BioRhythm = average,
                Value = results.Average(e => e.Value),
                RangeValues = averageRangeValues,
                SelectedDate = biorhythmsModel.SelectedDate.Value
            });

            return results;
        }

        private BioRhythmResult GetBioRhythmResult(BiorhythmBase biorhythm, BioRhythmsModel biorhythmsModel, double nineStarKiFactor = 0, double stabilityFactor = 0)
        {
            var dayInterval = GetDayInterval(biorhythm, biorhythmsModel.DaysElapsedSinceBirth);

            var result = new BioRhythmResult
            {
                BioRhythm = biorhythm,
                SelectedDate = biorhythmsModel.SelectedDate.Value,
                DayInterval = dayInterval,
                Value = CalculateValue(biorhythm, dayInterval, nineStarKiFactor, stabilityFactor),
                RangeValues = CalculateCosineRangeValues(biorhythm, biorhythmsModel, nineStarKiFactor, stabilityFactor)
            };
            
            return result;
        }

        private List<RangeValue> CalculateCosineRangeValues(IBiorhythm biorhythm, BioRhythmsModel bioRhythmsModel, double nineStarKiFactor = 0, double stabilityFactor = 0)
        {
            var nineStarMonthlyPeriod =
                bioRhythmsModel.NineStarKiModel.GetMonthlyPeriod(bioRhythmsModel.SelectedDate.Value,
                    bioRhythmsModel.PersonModel.Gender);
            var period = nineStarMonthlyPeriod.GetTotalDaysInMonthlyPeriod();
            var daysSinceBeginningOfPeriod =
                (int)bioRhythmsModel.SelectedDate.Value.Subtract(nineStarMonthlyPeriod.MonthlyPeriodStartsOn).TotalDays;
            var rangeValues = new List<RangeValue>();

            for (int i = 0; i < period; i++)
            {
                var factor = i - daysSinceBeginningOfPeriod;
                var dayInterval = GetDayInterval(biorhythm, bioRhythmsModel.DaysElapsedSinceBirth + factor);
                var dateTime = bioRhythmsModel.SelectedDate?.AddDays(factor);

                rangeValues.Add(new RangeValue(dateTime,
                    CalculateValue(biorhythm, dayInterval, nineStarKiFactor, stabilityFactor)));
            }

            return rangeValues;
        }

        private double CalculateValue(IBiorhythm bioRhythm, int dayInterval, double nineStarKiFactor = 0, double nineStarKiStabilityFactor = 0)
        {
            double range = 50;
            double phase = 0;

            if (nineStarKiFactor > 0)
            {
                double factor = nineStarKiFactor > 1 ? 1 - (nineStarKiFactor - 1) : nineStarKiFactor == 1 ? 1 : 1 - (1 - nineStarKiFactor);
                double stabilityFactor = 1 - (nineStarKiStabilityFactor - 0.7);
                double combinedFactor = (factor * stabilityFactor);

                double stabilityOffsetFactor = 3.3333333;
                double stabilityOffset = (1 - factor) * stabilityOffsetFactor;

                double nineStarKiPhase = nineStarKiFactor <= 1 ? 0 : 100 - (100 * factor);
                double stabilityPhase = (100 - (100 * stabilityFactor)) / 2;

                range = 50 * combinedFactor;

                if (nineStarKiFactor < 1)
                {
                    phase = stabilityPhase * stabilityOffset;
                }
                else if (nineStarKiFactor > 1)
                {
                    phase = nineStarKiPhase + (stabilityPhase * stabilityOffset);
                }
                else
                {
                    phase = stabilityPhase;
                }

            }

            var value = phase + range + (range * Math.Sin((2 * Math.PI * dayInterval) / bioRhythm.CycleLength));
            return value;
        }

        private int GetDayInterval(IBiorhythm bioRhythm, int daysElapsedSinceBirth)
        {
            return Math.Abs(daysElapsedSinceBirth % bioRhythm.CycleLength);
        }
    }
}