using K9.Base.DataAccessLayer.Enums;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using Moq;
using System;
using System.Diagnostics;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class NineStarKiServiceTests
    {
        private NineStarKiService _nineStarKiService;
        private SwissEphemerisService _swissEphemerisService;

        public NineStarKiServiceTests()
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
        [InlineData(1979, ENineStarKiEnergy.Thunder, EGender.Male)]
        [InlineData(1980, ENineStarKiEnergy.Soil, EGender.Male)]
        [InlineData(1981, ENineStarKiEnergy.Water, EGender.Male)]
        [InlineData(1982, ENineStarKiEnergy.Fire, EGender.Male)]
        [InlineData(1983, ENineStarKiEnergy.Mountain, EGender.Male)]
        [InlineData(1984, ENineStarKiEnergy.Lake, EGender.Male)]
        [InlineData(1985, ENineStarKiEnergy.Heaven, EGender.Male)]
        [InlineData(1986, ENineStarKiEnergy.CoreEarth, EGender.Male)]
        [InlineData(1987, ENineStarKiEnergy.Wind, EGender.Male)]
        public void YearEnergy_Test(int year, ENineStarKiEnergy energy, EGender gender)
        {
            var ninestar = new NineStarKiModel(new PersonModel
            {
                DateOfBirth = new DateTime(year, 2, 4),
                Gender = gender
            });
            Assert.Equal(energy, ninestar.MainEnergy.Energy);
        }

        [Theory]
        [InlineData(1979, ENineStarKiEnergy.Thunder, EGender.Female)]
        [InlineData(1980, ENineStarKiEnergy.Wind, EGender.Female)]
        [InlineData(1982, ENineStarKiEnergy.Heaven, EGender.Female)]
        [InlineData(1983, ENineStarKiEnergy.Lake, EGender.Female)]
        [InlineData(1984, ENineStarKiEnergy.Mountain, EGender.Female)]
        [InlineData(1985, ENineStarKiEnergy.Fire, EGender.Other)]
        [InlineData(1986, ENineStarKiEnergy.Water, EGender.Female)]
        [InlineData(1987, ENineStarKiEnergy.Soil, EGender.Female)]
        public void YearEnergyFemale_Test(int year, ENineStarKiEnergy energy, EGender gender)
        {
            var ninestar = new NineStarKiModel(new PersonModel
            {
                DateOfBirth = new DateTime(year, 2, 4),
                Gender = gender
            });
            Assert.Equal(energy, ninestar.MainEnergy.Energy);
        }

        [Fact]
        public void YearEnergyBeforeFeb4_Test()
        {
            var ninestar = new NineStarKiModel(new PersonModel
            {
                DateOfBirth = new DateTime(1979, 2, 3),
                Gender = EGender.Male
            });
            Assert.Equal(ENineStarKiEnergy.Wind, ninestar.MainEnergy.Energy);
        }

        [Theory]
        [InlineData(1979, 1976, 1973, 2, 4, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.CoreEarth, EGender.Male)]
        [InlineData(1979, 1976, 1973, 3, 6, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Wind, EGender.Male)]
        [InlineData(1979, 1976, 1973, 4, 5, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Thunder, EGender.Male)]
        [InlineData(1979, 1976, 1973, 5, 5, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Soil, EGender.Male)]
        [InlineData(1979, 1976, 1973, 6, 6, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Water, EGender.Male)]
        [InlineData(1979, 1976, 1973, 7, 7, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Fire, EGender.Male)]
        [InlineData(1979, 1976, 1973, 8, 7, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Mountain, EGender.Male)]
        [InlineData(1979, 1976, 1973, 9, 8, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Lake, EGender.Male)]
        [InlineData(1979, 1976, 1973, 10, 8, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, EGender.Male)]
        [InlineData(1979, 1976, 1973, 11, 7, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.CoreEarth, EGender.Male)]
        [InlineData(1979, 1976, 1973, 12, 7, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Wind, EGender.Male)]
        [InlineData(1980, 1977, 1974, 1, 5, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Thunder, EGender.Male)]
        [InlineData(1980, 1977, 1974, 2, 3, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Thunder, EGender.Male)]
        [InlineData(1979, 1976, 1973, 2, 3, ENineStarKiEnergy.Wind, ENineStarKiEnergy.Lake, ENineStarKiEnergy.Water, ENineStarKiEnergy.Heaven, EGender.Male)]
        [InlineData(1979, 1976, 1973, 3, 5, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.CoreEarth, EGender.Male)]
        [InlineData(1979, 1976, 1973, 4, 4, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Wind, EGender.Male)]
        [InlineData(1979, 1976, 1973, 5, 4, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Thunder, EGender.Male)]
        [InlineData(1979, 1976, 1973, 6, 5, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Soil, EGender.Male)]
        [InlineData(1979, 1976, 1973, 7, 6, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Water, EGender.Male)]
        [InlineData(1979, 1976, 1973, 8, 6, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Fire, EGender.Male)]
        [InlineData(1979, 1976, 1973, 9, 7, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Mountain, EGender.Male)]
        [InlineData(1979, 1976, 1973, 10, 7, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Lake, EGender.Male)]
        [InlineData(1979, 1976, 1973, 11, 6, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, EGender.Male)]
        [InlineData(1979, 1976, 1973, 12, 6, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.CoreEarth, EGender.Male)]
        [InlineData(1980, 1977, 1974, 1, 4, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Wind, EGender.Male)]
        public void MonthEnergy_Test(int year, int year2, int year3, int month, int day, ENineStarKiEnergy year1Energy, ENineStarKiEnergy year2Energy, ENineStarKiEnergy year3Energy, ENineStarKiEnergy monthEnergy, EGender gender)
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
        [InlineData(1979, 1976, 1973, 2, 4, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Water, EGender.Female)]
        [InlineData(1979, 1976, 1973, 3, 6, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Soil, EGender.Female)]
        [InlineData(1979, 1976, 1973, 4, 5, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Thunder, EGender.Female)]
        [InlineData(1979, 1976, 1973, 5, 5, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Wind, EGender.Female)]
        [InlineData(1979, 1976, 1973, 6, 6, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.CoreEarth, EGender.Female)]
        [InlineData(1979, 1976, 1973, 7, 7, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Heaven, EGender.Female)]
        [InlineData(1979, 1976, 1973, 8, 7, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Lake, EGender.Female)]
        [InlineData(1979, 1976, 1973, 9, 8, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Mountain, EGender.Female)]
        [InlineData(1979, 1976, 1973, 10, 8, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, EGender.Female)]
        [InlineData(1979, 1976, 1973, 11, 7, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Water, EGender.Female)]
        [InlineData(1979, 1976, 1973, 12, 7, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Soil, EGender.Female)]
        [InlineData(1980, 1977, 1974, 1, 5, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Thunder, EGender.Female)]
        [InlineData(1980, 1977, 1974, 2, 3, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Thunder, EGender.Female)]
        [InlineData(1979, 1976, 1973, 2, 3, ENineStarKiEnergy.Soil, ENineStarKiEnergy.Mountain, ENineStarKiEnergy.CoreEarth, ENineStarKiEnergy.Heaven, EGender.Female)]
        [InlineData(1979, 1976, 1973, 3, 5, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Water, EGender.Female)]
        [InlineData(1979, 1976, 1973, 4, 4, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Soil, EGender.Female)]
        [InlineData(1979, 1976, 1973, 5, 4, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Thunder, EGender.Female)]
        [InlineData(1979, 1976, 1973, 6, 5, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Wind, EGender.Female)]
        [InlineData(1979, 1976, 1973, 7, 6, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.CoreEarth, EGender.Female)]
        [InlineData(1979, 1976, 1973, 8, 6, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Heaven, EGender.Female)]
        [InlineData(1979, 1976, 1973, 9, 7, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Lake, EGender.Female)]
        [InlineData(1979, 1976, 1973, 10, 7, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Mountain, EGender.Female)]
        [InlineData(1979, 1976, 1973, 11, 6, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, EGender.Female)]
        [InlineData(1979, 1976, 1973, 12, 6, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Water, EGender.Female)]
        [InlineData(1980, 1977, 1974, 1, 4, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Soil, EGender.Female)]
        public void MonthEnergy_Yin_Test(int year, int year2, int year3, int month, int day, ENineStarKiEnergy year1Energy, ENineStarKiEnergy year2Energy, ENineStarKiEnergy year3Energy, ENineStarKiEnergy monthEnergy, EGender gender)
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
        [InlineData(1979, 2, 4, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.CoreEarth, ENineStarKiEnergy.Thunder, EGender.Male)]
        [InlineData(1976, 3, 6, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Wind, ENineStarKiEnergy.Lake, EGender.Male)]
        [InlineData(1973, 4, 5, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Soil, EGender.Male)]
        [InlineData(1979, 5, 5, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Soil, ENineStarKiEnergy.Heaven, EGender.Male)]
        [InlineData(1976, 6, 6, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Water, ENineStarKiEnergy.Water, EGender.Male)]
        [InlineData(1980, 7, 7, ENineStarKiEnergy.Soil, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Water, EGender.Male)]
        [InlineData(1981, 8, 7, ENineStarKiEnergy.Water, ENineStarKiEnergy.Soil, ENineStarKiEnergy.Wind, EGender.Male)]
        [InlineData(1982, 9, 8, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Lake, ENineStarKiEnergy.Lake, EGender.Male)]
        [InlineData(1983, 10, 8, ENineStarKiEnergy.Mountain, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Water, EGender.Male)]
        [InlineData(1984, 11, 7, ENineStarKiEnergy.Lake, ENineStarKiEnergy.Mountain, ENineStarKiEnergy.Wind, EGender.Male)]
        [InlineData(1985, 3, 5, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.CoreEarth, ENineStarKiEnergy.Heaven, EGender.Male)]
        [InlineData(1986, 12, 7, ENineStarKiEnergy.CoreEarth, ENineStarKiEnergy.Water, ENineStarKiEnergy.Fire, EGender.Male)]
        [InlineData(1987, 2, 4, ENineStarKiEnergy.Wind, ENineStarKiEnergy.Mountain, ENineStarKiEnergy.Water, EGender.Male)]
        [InlineData(1988, 3, 6, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Wind, ENineStarKiEnergy.Wind, EGender.Male)]
        public void SurfaceEnergy_Test(int year, int month, int day, ENineStarKiEnergy yearEnergy, ENineStarKiEnergy monthEnergy, ENineStarKiEnergy surfaceEnergy, EGender gender)
        {
            var ninestar = new NineStarKiModel(new PersonModel
            {
                DateOfBirth = new DateTime(year, month, day),
                Gender = gender
            });

            Assert.Equal(yearEnergy, ninestar.MainEnergy.Energy);
            Assert.Equal(monthEnergy, ninestar.CharacterEnergy.Energy);
            Assert.Equal(surfaceEnergy, ninestar.SurfaceEnergy.Energy);
        }

        [Theory]
        [InlineData(1979, 6, 16, 2011, 12, 14, EGender.Male, ENineStarKiEnergy.Water, ENineStarKiEnergy.Lake)]
        [InlineData(1979, 6, 16, 2011, 12, 14, EGender.Female, ENineStarKiEnergy.CoreEarth, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1980, 6, 16, 2011, 12, 14, EGender.Male, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Wind)]
        [InlineData(1978, 6, 16, 2011, 12, 14, EGender.Male, ENineStarKiEnergy.Soil, ENineStarKiEnergy.Water)]
        [InlineData(1978, 6, 16, 2011, 12, 14, EGender.Female, ENineStarKiEnergy.Wind, ENineStarKiEnergy.Mountain)]
        [InlineData(1980, 6, 16, 2011, 12, 14, EGender.Female, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Soil)]
        public void LifeCycle_Test(int birthYear, int birthMonth, int birthDay, int year, int month, int day, EGender gender, ENineStarKiEnergy yearlyCycleEnergy, ENineStarKiEnergy monthlyCycleEnergy)
        {
            var ninestar = new NineStarKiModel(new PersonModel
            {
                DateOfBirth = new DateTime(birthYear, birthMonth, birthDay),
                Gender = gender
            }, new DateTime(year, month, day));

            Assert.Equal(yearlyCycleEnergy, ninestar.YearlyCycleEnergy.Energy);
            Assert.Equal(monthlyCycleEnergy, ninestar.MonthlyCycleEnergy.Energy);
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
                    Debug.WriteLine($"{energy.MainEnergy.EnergyNumber} - {energy.CharacterEnergy.EnergyNumber} - {energy.SurfaceEnergy.EnergyNumber}");
                    dobMonth++;
                }
                dobYear++;
            }
        }

        [Theory]
        [InlineData(1977, ENineStarKiEnergy.CoreEarth, EGender.Male,
            1977, ENineStarKiEnergy.CoreEarth, ENineStarKiDirection.Centre,
            ENineStarKiEnergy.CoreEarth, ENineStarKiDirection.Centre)]

        [InlineData(1978, ENineStarKiEnergy.Wind, EGender.Male,
            1978, ENineStarKiEnergy.CoreEarth, ENineStarKiDirection.Centre,
            ENineStarKiEnergy.Heaven, ENineStarKiDirection.NorthWest)]

        [InlineData(1979, ENineStarKiEnergy.Thunder, EGender.Male,
            1979, ENineStarKiEnergy.CoreEarth, ENineStarKiDirection.Centre,
            ENineStarKiEnergy.Lake, ENineStarKiDirection.West)]

        [InlineData(1991, ENineStarKiEnergy.Fire, EGender.Male,
            1991, ENineStarKiEnergy.CoreEarth, ENineStarKiDirection.Centre,
            ENineStarKiEnergy.Water, ENineStarKiDirection.North)]

        [InlineData(1991, ENineStarKiEnergy.Fire, EGender.Male,
            1990, ENineStarKiEnergy.Wind, ENineStarKiDirection.SouthEast,
            ENineStarKiEnergy.Fire, ENineStarKiDirection.South)]

        [InlineData(1991, ENineStarKiEnergy.Fire, EGender.Male,
            1989, ENineStarKiEnergy.Thunder, ENineStarKiDirection.East,
            ENineStarKiEnergy.Mountain, ENineStarKiDirection.NorthEast)]

        [InlineData(1991, ENineStarKiEnergy.Heaven, EGender.Female,
            1991, ENineStarKiEnergy.Water, ENineStarKiDirection.North,
            ENineStarKiEnergy.Fire, ENineStarKiDirection.South)]

        [InlineData(1991, ENineStarKiEnergy.Heaven, EGender.Female,
            1990, ENineStarKiEnergy.Soil, ENineStarKiDirection.SouthWest,
            ENineStarKiEnergy.Water, ENineStarKiDirection.North)]
        public void CalculateDirectionForYear_Test(
            int birthYear,
            ENineStarKiEnergy energy,
            EGender gender,
            int todayYear,
            ENineStarKiEnergy yearlyCycleEnergy,
            ENineStarKiDirection yearlyCycleDirection,
            ENineStarKiEnergy coreEarthYearlyCycleEnergy,
            ENineStarKiDirection coreEarthYearlyCycleDirection)
        {
            var ninestar = _nineStarKiService.CalculateNineStarKiProfile(
                new PersonModel
                {
                    DateOfBirth = new DateTime(birthYear, 2, 5),
                    Gender = gender,
                },
                false,
                false,
                new DateTime(todayYear, 2, 5));


            Assert.Equal(energy, ninestar.MainEnergy.Energy);
            Assert.Equal(yearlyCycleEnergy, ninestar.YearlyCycleEnergy.Energy);
            Assert.Equal(yearlyCycleDirection, ninestar.YearlyCycleEnergy.Direction);
            Assert.Equal(coreEarthYearlyCycleEnergy, ninestar.YearlyCycleCoreEarthEnergy.Energy);
            Assert.Equal(coreEarthYearlyCycleDirection, ninestar.YearlyCycleCoreEarthEnergy.Direction);
        }

        [Theory]
        [InlineData(1977, ENineStarKiEnergy.CoreEarth, EGender.Male,
            1977, ENineStarKiEnergy.CoreEarth, ENineStarKiDirection.Centre,
            3, ENineStarKiEnergy.Fire, ENineStarKiDirection.South)]

        [InlineData(1978, ENineStarKiEnergy.Wind, EGender.Male,
            1978, ENineStarKiEnergy.CoreEarth, ENineStarKiDirection.Centre,
            2, ENineStarKiEnergy.Mountain, ENineStarKiDirection.NorthEast)]

        [InlineData(1979, ENineStarKiEnergy.Thunder, EGender.Male,
            1979, ENineStarKiEnergy.CoreEarth, ENineStarKiDirection.Centre,
            3, ENineStarKiEnergy.Fire, ENineStarKiDirection.South)]

        [InlineData(1991, ENineStarKiEnergy.Fire, EGender.Male,
            1991, ENineStarKiEnergy.CoreEarth, ENineStarKiDirection.Centre,
            5, ENineStarKiEnergy.Soil, ENineStarKiDirection.SouthWest)]

        [InlineData(1991, ENineStarKiEnergy.Fire, EGender.Male,
            1990, ENineStarKiEnergy.Wind, ENineStarKiDirection.SouthEast,
            6, ENineStarKiEnergy.Heaven, ENineStarKiDirection.NorthWest)]

        [InlineData(1991, ENineStarKiEnergy.Fire, EGender.Male,
            1991, ENineStarKiEnergy.CoreEarth, ENineStarKiDirection.Centre,
            3, ENineStarKiEnergy.Fire, ENineStarKiDirection.South)]

        [InlineData(1991, ENineStarKiEnergy.Fire, EGender.Male,
            1991, ENineStarKiEnergy.CoreEarth, ENineStarKiDirection.Centre,
            7, ENineStarKiEnergy.Wind, ENineStarKiDirection.SouthEast)]

        [InlineData(1991, ENineStarKiEnergy.Fire, EGender.Male,
            1989, ENineStarKiEnergy.Thunder, ENineStarKiDirection.East,
            3, ENineStarKiEnergy.Heaven, ENineStarKiDirection.NorthWest)]

        [InlineData(1991, ENineStarKiEnergy.Heaven, EGender.Female,
            1991, ENineStarKiEnergy.Water, ENineStarKiDirection.North,
            7, ENineStarKiEnergy.Lake, ENineStarKiDirection.West)]

        [InlineData(1991, ENineStarKiEnergy.Heaven, EGender.Female,
            1990, ENineStarKiEnergy.Soil, ENineStarKiDirection.SouthWest,
            3, ENineStarKiEnergy.CoreEarth, ENineStarKiDirection.Centre)]
        public void CalculateDirectionForMonth_Test(
            int birthYear,
            ENineStarKiEnergy energy,
            EGender gender,
            int todayYear,
            ENineStarKiEnergy yearlyCycleEnergy,
            ENineStarKiDirection yearlyCycleDirection,
            int monthNumber,
            ENineStarKiEnergy coreEarthMonthlyCycleEnergy,
            ENineStarKiDirection coreEarthMonthlyCycleDirection)
        {
            var ninestar = _nineStarKiService.CalculateNineStarKiProfile(
                new PersonModel
                {
                    DateOfBirth = new DateTime(birthYear, 2, 5),
                    Gender = gender,
                },
                false,
                false,
                new DateTime(todayYear, monthNumber, 15));


            Assert.Equal(energy, ninestar.MainEnergy.Energy);
            Assert.Equal(yearlyCycleEnergy, ninestar.YearlyCycleEnergy.Energy);
            Assert.Equal(yearlyCycleDirection, ninestar.YearlyCycleEnergy.Direction);
            Assert.Equal(coreEarthMonthlyCycleEnergy, ninestar.MonthlyCycleCoreEarthEnergy.Energy);
            Assert.Equal(coreEarthMonthlyCycleDirection, ninestar.MonthlyCycleCoreEarthEnergy.Direction);
        }

        [Theory]
        [InlineData(1979, ENineStarKiEnergy.Thunder, EGender.Male,
            1979, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1979, ENineStarKiEnergy.Thunder, EGender.Male,
            1980, ENineStarKiEnergy.Heaven)]
        [InlineData(1979, ENineStarKiEnergy.Thunder, EGender.Male,
            2102, ENineStarKiEnergy.Soil)]
        [InlineData(1979, ENineStarKiEnergy.Thunder, EGender.Male,
            2103, ENineStarKiEnergy.Thunder)]
        [InlineData(1979, ENineStarKiEnergy.Thunder, EGender.Male,
            2104, ENineStarKiEnergy.Wind)]
        [InlineData(1979, ENineStarKiEnergy.Thunder, EGender.Male,
            2105, ENineStarKiEnergy.Water)]
        [InlineData(1980, ENineStarKiEnergy.Soil, EGender.Male,
            2104, ENineStarKiEnergy.Thunder)]
        [InlineData(1980, ENineStarKiEnergy.Soil, EGender.Male,
            2105, ENineStarKiEnergy.Soil)]
        public void CalcualteYearlyCycleWithCycleChange_Test(
            int birthYear,
            ENineStarKiEnergy energy,
            EGender gender,
            int todayYear,
            ENineStarKiEnergy yearlyCycleEnergy)
        {

            var ninestar = _nineStarKiService.CalculateNineStarKiProfile(
                new PersonModel
                {
                    DateOfBirth = new DateTime(birthYear, 2, 5),
                    Gender = gender,
                },
                false,
                false,
                new DateTime(todayYear, 2, 5));

            Assert.Equal(energy, ninestar.MainEnergy.Energy);
            Assert.Equal(yearlyCycleEnergy, ninestar.YearlyCycleEnergy.Energy);
        }

        [Theory]
        [InlineData(1979, ENineStarKiEnergy.Thunder, EGender.Male,
            2096, ENineStarKiEnergy.CoreEarth, 2, ENineStarKiEnergy.Soil)]
        [InlineData(1979, ENineStarKiEnergy.Thunder, EGender.Male,
            2102, ENineStarKiEnergy.Soil, 2, ENineStarKiEnergy.Soil)]
        [InlineData(1979, ENineStarKiEnergy.Thunder, EGender.Male,
            2104, ENineStarKiEnergy.Wind, 2, ENineStarKiEnergy.Mountain)]
        [InlineData(1979, ENineStarKiEnergy.Thunder, EGender.Male,
            2105, ENineStarKiEnergy.Water, 2, ENineStarKiEnergy.Lake)]
        public void CalcualteMonthlyCycleWithCycleChange_Test(
            int birthYear,
            ENineStarKiEnergy energy,
            EGender gender,
            int todayYear,
            ENineStarKiEnergy yearlyCycleEnergy,
            int monthNumber,
            ENineStarKiEnergy monthlyCycleEnergy)
        {

            var ninestar = _nineStarKiService.CalculateNineStarKiProfile(
                new PersonModel
                {
                    DateOfBirth = new DateTime(birthYear, 2, 7),
                    Gender = gender,
                },
                false,
                false,
                new DateTime(todayYear, monthNumber, 15));

            Assert.Equal(energy, ninestar.MainEnergy.Energy);
            Assert.Equal(yearlyCycleEnergy, ninestar.YearlyCycleEnergy.Energy);
            Assert.Equal(monthlyCycleEnergy, ninestar.MonthlyCycleEnergy.Energy);
        }

        [Theory]
        [InlineData(1979, 6, 16, EGender.Male, ENineStarKiEnergy.Thunder)]
        [InlineData(1980, 6, 16, EGender.Male, ENineStarKiEnergy.Soil)]
        [InlineData(1981, 6, 16, EGender.Male, ENineStarKiEnergy.Water)]
        [InlineData(1982, 6, 16, EGender.Male, ENineStarKiEnergy.Fire)]
        [InlineData(1979, 2, 5, EGender.Male, ENineStarKiEnergy.Thunder)]
        [InlineData(1979, 2, 4, EGender.Male, ENineStarKiEnergy.Wind)]
        [InlineData(1976, 2, 1, EGender.Male, ENineStarKiEnergy.Lake)]
        [InlineData(1985, 9, 7, EGender.Male, ENineStarKiEnergy.Heaven)]
        [InlineData(1979, 2, 4, EGender.Male, ENineStarKiEnergy.Thunder, 23)]
        public void CalcualteSwissEphemeris_Year(
            int birthYear,
            int birthMonth,
            int birthDay,
            EGender gender,
            ENineStarKiEnergy energy,
            int hour = 0)
        {
            var nineStarKiYear = _swissEphemerisService.GetNineStarKiYear(new DateTime(birthYear, birthMonth, birthDay, hour, 0, 0), "Europe/London");

            Assert.Equal((int)energy, nineStarKiYear);
        }

        [Theory]
        [InlineData(1979, 6, 9, EGender.Male, ENineStarKiEnergy.Water)]
        [InlineData(1979, 2, 5, EGender.Male, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1979, 3, 5, EGender.Male, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1979, 3, 6, EGender.Male, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1979, 3, 7, EGender.Male, ENineStarKiEnergy.Wind)]
        [InlineData(1979, 4, 7, EGender.Male, ENineStarKiEnergy.Thunder)]
        [InlineData(1979, 4, 6, EGender.Male, ENineStarKiEnergy.Thunder)]
        [InlineData(1979, 4, 5, EGender.Male, ENineStarKiEnergy.Wind)]
        [InlineData(1985, 9, 7, EGender.Male, ENineStarKiEnergy.Mountain, "Europe/Samara")]
        [InlineData(1979, 2, 4, EGender.Male, ENineStarKiEnergy.CoreEarth, "Europe/London", 23)]
        public void CalcualteSwissEphemeris_Month(
            int birthYear,
            int birthMonth,
            int birthDay,
            EGender gender,
            ENineStarKiEnergy energy,
            string timeZone = "Europe/London",
            int hour = 0)
        {
            var nineStarKiMonth = _swissEphemerisService.GetNineStarKiMonth(new DateTime(birthYear, birthMonth, birthDay, hour, 0, 0), timeZone);

            Assert.Equal((int)energy, nineStarKiMonth);
        }

        [Theory]
        [InlineData(1979, 6, 9, EGender.Male, ENineStarKiEnergy.Water)]
        [InlineData(1979, 2, 5, EGender.Male, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1979, 3, 5, EGender.Male, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1979, 3, 6, EGender.Male, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1979, 3, 7, EGender.Male, ENineStarKiEnergy.Wind)]
        [InlineData(1979, 4, 7, EGender.Male, ENineStarKiEnergy.Thunder)]
        [InlineData(1979, 4, 6, EGender.Male, ENineStarKiEnergy.Thunder)]
        [InlineData(1979, 4, 5, EGender.Male, ENineStarKiEnergy.Wind)]
        [InlineData(1985, 9, 7, EGender.Male, ENineStarKiEnergy.Mountain, "Europe/Samara")]
        [InlineData(1979, 2, 4, EGender.Male, ENineStarKiEnergy.CoreEarth, "Europe/London", 23)]
        public void CalcualteSwissEphemeris_NineStarKi_Month(
            int birthYear,
            int birthMonth,
            int birthDay,
            EGender gender,
            ENineStarKiEnergy energy,
            string timeZone = "Europe/London",
            int hour = 0)
        {
            var ninestar = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = new DateTime(birthYear, birthMonth, birthDay, hour, 0, 0),
                Gender = gender,
                TimeZoneId = timeZone
            });

            Assert.Equal((int)energy, ninestar.CharacterEnergy.EnergyNumber);
        }

        [Theory]
        [InlineData(1979, 6, 9, EGender.Male, ENineStarKiEnergy.Thunder)]
        [InlineData(1980, 6, 9, EGender.Male, ENineStarKiEnergy.Soil)]
        [InlineData(1980, 6, 9, EGender.Female, ENineStarKiEnergy.Wind)]
        [InlineData(1979, 2, 4, EGender.Male, ENineStarKiEnergy.Wind)]
        [InlineData(1979, 2, 4, EGender.Female, ENineStarKiEnergy.Soil)]
        public void CalcualteSwissEphemeris_NineStarKi_Year(
            int birthYear,
            int birthMonth,
            int birthDay,
            EGender gender,
            ENineStarKiEnergy energy,
            string timeZone = "Europe/London",
            int hour = 0)
        {
            var ninestar = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = new DateTime(birthYear, birthMonth, birthDay, hour, 0, 0),
                Gender = gender,
                TimeZoneId = timeZone
            });

            Assert.Equal((int)energy, ninestar.MainEnergy.EnergyNumber);
        }

        [Theory]
        [InlineData(1974, 6, 9, EGender.Male, ENineStarKiEnergy.Mountain, ENineStarKiEnergy.Soil, 1900, 1, 1, ENineStarKiEnergy.Soil)]
        //[InlineData(1974, 6, 9, EGender.Male, ENineStarKiEnergy.Mountain, ENineStarKiEnergy.Soil, 2025, 3, 5, ENineStarKiEnergy.Lake)]
        //[InlineData(1974, 6, 9, EGender.Male, ENineStarKiEnergy.Mountain, ENineStarKiEnergy.Soil, 2025, 7, 30, ENineStarKiEnergy.Fire)]
        public void CalcualteSwissEphemeris_NineStarKi_DailyKi(
            int birthYear,
            int birthMonth,
            int birthDay,
            EGender gender,
            ENineStarKiEnergy energy,
            ENineStarKiEnergy yearlyCycleEnergy,
            int todayYear,
            int todayMonth,
            int todayDay,
            ENineStarKiEnergy dailyKi,
            string timeZone = "Europe/London",
            int birthHour = 0)
        {
            var today = new DateTime(todayYear, todayMonth, todayDay, 12, 0, 0);
            //var ninestar = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            //{
            //    DateOfBirth = new DateTime(birthYear, birthMonth, birthDay, birthHour, 0, 0),
            //    Gender = gender,
            //    TimeZoneId = timeZone
            //}, false, false, today);

            var dayEnergy = _swissEphemerisService.GetNineStarKiDailyKi(today, timeZone);
            Assert.Equal((int)dailyKi, dayEnergy.ki);

            //Assert.Equal((int)energy, ninestar.MainEnergy.EnergyNumber);
            //Assert.Equal((int)yearlyCycleEnergy, ninestar.YearlyCycleEnergy.EnergyNumber);
            //Assert.Equal((int)dailyKi, ninestar.DailyCycleEnergy.EnergyNumber);
        }

        [Theory]
        [InlineData(1900, 1, 1, ENineStarKiEnergy.Soil)]
        [InlineData(1900, 1, 2, ENineStarKiEnergy.Thunder)]
        [InlineData(1900, 1, 5, ENineStarKiEnergy.Heaven)]
        [InlineData(1900, 1, 31, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1900, 2, 1, ENineStarKiEnergy.Heaven)]
        [InlineData(1900, 2, 4, ENineStarKiEnergy.Fire)]
        [InlineData(1900, 2, 28, ENineStarKiEnergy.Heaven)]
        [InlineData(1900, 3, 1, ENineStarKiEnergy.Lake)]
        [InlineData(1900, 3, 6, ENineStarKiEnergy.Thunder)]
        [InlineData(1900, 3, 20, ENineStarKiEnergy.Mountain)]
        [InlineData(1900, 3, 24, ENineStarKiEnergy.Thunder)]
        [InlineData(1900, 3, 25, ENineStarKiEnergy.Wind)]
        [InlineData(1900, 3, 26, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1900, 3, 31, ENineStarKiEnergy.Water)]
        [InlineData(1900, 5, 5, ENineStarKiEnergy.Fire)]
        public void CalcualteSwissEphemeris_DailyKi(
            int todayYear,
            int todayMonth,
            int todayDay,
            ENineStarKiEnergy dailyKi)
        {
            var today = new DateTime(todayYear, todayMonth, todayDay, 12, 0, 0);
           
            var dayEnergy = _swissEphemerisService.GetNineStarKiDailyKi(today, "Europe/London");
            Assert.Equal((int)dailyKi, dayEnergy.ki);
        }

        [Theory]
        [InlineData(1900, 6, 20, ENineStarKiEnergy.Wind)]
        //[InlineData(1900, 7, 7, ENineStarKiEnergy.Lake)]
        //[InlineData(2025, 3, 5, ENineStarKiEnergy.Lake)]
        //[InlineData(2025, 7, 30, ENineStarKiEnergy.Fire)]
        public void CalcualteSwissEphemeris_DailyKi_Ascending_Descending(
            int todayYear,
            int todayMonth,
            int todayDay,
            ENineStarKiEnergy dailyKi)
        {
            var today = new DateTime(todayYear, todayMonth, todayDay, 12, 0, 0);
           
            var dayEnergy = _swissEphemerisService.GetNineStarKiDailyKi(today, "Europe/London");
            Assert.Equal((int)dailyKi, dayEnergy.ki);
        }
        
        //[Theory]
        //[InlineData(1979, 6, 16, EGender.Male, 1984, 6, 21, EGender.Male, ECompatibilityScore.ExtremelyHigh)]
        //[InlineData(1981, 6, 16, EGender.Male, 1984, 6, 21, EGender.Male, ECompatibilityScore.MediumToHigh)]
        //[InlineData(1980, 6, 16, EGender.Male, 1984, 6, 21, EGender.Male, ECompatibilityScore.Medium)]
        //[InlineData(1979, 6, 16, EGender.Male, 1983, 6, 21, EGender.Male, ECompatibilityScore.VeryHigh)]
        //[InlineData(1979, 6, 16, EGender.Male, 1985, 6, 21, EGender.Male, ECompatibilityScore.High)]
        //[InlineData(1979, 6, 16, EGender.Male, 1978, 6, 21, EGender.Male, ECompatibilityScore.LowToMedium)]
        //[InlineData(1979, 6, 16, EGender.Male, 1979, 6, 21, EGender.Male, ECompatibilityScore.ExtremelyLow)]
        //[InlineData(1979, 6, 16, EGender.Male, 1982, 6, 21, EGender.Male, ECompatibilityScore.MediumToHigh)]
        //[InlineData(1979, 6, 16, EGender.Male, 1980, 6, 21, EGender.Male, ECompatibilityScore.ExtremelyHigh)]
        //[InlineData(1982, 6, 16, EGender.Male, 1985, 6, 21, EGender.Male, ECompatibilityScore.VeryHigh)]
        //public void Calculate_ChemistryLevel(int year1, int month1, int day1, EGender gender1, int year2, int month2, int day2, EGender gender2, ECompatibilityScore chemistryScore)
        //{
        //    var mockAuthentication = new Mock<IAuthentication>();
        //    mockAuthentication.SetupGet(e => e.CurrentUserId).Returns(2);
        //    mockAuthentication.SetupGet(e => e.IsAuthenticated).Returns(true);

        //    var nineStarKiService = new NineStarKiService(new Mock<IMembershipService>().Object, mockAuthentication.Object, new Mock<IRoles>().Object);

        //    var compatibility = nineStarKiService.CalculateCompatibility(new PersonModel
        //    {
        //        DateOfBirth = new DateTime(year1, month1, day1),
        //        Gender = gender1
        //    }, new PersonModel
        //    {
        //        DateOfBirth = new DateTime(year2, month2, day2),
        //        Gender = gender2
        //    }); 

        //    Assert.Equal(chemistryScore, compatibility.CompatibilityDetails.Score.SparkScore);
        //}

        //[Theory]
        //[InlineData(1979, 6, 16, EGender.Male, 1984, 6, 21, EGender.Male, ECompatibilityScore.ExtremelyHigh)]
        //[InlineData(1981, 6, 16, EGender.Male, 1984, 6, 21, EGender.Male, ECompatibilityScore.ExtremelyLow)]
        //[InlineData(1980, 6, 16, EGender.Male, 1984, 6, 21, EGender.Male, ECompatibilityScore.Low)]
        //[InlineData(1979, 6, 16, EGender.Male, 1983, 6, 21, EGender.Male, ECompatibilityScore.ExtremelyHigh)]
        //[InlineData(1979, 6, 16, EGender.Male, 1985, 6, 21, EGender.Male, ECompatibilityScore.VeryHigh)]
        //[InlineData(1979, 6, 16, EGender.Male, 1978, 6, 21, EGender.Male, ECompatibilityScore.Low)]
        //[InlineData(1979, 6, 16, EGender.Male, 1979, 6, 21, EGender.Male, ECompatibilityScore.ExtremelyLow)]
        //[InlineData(1979, 6, 16, EGender.Male, 1982, 6, 21, EGender.Male, ECompatibilityScore.Low)]
        //[InlineData(1979, 6, 16, EGender.Male, 1980, 6, 21, EGender.Male, ECompatibilityScore.ExtremelyHigh)]
        //[InlineData(1982, 6, 16, EGender.Male, 1985, 6, 21, EGender.Male, ECompatibilityScore.High)]
        //public void Calculate_ConflictLevel(int year1, int month1, int day1, EGender gender1, int year2, int month2, int day2, EGender gender2, ECompatibilityScore conflictScore)
        //{
        //    var mockAuthentication = new Mock<IAuthentication>();
        //    mockAuthentication.SetupGet(e => e.CurrentUserId).Returns(2);
        //    mockAuthentication.SetupGet(e => e.IsAuthenticated).Returns(true);

        //    var nineStarKiService = new NineStarKiService(new Mock<IMembershipService>().Object, mockAuthentication.Object, new Mock<IRoles>().Object);

        //    var compatibility = nineStarKiService.CalculateCompatibility(new PersonModel
        //    {
        //        DateOfBirth = new DateTime(year1, month1, day1),
        //        Gender = gender1
        //    }, new PersonModel
        //    {
        //        DateOfBirth = new DateTime(year2, month2, day2),
        //        Gender = gender2
        //    }); 

        //    Assert.Equal(conflictScore, compatibility.CompatibilityDetails.Score.ConflictScore);
        //}

    }
}
