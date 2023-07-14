using K9.SharedLibrary.Models;
using K9.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
            
            nineStarKiModel.BiorhythmResultSet.BioRhythms = biorhythmsModel;
            nineStarKiModel.BiorhythmResultSet.NineStarKiBioRhythms = nineStarBiorhythmsModel;

            return nineStarKiModel.BiorhythmResultSet;
        }

        private static List<BiorhythmBase> GetBiorhythms() => Helpers.Methods.GetClassesThatDeriveFrom<BiorhythmBase>().Select(e => (BiorhythmBase)Activator.CreateInstance(e)).OrderBy(e => e.Index).ToList();

        private List<BioRhythmResult> GetBioRhythmResults(BioRhythmsModel biorhythmsModel, NineStarKiBiorhythmsFactors factors = null)
        {
            var bioRhythms = GetBiorhythms();
            var results = new List<BioRhythmResult>();
            double nineStarKiFactor = 0;
            double stabilityFactor = 0;

            foreach (var biorhythm in bioRhythms)
            {
                if (factors != null)
                {
                    nineStarKiFactor = factors.GetFactor(biorhythm.Biorhythm);
                    stabilityFactor = factors.StabilityFactor;
                }
                results.Add(GetBioRhythmResult(biorhythm, biorhythmsModel, nineStarKiFactor, stabilityFactor));
            }

            return results;
        }

        private BioRhythmResult GetBioRhythmResult(BiorhythmBase biorhythm, BioRhythmsModel biorhythmsModel, double nineStarKiFactor = 0, double stabilityFactor = 0)
        {
            var dayInterval = GetDayInterval(biorhythm, biorhythmsModel.DaysElapsedSinceBirth);

            var result = new BioRhythmResult
            {
                BioRhythm = biorhythm,
                DayInterval = dayInterval,
                Value = CalculateValue(biorhythm, dayInterval, nineStarKiFactor, stabilityFactor),
                RangeValues = CalculateCosineRangeValues(biorhythm, biorhythmsModel, nineStarKiFactor, stabilityFactor)
            };
            return result;
        }

        private List<Tuple<string, double>> CalculateCosineRangeValues(IBiorhythm biorhythm, BioRhythmsModel bioRhythmsModel, double nineStarKiFactor = 0, double stabilityFactor = 0)
        {
            var nineStarMonthlyPeriod =
                bioRhythmsModel.NineStarKiModel.GetMonthlyPeriod(bioRhythmsModel.SelectedDate.Value,
                    bioRhythmsModel.PersonModel.Gender);
            var period = nineStarMonthlyPeriod.GetTotalDaysInMonthlyPeriod();
            var daysSinceBeginningOfPeriod =
                (int)bioRhythmsModel.SelectedDate.Value.Subtract(nineStarMonthlyPeriod.MonthlyPeriodStartsOn).TotalDays;
            var list = new List<Tuple<string, double>>();
            
            for (int i = 0; i < period; i++)
            {
                var factor = i - daysSinceBeginningOfPeriod;
                var dayInterval = GetDayInterval(biorhythm, bioRhythmsModel.DaysElapsedSinceBirth + factor);

                list.Add(new Tuple<string, double>((bioRhythmsModel.SelectedDate?.AddDays(factor).ToString(Constants.FormatConstants.SessionDateTimeFormat)), CalculateValue(biorhythm, dayInterval, nineStarKiFactor, stabilityFactor)));
            }

            return list;
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