using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.WebApplication.Models;
using System;
using System.Diagnostics;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class NineStarKiServiceTests
    {
        public NineStarKiServiceTests()
        {
        }

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
            Assert.Equal(ENineStarEnergy.Wind, ninestar.MainEnergy.Energy);
        }

        [Theory]
        [InlineData(1979, 1976, 1973, 2, 4, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.CoreEarth, EGender.Male)]
        [InlineData(1979, 1976, 1973, 3, 6, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Wind, EGender.Male)]
        [InlineData(1979, 1976, 1973, 4, 5, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Thunder, EGender.Male)]
        [InlineData(1979, 1976, 1973, 5, 5, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Soil, EGender.Male)]
        [InlineData(1979, 1976, 1973, 6, 6, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Water, EGender.Male)]
        [InlineData(1979, 1976, 1973, 7, 7, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Fire, EGender.Male)]
        [InlineData(1979, 1976, 1973, 8, 7, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Mountain, EGender.Male)]
        [InlineData(1979, 1976, 1973, 9, 8, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Lake, EGender.Male)]
        [InlineData(1979, 1976, 1973, 10, 8, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, EGender.Male)]
        [InlineData(1979, 1976, 1973, 11, 7, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.CoreEarth, EGender.Male)]
        [InlineData(1979, 1976, 1973, 12, 7, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Wind, EGender.Male)]
        [InlineData(1980, 1977, 1974, 1, 5, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Thunder, EGender.Male)]
        [InlineData(1980, 1977, 1974, 2, 3, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Thunder, EGender.Male)]
        [InlineData(1979, 1976, 1973, 2, 3, ENineStarEnergy.Wind, ENineStarEnergy.Lake, ENineStarEnergy.Water, ENineStarEnergy.Heaven, EGender.Male)]
        [InlineData(1979, 1976, 1973, 3, 5, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.CoreEarth, EGender.Male)]
        [InlineData(1979, 1976, 1973, 4, 4, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Wind, EGender.Male)]
        [InlineData(1979, 1976, 1973, 5, 4, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Thunder, EGender.Male)]
        [InlineData(1979, 1976, 1973, 6, 5, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Soil, EGender.Male)]
        [InlineData(1979, 1976, 1973, 7, 6, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Water, EGender.Male)]
        [InlineData(1979, 1976, 1973, 8, 6, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Fire, EGender.Male)]
        [InlineData(1979, 1976, 1973, 9, 7, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Mountain, EGender.Male)]
        [InlineData(1979, 1976, 1973, 10, 7, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Lake, EGender.Male)]
        [InlineData(1979, 1976, 1973, 11, 6, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, EGender.Male)]
        [InlineData(1979, 1976, 1973, 12, 6, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.CoreEarth, EGender.Male)]
        [InlineData(1980, 1977, 1974, 1, 4, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Wind, EGender.Male)]
        [InlineData(1980, 1977, 1974, 2, 3, ENineStarEnergy.Thunder, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, ENineStarEnergy.Thunder, EGender.Male)]
        public void MonthEnergy_HappyPath(int year, int year2, int year3, int month, int day, ENineStarEnergy year1Energy, ENineStarEnergy year2Energy, ENineStarEnergy year3Energy, ENineStarEnergy monthEnergy, EGender gender)
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
            Assert.Equal(year1Energy, ninestar.MainEnergy.Energy);
            Assert.Equal(year2Energy, ninestar2.MainEnergy.Energy);
            Assert.Equal(year3Energy, ninestar3.MainEnergy.Energy);

            Assert.Equal(year1Energy, ninestar.MainEnergy.Energy);
            Assert.Equal(year2Energy, ninestar2.MainEnergy.Energy);
            Assert.Equal(year3Energy, ninestar3.MainEnergy.Energy);

            Assert.Equal(year1Energy, ninestar.MainEnergy.Energy);
            Assert.Equal(year2Energy, ninestar2.MainEnergy.Energy);
            Assert.Equal(year3Energy, ninestar3.MainEnergy.Energy);

            Assert.Equal(monthEnergy, ninestar.CharacterEnergy.Energy);
            Assert.Equal(monthEnergy, ninestar2.CharacterEnergy.Energy);
            Assert.Equal(monthEnergy, ninestar3.CharacterEnergy.Energy);
        }

        [Theory]
        [InlineData(1979, 1976, 1973, 2, 4, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Water, EGender.Female)]
        [InlineData(1979, 1976, 1973, 3, 6, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Soil, EGender.Female)]
        [InlineData(1979, 1976, 1973, 4, 5, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Thunder, EGender.Female)]
        [InlineData(1979, 1976, 1973, 5, 5, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Wind, EGender.Female)]
        [InlineData(1979, 1976, 1973, 6, 6, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.CoreEarth, EGender.Female)]
        [InlineData(1979, 1976, 1973, 7, 7, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Heaven, EGender.Female)]
        [InlineData(1979, 1976, 1973, 8, 7, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Lake, EGender.Female)]
        [InlineData(1979, 1976, 1973, 9, 8, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Mountain, EGender.Female)]
        [InlineData(1979, 1976, 1973, 10, 8, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, EGender.Female)]
        [InlineData(1979, 1976, 1973, 11, 7, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Water, EGender.Female)]
        [InlineData(1979, 1976, 1973, 12, 7, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Soil, EGender.Female)]
        [InlineData(1980, 1977, 1974, 1, 5, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Thunder, EGender.Female)]
        [InlineData(1980, 1977, 1974, 2, 3, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Thunder, EGender.Female)]
        [InlineData(1979, 1976, 1973, 2, 3, ENineStarEnergy.Soil, ENineStarEnergy.Mountain, ENineStarEnergy.CoreEarth, ENineStarEnergy.Heaven, EGender.Female)]
        [InlineData(1979, 1976, 1973, 3, 5, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Water, EGender.Female)]
        [InlineData(1979, 1976, 1973, 4, 4, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Soil, EGender.Female)]
        [InlineData(1979, 1976, 1973, 5, 4, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Thunder, EGender.Female)]
        [InlineData(1979, 1976, 1973, 6, 5, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Wind, EGender.Female)]
        [InlineData(1979, 1976, 1973, 7, 6, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.CoreEarth, EGender.Female)]
        [InlineData(1979, 1976, 1973, 8, 6, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Heaven, EGender.Female)]
        [InlineData(1979, 1976, 1973, 9, 7, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Lake, EGender.Female)]
        [InlineData(1979, 1976, 1973, 10, 7, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Mountain, EGender.Female)]
        [InlineData(1979, 1976, 1973, 11, 6, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Fire, EGender.Female)]
        [InlineData(1979, 1976, 1973, 12, 6, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Water, EGender.Female)]
        [InlineData(1980, 1977, 1974, 1, 4, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Soil, EGender.Female)]
        [InlineData(1980, 1977, 1974, 2, 3, ENineStarEnergy.Thunder, ENineStarEnergy.Fire, ENineStarEnergy.Heaven, ENineStarEnergy.Thunder, EGender.Female)]
        public void MonthEnergy_Yin_HappyPath(int year, int year2, int year3, int month, int day, ENineStarEnergy year1Energy, ENineStarEnergy year2Energy, ENineStarEnergy year3Energy, ENineStarEnergy monthEnergy, EGender gender)
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
            Assert.Equal(year1Energy, ninestar.MainEnergy.Energy);
            Assert.Equal(year2Energy, ninestar2.MainEnergy.Energy);
            Assert.Equal(year3Energy, ninestar3.MainEnergy.Energy);

            Assert.Equal(year1Energy, ninestar.MainEnergy.Energy);
            Assert.Equal(year2Energy, ninestar2.MainEnergy.Energy);
            Assert.Equal(year3Energy, ninestar3.MainEnergy.Energy);

            Assert.Equal(year1Energy, ninestar.MainEnergy.Energy);
            Assert.Equal(year2Energy, ninestar2.MainEnergy.Energy);
            Assert.Equal(year3Energy, ninestar3.MainEnergy.Energy);

            Assert.Equal(monthEnergy, ninestar.CharacterEnergy.Energy);
            Assert.Equal(monthEnergy, ninestar2.CharacterEnergy.Energy);
            Assert.Equal(monthEnergy, ninestar3.CharacterEnergy.Energy);
        }

        [Theory]
        [InlineData(1979, 2, 4, ENineStarEnergy.Thunder, ENineStarEnergy.CoreEarth, ENineStarEnergy.Thunder, EGender.Male)]
        [InlineData(1976, 3, 6, ENineStarEnergy.Heaven, ENineStarEnergy.Wind, ENineStarEnergy.Lake, EGender.Male)]
        [InlineData(1973, 4, 5, ENineStarEnergy.Fire, ENineStarEnergy.Thunder, ENineStarEnergy.Soil, EGender.Male)]
        [InlineData(1979, 5, 5, ENineStarEnergy.Thunder, ENineStarEnergy.Soil, ENineStarEnergy.Heaven, EGender.Male)]
        [InlineData(1976, 6, 6, ENineStarEnergy.Heaven, ENineStarEnergy.Water, ENineStarEnergy.Water, EGender.Male)]
        [InlineData(1980, 7, 7, ENineStarEnergy.Soil, ENineStarEnergy.Heaven, ENineStarEnergy.Water, EGender.Male)]
        [InlineData(1981, 8, 7, ENineStarEnergy.Water, ENineStarEnergy.Soil, ENineStarEnergy.Wind, EGender.Male)]
        [InlineData(1982, 9, 8, ENineStarEnergy.Fire, ENineStarEnergy.Lake, ENineStarEnergy.Lake, EGender.Male)]
        [InlineData(1983, 10, 8, ENineStarEnergy.Mountain, ENineStarEnergy.Thunder, ENineStarEnergy.Water, EGender.Male)]
        [InlineData(1984, 11, 7, ENineStarEnergy.Lake, ENineStarEnergy.Mountain, ENineStarEnergy.Wind, EGender.Male)]
        [InlineData(1985, 3, 5, ENineStarEnergy.Heaven, ENineStarEnergy.CoreEarth, ENineStarEnergy.Heaven, EGender.Male)]
        [InlineData(1986, 12, 7, ENineStarEnergy.CoreEarth, ENineStarEnergy.Water, ENineStarEnergy.Fire, EGender.Male)]
        [InlineData(1987, 2, 4, ENineStarEnergy.Wind, ENineStarEnergy.Mountain, ENineStarEnergy.Water, EGender.Male)]
        [InlineData(1988, 3, 6, ENineStarEnergy.Thunder, ENineStarEnergy.Wind, ENineStarEnergy.Wind, EGender.Male)]
        public void RisingEnergy_HappyPath(int year, int month, int day, ENineStarEnergy yearEnergy, ENineStarEnergy monthEnergy, ENineStarEnergy risingEnergy, EGender gender)
        {
            var ninestar = new NineStarKiModel(new PersonModel
            {
                DateOfBirth = new DateTime(year, month, day),
                Gender = gender
            });

            Assert.Equal(yearEnergy, ninestar.MainEnergy.Energy);
            Assert.Equal(monthEnergy, ninestar.CharacterEnergy.Energy);
            Assert.Equal(risingEnergy, ninestar.RisingEnergy.Energy);
        }

        [Fact]
        public void OutputEnergies()
        {
            var dobYear = 1979;
            for (int yearNumber = 0; yearNumber < 9; yearNumber++)
            {
                var dobMonth = 1;
                for (int monthNumber = 0; monthNumber < 9; monthNumber++)
                {
                    var dob = new DateTime(dobYear, dobMonth, 10);
                    var energy = new NineStarKiModel(new PersonModel
                    {
                        Gender = EGender.Male,
                        DateOfBirth = dob
                    });
                    Debug.WriteLine($"{energy.MainEnergy.EnergyNumber} - {energy.CharacterEnergy.EnergyNumber} - {energy.RisingEnergy.EnergyNumber}");
                    dobMonth++;
                }
                dobYear++;
            }
        }

    }
}
