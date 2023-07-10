using System;
using System.Diagnostics;
using K9.Base.DataAccessLayer.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class BioRhythmsServiceTests
    {
        public BioRhythmsServiceTests()
        {
        }

        [Theory]
        [InlineData(1979, 06, 16, 1979, 06, 16, EGender.Male, ENineStarKiEnergy.Thunder, 0, 0, 50)]
        [InlineData(1979, 06, 16, 1979, 06, 19, EGender.Male, ENineStarKiEnergy.Thunder, 3, 3, 77)]
        [InlineData(1979, 06, 16, 1979, 06, 24, EGender.Male, ENineStarKiEnergy.Thunder, 8, 8, 100)]
        [InlineData(1979, 06, 16, 1979, 06, 26, EGender.Male, ENineStarKiEnergy.Thunder, 10, 10, 97)]
        [InlineData(1979, 06, 16, 1979, 07, 03, EGender.Male, ENineStarKiEnergy.Thunder, 17, 17, 45)]
        [InlineData(1979, 06, 16, 1979, 07, 19, EGender.Male, ENineStarKiEnergy.Thunder, 33, 0, 50)]
        public void MonthEnergy_HappyPath(int birthYear, int birthMonth, int birthDay, int dateYear, int dateMonth, int dateDay, EGender gender, ENineStarKiEnergy expectedEnergy, int expectedDaysElapsedSinceBirth, int expectedDayInterval, double expectedValue)
        {
            var biorhythmsService = new BiorhythmsService();

            var result = biorhythmsService.Calculate(
                new PersonModel
                {
                    DateOfBirth = new DateTime(birthYear, birthMonth, birthDay)
                },
                new DateTime(dateYear, dateMonth, dateDay));

            Assert.Equal(expectedEnergy, result.NineStarKiModel.MainEnergy.Energy);
            Assert.Equal(expectedDaysElapsedSinceBirth, result.DaysElapsedSinceBirth);
            Assert.Equal(expectedDayInterval, result.IntellectualBiorhythmResult.DayInterval);
            Assert.Equal(expectedValue, Math.Round(result.IntellectualBiorhythmResult.Value, MidpointRounding.ToEven));
        }

    }
}
