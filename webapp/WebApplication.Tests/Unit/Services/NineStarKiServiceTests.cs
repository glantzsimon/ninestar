using K9.Base.DataAccessLayer.Enums;
using K9.SharedLibrary.Models;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using Moq;
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
        [InlineData(1979, ENineStarKiEnergy.Thunder, EGender.Male)]
        [InlineData(1980, ENineStarKiEnergy.Soil, EGender.Male)]
        [InlineData(1981, ENineStarKiEnergy.Water, EGender.Male)]
        [InlineData(1982, ENineStarKiEnergy.Fire, EGender.Male)]
        [InlineData(1983, ENineStarKiEnergy.Mountain, EGender.Male)]
        [InlineData(1984, ENineStarKiEnergy.Lake, EGender.Male)]
        [InlineData(1985, ENineStarKiEnergy.Heaven, EGender.Male)]
        [InlineData(1986, ENineStarKiEnergy.CoreEarth, EGender.Male)]
        [InlineData(1987, ENineStarKiEnergy.Wind, EGender.Male)]
        public void YearEnergy_HappyPath(int year, ENineStarKiEnergy energy, EGender gender)
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
        public void YearEnergyFemale_HappyPath(int year, ENineStarKiEnergy energy, EGender gender)
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
        [InlineData(1980, 1977, 1974, 2, 3, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Thunder, EGender.Male)]
        public void MonthEnergy_HappyPath(int year, int year2, int year3, int month, int day, ENineStarKiEnergy year1Energy, ENineStarKiEnergy year2Energy, ENineStarKiEnergy year3Energy, ENineStarKiEnergy monthEnergy, EGender gender)
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
        [InlineData(1980, 1977, 1974, 2, 3, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Thunder, EGender.Female)]
        public void MonthEnergy_Yin_HappyPath(int year, int year2, int year3, int month, int day, ENineStarKiEnergy year1Energy, ENineStarKiEnergy year2Energy, ENineStarKiEnergy year3Energy, ENineStarKiEnergy monthEnergy, EGender gender)
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
        public void SurfaceEnergy_HappyPath(int year, int month, int day, ENineStarKiEnergy yearEnergy, ENineStarKiEnergy monthEnergy, ENineStarKiEnergy surfaceEnergy, EGender gender)
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
        [InlineData(1979, 6, 16, 2020, 12, 14, EGender.Male, ENineStarKiEnergy.Water, ENineStarKiEnergy.Lake)]
        [InlineData(1979, 6, 16, 2020, 12, 14, EGender.Female, ENineStarKiEnergy.CoreEarth, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1980, 6, 16, 2020, 12, 14, EGender.Male, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Wind)]
        [InlineData(1978, 6, 16, 2020, 12, 14, EGender.Male, ENineStarKiEnergy.Soil, ENineStarKiEnergy.Water)]
        [InlineData(1978, 6, 16, 2020, 12, 14, EGender.Female, ENineStarKiEnergy.Wind, ENineStarKiEnergy.Mountain)]
        [InlineData(1980, 6, 16, 2020, 12, 14, EGender.Female, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Soil)]
        public void LifeCycle_HappyPath(int birthYear, int birthMonth, int birthDay, int year, int month, int day, EGender gender, ENineStarKiEnergy yearlyCycleEnergy, ENineStarKiEnergy monthlyCycleEnergy)
        {
            var ninestar = new NineStarKiModel(new PersonModel
            {
                DateOfBirth = new DateTime(birthYear, birthMonth, birthDay),
                Gender = gender
            })
            {
                Today = new DateTime(year, month, day)
            };

            Assert.Equal(yearlyCycleEnergy, ninestar.LifeCycleYearEnergy);
            Assert.Equal(monthlyCycleEnergy, ninestar.LifeCycleMonthEnergy);
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
        [InlineData(1979, 6, 16, EGender.Male, 1984, 6, 21, EGender.Male, ECompatibilityScore.ExtremelyHigh)]
        [InlineData(1981, 6, 16, EGender.Male, 1984, 6, 21, EGender.Male, ECompatibilityScore.MediumToHigh)]
        [InlineData(1980, 6, 16, EGender.Male, 1984, 6, 21, EGender.Male, ECompatibilityScore.Medium)]
        [InlineData(1979, 6, 16, EGender.Male, 1983, 6, 21, EGender.Male, ECompatibilityScore.VeryHigh)]
        [InlineData(1979, 6, 16, EGender.Male, 1985, 6, 21, EGender.Male, ECompatibilityScore.High)]
        [InlineData(1979, 6, 16, EGender.Male, 1978, 6, 21, EGender.Male, ECompatibilityScore.Medium)]
        [InlineData(1979, 6, 16, EGender.Male, 1979, 6, 21, EGender.Male, ECompatibilityScore.ExtremelyLow)]
        [InlineData(1979, 6, 16, EGender.Male, 1982, 6, 21, EGender.Male, ECompatibilityScore.MediumToHigh)]
        [InlineData(1979, 6, 16, EGender.Male, 1980, 6, 21, EGender.Male, ECompatibilityScore.ExtremelyHigh)]
        [InlineData(1982, 6, 16, EGender.Male, 1985, 6, 21, EGender.Male, ECompatibilityScore.VeryHigh)]
        public void Calculate_ChemistryLevel(int year1, int month1, int day1, EGender gender1, int year2, int month2, int day2, EGender gender2, ECompatibilityScore chemistryScore)
        {
            var nineStarKiService = new NineStarKiService(new Mock<IMembershipService>().Object, new Mock<IAuthentication>().Object, new Mock<IRoles>().Object);

            Assert.Equal(chemistryScore, nineStarKiService.CalculateCompatibility(new PersonModel
            {
                DateOfBirth = new DateTime(year1, month1, day1),
                Gender = gender1
            }, new PersonModel
            {
                DateOfBirth = new DateTime(year2, month2, day2),
                Gender = gender2
            }).FundamentalEnergyChemistryScore);
        }

    }
}
