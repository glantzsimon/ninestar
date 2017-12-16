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
            var service = new NineStarKiService();
            var ninestar = service.Calculate(new PersonModel
            {
                DateOfBirth = new DateTime(year, 2, 4),
                Gender = gender
            });
            Assert.Equal(energy, ninestar.MainEnergy.Energy);
        }
    }
}
