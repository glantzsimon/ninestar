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
            nineStarBiorhythmsModel.Summary = GetSummary(nineStarBiorhythmsModel);

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
                    BiorhythmLevel = biorhythm.GetValueLevelDescription(biorhythm.Value),
                    BiorhythmSummary = GetBiorhythmSummary(biorhythm),
                    BiorhythmTrendHtml = biorhythm.GetBiorhythmTrendHtmlString(),
                    BiorhythmTrend = biorhythm.GetBiorhythmTrendDescription(),
                    BiorhythmNextMax = biorhythm.GetDaysUntilNextMaximumString(),
                    BiorhythmNextMin = biorhythm.GetDaysUntilNextMinimumString(),
                    BiorhythmNextCritical = biorhythm.GetDaysUntilNextCriticalString()
                }));
                sb.AppendLine("</br>");
            }

            var average = biorhythmsModel.GetAverageResult();

            sb.Append(TemplateProcessor.PopulateTemplate(Globalisation.Dictionary.biorhythms_summary, new
            {
                BiorhythmName = average.BioRhythm.FullName,
                BiorhythmLevel = average.GetValueLevelDescription(average.Value),
                BiorhythmSummary = GetBiorhythmSummary(average),
                BiorhythmTrendHtml = average.GetBiorhythmTrendHtmlString(),
                BiorhythmTrend = average.GetBiorhythmTrendDescription(),
                BiorhythmNextMax = average.GetDaysUntilNextMaximumString(),
                BiorhythmNextMin = average.GetDaysUntilNextMinimumString(),
                BiorhythmNextCritical = average.GetDaysUntilNextCriticalString(),
                AverageClass = "display-none"
            }));
            sb.AppendLine("</br>");

            return sb.ToString();
        }

        private string GetBiorhythmSummary(BioRhythmResult biorhythm)
        {
            switch (biorhythm.BioRhythm.Biorhythm)
            {
                case EBiorhythm.Intellectual:
                    switch (biorhythm.GetValueLevel(biorhythm.Value))
                    {
                        case EBiorhythmLevel.ExtremelyLow:
                            return Globalisation.Dictionary.intellectual_extremely_low;

                        case EBiorhythmLevel.VeryLow:
                            return Globalisation.Dictionary.intellectual_very_low;

                        case EBiorhythmLevel.Low:
                            return Globalisation.Dictionary.intellectual_low;

                        case EBiorhythmLevel.Moderate:
                            return Globalisation.Dictionary.intellectual_moderate;

                        case EBiorhythmLevel.Critical:
                            return Globalisation.Dictionary.intellectual_critical;

                        case EBiorhythmLevel.High:
                            return Globalisation.Dictionary.intellectual_high;

                        case EBiorhythmLevel.VeryHigh:
                            return Globalisation.Dictionary.intellectual_very_high;

                        case EBiorhythmLevel.Excellent:
                            return Globalisation.Dictionary.intellectual_excellent;

                        default:
                            return string.Empty;
                    }

                case EBiorhythm.Emotional:
                    switch (biorhythm.GetValueLevel(biorhythm.Value))
                    {
                        case EBiorhythmLevel.ExtremelyLow:
                            return Globalisation.Dictionary.emotional_extremely_low;

                        case EBiorhythmLevel.VeryLow:
                            return Globalisation.Dictionary.emotional_very_low;

                        case EBiorhythmLevel.Low:
                            return Globalisation.Dictionary.emotional_low;

                        case EBiorhythmLevel.Moderate:
                            return Globalisation.Dictionary.emotional_moderate;

                        case EBiorhythmLevel.Critical:
                            return Globalisation.Dictionary.emotional_critical;

                        case EBiorhythmLevel.High:
                            return Globalisation.Dictionary.emotional_high;

                        case EBiorhythmLevel.VeryHigh:
                            return Globalisation.Dictionary.emotional_very_high;

                        case EBiorhythmLevel.Excellent:
                            return Globalisation.Dictionary.emotional_excellent;

                        default:
                            return string.Empty;
                    }

                case EBiorhythm.Physical:
                    switch (biorhythm.GetValueLevel(biorhythm.Value))
                    {
                        case EBiorhythmLevel.ExtremelyLow:
                            return Globalisation.Dictionary.physical_extremely_low;

                        case EBiorhythmLevel.VeryLow:
                            return Globalisation.Dictionary.physical_very_low;

                        case EBiorhythmLevel.Low:
                            return Globalisation.Dictionary.physical_low;

                        case EBiorhythmLevel.Moderate:
                            return Globalisation.Dictionary.physical_moderate;

                        case EBiorhythmLevel.Critical:
                            return Globalisation.Dictionary.physical_critical;

                        case EBiorhythmLevel.High:
                            return Globalisation.Dictionary.physical_high;

                        case EBiorhythmLevel.VeryHigh:
                            return Globalisation.Dictionary.physical_very_high;

                        case EBiorhythmLevel.Excellent:
                            return Globalisation.Dictionary.physical_excellent;

                        default:
                            return string.Empty;
                    }

                case EBiorhythm.Spiritual:
                    switch (biorhythm.GetValueLevel(biorhythm.Value))
                    {
                        case EBiorhythmLevel.ExtremelyLow:
                            return Globalisation.Dictionary.spiritual_extremely_low;

                        case EBiorhythmLevel.VeryLow:
                            return Globalisation.Dictionary.spiritual_very_low;

                        case EBiorhythmLevel.Low:
                            return Globalisation.Dictionary.spiritual_low;

                        case EBiorhythmLevel.Moderate:
                            return Globalisation.Dictionary.spiritual_moderate;

                        case EBiorhythmLevel.Critical:
                            return Globalisation.Dictionary.spiritual_critical;

                        case EBiorhythmLevel.High:
                            return Globalisation.Dictionary.spiritual_high;

                        case EBiorhythmLevel.VeryHigh:
                            return Globalisation.Dictionary.spiritual_very_high;

                        case EBiorhythmLevel.Excellent:
                            return Globalisation.Dictionary.spiritual_excellent;

                        default:
                            return string.Empty;
                    }

                case EBiorhythm.Intuitive:
                    switch (biorhythm.GetValueLevel(biorhythm.Value))
                    {
                        case EBiorhythmLevel.ExtremelyLow:
                            return Globalisation.Dictionary.intuitive_extremely_low;

                        case EBiorhythmLevel.VeryLow:
                            return Globalisation.Dictionary.intuitive_very_low;

                        case EBiorhythmLevel.Low:
                            return Globalisation.Dictionary.intuitive_low;

                        case EBiorhythmLevel.Moderate:
                            return Globalisation.Dictionary.intuitive_moderate;

                        case EBiorhythmLevel.Critical:
                            return Globalisation.Dictionary.intuitive_critical;

                        case EBiorhythmLevel.High:
                            return Globalisation.Dictionary.intuitive_high;

                        case EBiorhythmLevel.VeryHigh:
                            return Globalisation.Dictionary.intuitive_very_high;

                        case EBiorhythmLevel.Excellent:
                            return Globalisation.Dictionary.intuitive_excellent;

                        default:
                            return string.Empty;
                    }

                case EBiorhythm.Creative:
                    switch (biorhythm.GetValueLevel(biorhythm.Value))
                    {
                        case EBiorhythmLevel.ExtremelyLow:
                            return Globalisation.Dictionary.creative_extremely_low;

                        case EBiorhythmLevel.VeryLow:
                            return Globalisation.Dictionary.creative_very_low;

                        case EBiorhythmLevel.Low:
                            return Globalisation.Dictionary.creative_low;

                        case EBiorhythmLevel.Moderate:
                            return Globalisation.Dictionary.creative_moderate;

                        case EBiorhythmLevel.Critical:
                            return Globalisation.Dictionary.creative_critical;

                        case EBiorhythmLevel.High:
                            return Globalisation.Dictionary.creative_high;

                        case EBiorhythmLevel.VeryHigh:
                            return Globalisation.Dictionary.creative_very_high;

                        case EBiorhythmLevel.Excellent:
                            return Globalisation.Dictionary.creative_excellent;

                        default:
                            return string.Empty;
                    }

                case EBiorhythm.Average:
                    switch (biorhythm.GetValueLevel(biorhythm.Value))
                    {
                        case EBiorhythmLevel.ExtremelyLow:
                            return Globalisation.Dictionary.average_extremely_low;

                        case EBiorhythmLevel.VeryLow:
                            return Globalisation.Dictionary.average_very_low;

                        case EBiorhythmLevel.Low:
                            return Globalisation.Dictionary.average_low;

                        case EBiorhythmLevel.Moderate:
                            return Globalisation.Dictionary.average_moderate;

                        case EBiorhythmLevel.Critical:
                            return Globalisation.Dictionary.average_critical;

                        case EBiorhythmLevel.High:
                            return Globalisation.Dictionary.average_high;

                        case EBiorhythmLevel.VeryHigh:
                            return Globalisation.Dictionary.average_very_high;

                        case EBiorhythmLevel.Excellent:
                            return Globalisation.Dictionary.average_excellent;

                        default:
                            return string.Empty;
                    }

                default:
                    return string.Empty;
            }
        }

       
        private static List<BiorhythmBase> GetBiorhythms() => Helpers.Methods.GetClassesThatDeriveFrom<BiorhythmBase>().Select(e => (BiorhythmBase)Activator.CreateInstance(e)).OrderBy(e => e.Index).ToList();

        private List<BioRhythmResult> GetBioRhythmResults(BioRhythmsModel biorhythmsModel, NineStarKiBiorhythmsFactors factors = null)
        {
            var bioRhythms = GetBiorhythms();
            var results = new List<BioRhythmResult>();
            double nineStarKiFactor = 0;
            double stabilityFactor = 0;
            biorhythmsModel.MaxCycleLength = bioRhythms.Max(e => e.CycleLength);

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
            var averageLongRangeValues = new List<RangeValue>();
            var firstResult = results.First();
            
            for (int i = 0; i < firstResult.RangeValues.Count; i++)
            {
                var date = firstResult.RangeValues[i].Date;
                averageRangeValues.Add(new RangeValue(date, results.Where(e => e.BioRhythm.Biorhythm != EBiorhythm.Creative).Select(e => e.RangeValues[i].Value).Average()));
            }
            
            for (int i = 0; i < firstResult.LongRangeValues.Count; i++)
            {
                var date = firstResult.LongRangeValues[i].Date;
                averageLongRangeValues.Add(new RangeValue(date, results.Where(e => e.BioRhythm.Biorhythm != EBiorhythm.Creative).Select(e => e.LongRangeValues[i].Value).Average()));
            }

            results.Insert(0, new BioRhythmResult
            {
                BioRhythm = average,
                Value = results.Average(e => e.Value),
                RangeValues = averageRangeValues,
                LongRangeValues = averageLongRangeValues,
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
                Value = CalculateValue(biorhythm, dayInterval, nineStarKiFactor, stabilityFactor)
            };

            CalculateCosineRangeValues(result, biorhythm, biorhythmsModel, nineStarKiFactor, stabilityFactor);
            
            return result;
        }

        private void CalculateCosineRangeValues(BioRhythmResult result,IBiorhythm biorhythm, BioRhythmsModel bioRhythmsModel, double nineStarKiFactor = 0, double stabilityFactor = 0)
        {
            var nineStarMonthlyPeriod =
                bioRhythmsModel.NineStarKiModel.GetMonthlyPeriod(bioRhythmsModel.SelectedDate.Value,
                    bioRhythmsModel.PersonModel.Gender);
            var period = nineStarMonthlyPeriod.GetTotalDaysInMonthlyPeriod();
            var daysSinceBeginningOfPeriod =
                (int)bioRhythmsModel.SelectedDate.Value.Subtract(nineStarMonthlyPeriod.MonthlyPeriodStartsOn).TotalDays;
            var rangeValues = new List<RangeValue>();
            var longRangeValues = new List<RangeValue>();
           
            for (int i = 0; i < period; i++)
            {
                var factor = i - daysSinceBeginningOfPeriod;
                var dayInterval = GetDayInterval(biorhythm, bioRhythmsModel.DaysElapsedSinceBirth + factor);
                var dateTime = bioRhythmsModel.SelectedDate?.AddDays(factor);

                rangeValues.Add(new RangeValue(dateTime,
                    CalculateValue(biorhythm, dayInterval, nineStarKiFactor, stabilityFactor)));
            }
            result.RangeValues = rangeValues;

            for (int i = -(bioRhythmsModel.MaxCycleLength * 3); i < (bioRhythmsModel.MaxCycleLength * 6); i++)
            {
                var dayInterval = GetDayInterval(biorhythm, bioRhythmsModel.DaysElapsedSinceBirth + i);
                var dateTime = bioRhythmsModel.SelectedDate?.AddDays(i);

                longRangeValues.Add(new RangeValue(dateTime,
                    CalculateValue(biorhythm, dayInterval, nineStarKiFactor, stabilityFactor)));
            }
            result.LongRangeValues = longRangeValues;
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