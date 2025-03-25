using K9.Base.DataAccessLayer.Enums;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using Moq;
using System;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class BioRhythmsServiceTests
    {
        private NineStarKiService _nineStarKiService;
        private SwissEphemerisService _swissEphemerisService;

        public BioRhythmsServiceTests()
        {
            var mockAuthentication = new Mock<IAuthentication>();
            mockAuthentication.SetupGet(e => e.CurrentUserId).Returns(2);
            mockAuthentication.SetupGet(e => e.IsAuthenticated).Returns(true);

            var basePackage = new Mock<INineStarKiBasePackage>();
            basePackage.SetupGet(e => e.Authentication).Returns(mockAuthentication.Object);

            var nineStarKiBasePackage = new Mock<INineStarKiBasePackage>();
            nineStarKiBasePackage.SetupGet(e => e.DefaultValuesConfiguration).Returns(new DefaultValuesConfiguration
            {
                SwephPath = @"c:\workspace\sweph\datafiles"
            });

            _swissEphemerisService = new SwissEphemerisService(nineStarKiBasePackage.Object);
            _nineStarKiService = new NineStarKiService(basePackage.Object, _swissEphemerisService);
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
            var biorhythmsService = new BiorhythmsService(new Mock<IRoles>().Object, new Mock<IMembershipService>().Object, new Mock<IAuthentication>().Object, _swissEphemerisService);

            var nineStarKiModel = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
                {
                    DateOfBirth = new DateTime(birthYear, birthMonth, birthDay)
                });

            var result = biorhythmsService.Calculate(nineStarKiModel, new DateTime(dateYear, dateMonth, dateDay));

            Assert.Equal(expectedEnergy, result.NineStarKiModel.MainEnergy.Energy);
            Assert.Equal(expectedDaysElapsedSinceBirth, result.DaysElapsedSinceBirth);

            var intellectualResult = result.GetResultByType(EBiorhythm.Intellectual);
            var emotionalResult = result.GetResultByType(EBiorhythm.Emotional);

            Assert.Equal(expectedDayInterval, intellectualResult?.DayInterval);
            Assert.Equal(expectedIntellectualValue, Math.Round(intellectualResult.Value, MidpointRounding.ToEven));
            Assert.Equal(expectedEmotionalValue, Math.Round(emotionalResult.Value, MidpointRounding.ToEven));
        }

        [Theory]
        [InlineData(1979, 06, 16, 1979, 06, 16, EGender.Male, ENineStarKiEnergy.Thunder, 50, 50.2254)]
        public void NineStarKiBiorhythms_PhysicalEnergy_HappyPath(int birthYear, int birthMonth, int birthDay, int dateYear, int dateMonth, int dateDay, EGender gender, ENineStarKiEnergy expectedEnergy, double expectedBiorhythmValue, double expectedNineStarKiBiorhythmsValue)
        {
            var biorhythmsService = new BiorhythmsService(new Mock<IRoles>().Object, new Mock<IMembershipService>().Object, new Mock<IAuthentication>().Object, _swissEphemerisService);

            var nineStarKiModel = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
                {
                    DateOfBirth = new DateTime(birthYear, birthMonth, birthDay)
                });
            var result = biorhythmsService.Calculate(nineStarKiModel, new DateTime(dateYear, dateMonth, dateDay));

            var physicalResult = result.GetResultByType(EBiorhythm.Physical);
            var nineStarPhysicalResult = result.GetResultByType(EBiorhythm.Physical);

            Assert.Equal(expectedEnergy, result.NineStarKiModel.MainEnergy.Energy);
            Assert.Equal(expectedBiorhythmValue, physicalResult.Value);
            //Assert.Equal(expectedNineStarKiBiorhythmsValue, Math.Round(nineStarPhysicalResult.Value, 4));
        }

    }
}
