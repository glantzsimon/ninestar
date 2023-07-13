using System;
using System.Diagnostics;
using K9.Base.DataAccessLayer.Enums;
using K9.SharedLibrary.Models;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using Moq;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class BioRhythmsServiceTests
    {
        public BioRhythmsServiceTests()
        {
        }

        [Theory]
        [InlineData(1979, 06, 16, 1979, 06, 16, EGender.Male, ENineStarKiEnergy.Thunder, 0, 0, 50, 50)]
        [InlineData(1979, 06, 16, 1979, 06, 19, EGender.Male, ENineStarKiEnergy.Thunder, 3, 3, 77, 81)]
        [InlineData(1979, 06, 16, 1979, 06, 24, EGender.Male, ENineStarKiEnergy.Thunder, 8, 8, 100, 99)]
        [InlineData(1979, 06, 16, 1979, 06, 26, EGender.Male, ENineStarKiEnergy.Thunder, 10, 10, 97, 89)]
        [InlineData(1979, 06, 16, 1979, 07, 03, EGender.Male, ENineStarKiEnergy.Thunder, 17, 17, 45, 19)]
        [InlineData(1979, 06, 16, 1979, 07, 19, EGender.Male, ENineStarKiEnergy.Thunder, 33, 0, 50, 95)]
        [InlineData(1979, 06, 16, 1979, 08, 02, EGender.Male, ENineStarKiEnergy.Thunder, 47, 14, 73, 5)]
        public void Biorhythms_HappyPath(int birthYear, int birthMonth, int birthDay, int dateYear, int dateMonth, int dateDay, EGender gender, ENineStarKiEnergy expectedEnergy, int expectedDaysElapsedSinceBirth, int expectedDayInterval, double expectedIntellectualValue, double expectedEmotionalValue)
        {
            var biorhythmsService = new BiorhythmsService(new Mock<IRoles>().Object, new Mock<IMembershipService>().Object, new Mock<IAuthentication>().Object);

            var result = biorhythmsService.Calculate(new NineStarKiModel(
                new PersonModel
                {
                    DateOfBirth = new DateTime(birthYear, birthMonth, birthDay)
                }), new DateTime(dateYear, dateMonth, dateDay));

            Assert.Equal(expectedEnergy, result.NineStarKiModel.MainEnergy.Energy);
            Assert.Equal(expectedDaysElapsedSinceBirth, result.DaysElapsedSinceBirth);
            Assert.Equal(expectedDayInterval, result.IntellectualBiorhythmResult.DayInterval);
            Assert.Equal(expectedIntellectualValue, Math.Round(result.IntellectualBiorhythmResult.Value, MidpointRounding.ToEven));
            Assert.Equal(expectedEmotionalValue, Math.Round(result.EmotionalBiorhythmResult.Value, MidpointRounding.ToEven));
        }

        [Theory]
        [InlineData(1979, 06, 16, 1979, 06, 16, EGender.Male, ENineStarKiEnergy.Thunder, 50, 55.9976)]
        public void NineStarKiBiorhythms_PhysicalEnergy_HappyPath(int birthYear, int birthMonth, int birthDay, int dateYear, int dateMonth, int dateDay, EGender gender, ENineStarKiEnergy expectedEnergy, double expectedBiorhythmValue, double expectedNineStarKiBiorhythmsValue)
        {
            var biorhythmsService = new BiorhythmsService(new Mock<IRoles>().Object, new Mock<IMembershipService>().Object, new Mock<IAuthentication>().Object);

            var result = biorhythmsService.Calculate(new NineStarKiModel(
                new PersonModel
                {
                    DateOfBirth = new DateTime(birthYear, birthMonth, birthDay)
                }), new DateTime(dateYear, dateMonth, dateDay));

            Assert.Equal(expectedEnergy, result.NineStarKiModel.MainEnergy.Energy);
            Assert.Equal(expectedBiorhythmValue, result.NineStarKiModel.Biorhythms.PhysicalBiorhythmResult.Value);
            Assert.Equal(expectedNineStarKiBiorhythmsValue, Math.Round(result.NineStarKiModel.NineStarKiBiorhythms.PhysicalBiorhythmResult.Value, 4));
        }

    }
}
