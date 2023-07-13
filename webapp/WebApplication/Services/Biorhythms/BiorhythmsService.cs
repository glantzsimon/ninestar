using K9.WebApplication.Models;
using System;
using System.Collections.Generic;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using K9.WebApplication.Enums;
using Microsoft.Ajax.Utilities;

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

        public BioRhythmsModel Calculate(NineStarKiModel nineStarKiModel, DateTime date)
        {
            var biorhythmsModel = new BioRhythmsModel(nineStarKiModel, date);
            var nineStarBiorhythmsModel = new BioRhythmsModel(nineStarKiModel, date);
            var nineStarKiBiorhythmsModel = new NineStarKiBiorhythmsModel(nineStarKiModel);
            
            biorhythmsModel.IntellectualBiorhythmResult = GetBioRhythmResult(new IntellectualBiorhythm(), biorhythmsModel);
            biorhythmsModel.EmotionalBiorhythmResult = GetBioRhythmResult(new EmotionalBiorhythm(), biorhythmsModel);
            biorhythmsModel.PhysicalBiorhythmResult = GetBioRhythmResult(new PhysicalBiorhythm(), biorhythmsModel);
            biorhythmsModel.SpiritualBiorhythmResult = GetBioRhythmResult(new SpiritualBiorhythm(), biorhythmsModel);

            nineStarBiorhythmsModel.IntellectualBiorhythmResult = GetBioRhythmResult(new IntellectualBiorhythm(), nineStarBiorhythmsModel, nineStarKiBiorhythmsModel.IntellectualFactor, nineStarKiBiorhythmsModel.StabilityFactor);
            nineStarBiorhythmsModel.EmotionalBiorhythmResult = GetBioRhythmResult(new EmotionalBiorhythm(), nineStarBiorhythmsModel,
                nineStarKiBiorhythmsModel.EmotionalFactor, nineStarKiBiorhythmsModel.StabilityFactor);
            nineStarBiorhythmsModel.PhysicalBiorhythmResult = GetBioRhythmResult(new PhysicalBiorhythm(), nineStarBiorhythmsModel,
                nineStarKiBiorhythmsModel.PhysicalFactor, nineStarKiBiorhythmsModel.StabilityFactor);
            nineStarBiorhythmsModel.SpiritualBiorhythmResult = GetBioRhythmResult(new SpiritualBiorhythm(), nineStarBiorhythmsModel,
                nineStarKiBiorhythmsModel.SpiritualFactor, nineStarKiBiorhythmsModel.StabilityFactor);

            nineStarKiModel.Biorhythms = biorhythmsModel;
            nineStarKiModel.NineStarKiBiorhythms = nineStarBiorhythmsModel;

            return biorhythmsModel;
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

        private List<Tuple<string, double>> CalculateCosineRangeValues(IBioRhythm biorhythm, BioRhythmsModel bioRhythmsModel, double nineStarKiFactor = 0, double stabilityFactor = 0)
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

        private double CalculateValue(IBioRhythm bioRhythm, int dayInterval, double nineStarKiFactor = 0, double nineStarKiStabilityFactor = 0)
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

        private int GetDayInterval(IBioRhythm bioRhythm, int daysElapsedSinceBirth)
        {
            return Math.Abs(daysElapsedSinceBirth % bioRhythm.CycleLength);
        }
    }
}