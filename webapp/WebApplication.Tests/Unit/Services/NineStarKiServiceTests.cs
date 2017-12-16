using K9.WebApplication.Models;
using K9.WebApplication.Services;
using System;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class NineStarKiServiceTests
    {
        [Theory]
        [InlineData(1979, ENineStarEnergy.Thunder, EGender.Male)]
        [InlineData(1980, ENineStarEnergy.Soil, EGender.Male)]
        [InlineData(1981, ENineStarEnergy.Water, EGender.Male)]
        [InlineData(1982, ENineStarEnergy.Fire, EGender.Male)]
        [InlineData(1983, ENineStarEnergy.Mountain, EGender.Male)]
        [InlineData(1984, ENineStarEnergy.Lake, EGender.Male)]
        [InlineData(1985, ENineStarEnergy.Heaven, EGender.Male)]
        [InlineData(1986, ENineStarEnergy.CoreEarth, EGender.Male)]
        [InlineData(1987, ENineStarEnergy.Wind, EGender.Male)]
        public void YearEnergy_HappyPath(int year, ENineStarEnergy energy, EGender gender)
        {
            var ninestar = new NineStarKiModel(new PersonModel
            {
                DateOfBirth = new DateTime(year, 2, 4),
                Gender = gender
            });
            Assert.Equal(energy, ninestar.MainEnergy.Energy);
        }

        [Theory]
        [InlineData(1979, ENineStarEnergy.Thunder, EGender.Female)]
        [InlineData(1980, ENineStarEnergy.Wind, EGender.Female)]
        [InlineData(1981, ENineStarEnergy.CoreEarth, EGender.TransFemale)]
        [InlineData(1982, ENineStarEnergy.Heaven, EGender.Female)]
        [InlineData(1983, ENineStarEnergy.Lake, EGender.Female)]
        [InlineData(1984, ENineStarEnergy.Mountain, EGender.Female)]
        [InlineData(1985, ENineStarEnergy.Fire, EGender.Hermaphrodite)]
        [InlineData(1986, ENineStarEnergy.Water, EGender.Female)]
        [InlineData(1987, ENineStarEnergy.Soil, EGender.Female)]
        public void YearEnergyFemale_HappyPath(int year, ENineStarEnergy energy, EGender gender)
        {
            var ninestar = new NineStarKiModel(new PersonModel
            {
                DateOfBirth = new DateTime(year, 2, 4),
                Gender = gender
            });
            Assert.Equal(energy, ninestar.MainEnergy.Energy);
        }

        [Fact]
        public void YearEnergyBeforeFeb4_HappyPath()
        {
            var ninestar = new NineStarKiModel(new PersonModel
            {
                DateOfBirth = new DateTime(1979, 2, 3),
                Gender = EGender.Male
            });
            Assert.Equal(ENineStarEnergy.Soil, ninestar.MainEnergy.Energy);
        }

        [Theory]
        [InlineData(1979, 1976, 1973, 2, 4, ENineStarEnergy.CoreEarth, EGender.Male)]
        [InlineData(1979, 1976, 1973, 3, 6, ENineStarEnergy.Wind, EGender.Male)]
        [InlineData(1979, 1976, 1973, 4, 5, ENineStarEnergy.Thunder, EGender.Male)]
        [InlineData(1979, 1976, 1973, 5, 5, ENineStarEnergy.Soil, EGender.Male)]
        [InlineData(1979, 1976, 1973, 6, 6, ENineStarEnergy.Water, EGender.Male)]
        [InlineData(1979, 1976, 1973, 7, 7, ENineStarEnergy.Fire, EGender.Male)]
        [InlineData(1979, 1976, 1973, 8, 7, ENineStarEnergy.Mountain, EGender.Male)]
        [InlineData(1979, 1976, 1973, 9, 8, ENineStarEnergy.Lake, EGender.Male)]
        [InlineData(1979, 1976, 1973, 10, 8, ENineStarEnergy.Heaven, EGender.Male)]
        [InlineData(1979, 1976, 1973, 11, 7, ENineStarEnergy.CoreEarth, EGender.Male)]
        [InlineData(1979, 1976, 1973, 12, 7, ENineStarEnergy.Wind, EGender.Male)]
        [InlineData(1980, 1977, 1974, 1, 5, ENineStarEnergy.Thunder, EGender.Male)]
        [InlineData(1980, 1977, 1974, 2, 3, ENineStarEnergy.Thunder, EGender.Male)]
        [InlineData(1979, 1976, 1973, 2, 3, ENineStarEnergy.Heaven, EGender.Male)]
        [InlineData(1979, 1976, 1973, 3, 5, ENineStarEnergy.CoreEarth, EGender.Male)]
        [InlineData(1979, 1976, 1973, 4, 4, ENineStarEnergy.Wind, EGender.Male)]
        [InlineData(1979, 1976, 1973, 5, 4, ENineStarEnergy.Thunder, EGender.Male)]
        [InlineData(1979, 1976, 1973, 6, 5, ENineStarEnergy.Soil, EGender.Male)]
        [InlineData(1979, 1976, 1973, 7, 6, ENineStarEnergy.Water, EGender.Male)]
        [InlineData(1979, 1976, 1973, 8, 6, ENineStarEnergy.Fire, EGender.Male)]
        [InlineData(1979, 1976, 1973, 9, 7, ENineStarEnergy.Mountain, EGender.Male)]
        [InlineData(1979, 1976, 1973, 10, 7, ENineStarEnergy.Lake, EGender.Male)]
        [InlineData(1979, 1976, 1973, 11, 6, ENineStarEnergy.Heaven, EGender.Male)]
        [InlineData(1979, 1976, 1973, 12, 6, ENineStarEnergy.CoreEarth, EGender.Male)]
        [InlineData(1980, 1977, 1974, 1, 4, ENineStarEnergy.Wind, EGender.Male)]
        [InlineData(1980, 1977, 1974, 2, 3, ENineStarEnergy.Thunder, EGender.Male)]
        public void MonthEnergy_HappyPath(int year, int year2, int year3, int month, int day, ENineStarEnergy energy, EGender gender)
        {
            var ninestar = new NineStarKiModel(new PersonModel
            {
                DateOfBirth = new DateTime(year, month, day),
                Gender = gender
            });
            var ninestar2 = new NineStarKiModel(new PersonModel
            {
                DateOfBirth = new DateTime(year2, month, day),
                Gender = gender
            });
            var ninestar3 = new NineStarKiModel(new PersonModel
            {
                DateOfBirth = new DateTime(year3, month, day),
                Gender = gender
            });
            Assert.Equal(energy, ninestar.CharacterEnergy.Energy);
            Assert.Equal(energy, ninestar2.CharacterEnergy.Energy);
            Assert.Equal(energy, ninestar3.CharacterEnergy.Energy);
        }
    }
}
