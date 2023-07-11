using K9.WebApplication.Models;
using System;
using System.Collections.Generic;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;

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

        public BioRhythmsModel Calculate(PersonModel personModel, DateTime date)
        {
            var model = new BioRhythmsModel(personModel, date);

            model.IntellectualBiorhythmResult = GetBioRhythmResult(new IntellectualBiorhythm(), model);
            model.EmotionalBiorhythmResult = GetBioRhythmResult(new EmotionalBiorhythm(), model);
            model.PhysicalBiorhythmResult = GetBioRhythmResult(new PhysicalBiorhythm(), model);
            model.SpiritualBiorhythmResult = GetBioRhythmResult(new SpiritualBiorhythm(), model);

            var isUpgradeRequired = !(_roles.CurrentUserIsInRoles(RoleNames.Administrators) || _membershipService.IsCompleteProfileReading(_authentication.CurrentUserId, personModel));

            model.IntellectualBiorhythmResult.IsUpgradeRequired = isUpgradeRequired;
            model.EmotionalBiorhythmResult.IsUpgradeRequired = isUpgradeRequired;
            model.SpiritualBiorhythmResult.IsUpgradeRequired = isUpgradeRequired;

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

        private List<Tuple<string, double>> CalculateCosineRangeValues(IBioRhythm biorhythm, BioRhythmsModel bioRhythmsModel, int numberOfWeeks = 4)
        {
            var period = numberOfWeeks * 7;
            var list = new List<Tuple<string, double>>();

            for (int i = 0; i < period; i++)
            {
                var factor = -(period / 2) + i;
                var dayInterval = GetDayInterval(biorhythm, bioRhythmsModel.DaysElapsedSinceBirth + factor);
                list.Add(new Tuple<string, double>((bioRhythmsModel.SelectedDate?.AddDays(factor).ToString(Constants.FormatConstants.SessionDateTimeFormat)), CalculateValue(biorhythm, dayInterval)));
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