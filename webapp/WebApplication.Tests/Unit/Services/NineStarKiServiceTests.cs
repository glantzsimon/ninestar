using K9.Base.DataAccessLayer.Enums;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.Tests.Unit.Helpers;
using Moq;
using System;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class NineStarKiServiceTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly TestOutputTraceListener _listener;
        private NineStarKiService _nineStarKiService;
        private AstronomyService _astronomyService;

        public NineStarKiServiceTests(ITestOutputHelper output)
        {
            var mockAuthentication = new Mock<IAuthentication>();
            mockAuthentication.SetupGet(e => e.CurrentUserId).Returns(2);
            mockAuthentication.SetupGet(e => e.IsAuthenticated).Returns(true);

            var basePackage = new Mock<INineStarKiBasePackage>();
            basePackage.SetupGet(e => e.Authentication).Returns(mockAuthentication.Object);

            var aiTextMergeService = new Mock<IAITextMergeService>();

            var astrologyService = new Mock<IAstrologyService>();

            var nineStarKiBasePackage = new Mock<INineStarKiBasePackage>();
            nineStarKiBasePackage.SetupGet(e => e.DefaultValuesConfiguration).Returns(new DefaultValuesConfiguration
            {
                SwephPath = @"c:\workspace\sweph\datafiles"
            });

            _output = output;
            _listener = new TestOutputTraceListener(_output);
            Trace.Listeners.Add(_listener);

            _astronomyService = new AstronomyService(nineStarKiBasePackage.Object, _output);
            _nineStarKiService = new NineStarKiService(basePackage.Object, _astronomyService, aiTextMergeService.Object, astrologyService.Object);
        }

        [Theory]
        [InlineData(1979, 6, 16, EGender.Male, ENineStarKiEnergy.Thunder, ETransformationType.Supports)]
        [InlineData(1985, 9, 07, EGender.Male, ENineStarKiEnergy.Lake, ETransformationType.Supports)]
        [InlineData(1976, 5, 01, EGender.Female, ENineStarKiEnergy.Mountain, ETransformationType.Controls)]
        public void ChildNatalHouseTransformation_HappyPath(int year, int month, int day, EGender gender, ENineStarKiEnergy childNatalHouse, ETransformationType childNatalTransformationType)
        {
            var personModel = new PersonModel
            {
                DateOfBirth = new DateTime(year, month, day),
                Gender = gender
            };

            var nineStarKiModel = _nineStarKiService.CalculateNineStarKiProfile(personModel);

            Assert.Equal(childNatalHouse, nineStarKiModel.GetChildNatalHouse().Energy);
            Assert.Equal(childNatalTransformationType, nineStarKiModel.GetChildNatalHouseTransformation());
        }

        [Theory]
        [InlineData(1979, 6, 16, EGender.Male, ESexualityRelationType.MatchMatch)]
        [InlineData(1979, 7, 16, EGender.Male, ESexualityRelationType.MatchOpposite)]
        [InlineData(1984, 6, 21, EGender.Male, ESexualityRelationType.OppositeOpposite)]
        [InlineData(1984, 7, 21, EGender.Male, ESexualityRelationType.OppositeMatch)]
        [InlineData(1984, 6, 21, EGender.Female, ESexualityRelationType.OppositeOpposite, true)]
        public void SexualityRelationType_HappyPath(int year, int month, int day, EGender gender, ESexualityRelationType relationType, bool isDebug = false)
        {
            var personModel = new PersonModel
            {
                DateOfBirth = new DateTime(year, month, day),
                Gender = gender
            };

            if (isDebug)
                Debugger.Break();

            var nineStarKiModel = _nineStarKiService.CalculateNineStarKiProfile(personModel);

            Assert.Equal(relationType, nineStarKiModel.SexualityRelationType);
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
            var ninestar = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = new DateTime(year, 2, 7),
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
            var ninestar = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = new DateTime(year, 2, 15),
                Gender = gender
            });
            Assert.Equal(energy, ninestar.MainEnergy.Energy);
        }

        [Fact]
        public void YearEnergyBeforeFeb4_Test()
        {
            var ninestar = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = new DateTime(1979, 2, 3),
                Gender = EGender.Male
            });
            Assert.Equal(ENineStarKiEnergy.Wind, ninestar.MainEnergy.Energy);
        }

        [Theory]
        [InlineData(1979, 1976, 1973, 2, 5, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.CoreEarth, EGender.Male)]
        [InlineData(1979, 1976, 1973, 3, 7, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Wind, EGender.Male)]
        [InlineData(1979, 1976, 1973, 4, 8, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Thunder, EGender.Male)]
        [InlineData(1979, 1976, 1973, 5, 8, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Soil, EGender.Male)]
        [InlineData(1979, 1976, 1973, 6, 8, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Water, EGender.Male)]
        [InlineData(1979, 1976, 1973, 7, 9, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Fire, EGender.Male)]
        [InlineData(1979, 1976, 1973, 8, 15, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Mountain, EGender.Male)]
        [InlineData(1979, 1976, 1973, 9, 15, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Lake, EGender.Male)]
        [InlineData(1979, 1976, 1973, 10, 15, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, EGender.Male)]
        [InlineData(1979, 1976, 1973, 11, 15, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, ENineStarKiEnergy.CoreEarth, EGender.Male)]
        public void MonthEnergy_Test(int year, int year2, int year3, int month, int day, ENineStarKiEnergy year1Energy, ENineStarKiEnergy year2Energy, ENineStarKiEnergy year3Energy, ENineStarKiEnergy monthEnergy, EGender gender)
        {
            var ninestar = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = new DateTime(year, month, day),
                Gender = gender
            });
            var ninestar2 = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = new DateTime(year2, month, day),
                Gender = gender
            });
            var ninestar3 = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = new DateTime(year3, month, day),
                Gender = gender
            });

            Assert.Equal(year1Energy, ninestar.MainEnergy.Energy);
            Assert.Equal(year2Energy, ninestar2.MainEnergy.Energy);
            Assert.Equal(year3Energy, ninestar3.MainEnergy.Energy);

            Assert.Equal(monthEnergy, ninestar.CharacterEnergy.Energy);
            Assert.Equal(monthEnergy, ninestar2.CharacterEnergy.Energy);
            Assert.Equal(monthEnergy, ninestar3.CharacterEnergy.Energy);
        }

        [Fact]
        public void TestSwissEphemerisService_GetMonthKi()
        {
            var date = new DateTime(1979, 10, 15);
            var result = _astronomyService.GetNineStarKiMonthlyKi(date, "");
            Assert.Equal(6, result);

            date = new DateTime(1979, 11, 15);
            result = _astronomyService.GetNineStarKiMonthlyKi(date, "");
            Assert.Equal(5, result);

            date = new DateTime(1979, 12, 15);
            result = _astronomyService.GetNineStarKiMonthlyKi(date, "");
            Assert.Equal(4, result);

            date = new DateTime(1980, 1, 26);
            result = _astronomyService.GetNineStarKiMonthlyKi(date, "");
            Assert.Equal(3, result);

            date = new DateTime(1980, 2, 3);
            result = _astronomyService.GetNineStarKiMonthlyKi(date, "");
            Assert.Equal(3, result);

            date = new DateTime(1980, 2, 5);
            result = _astronomyService.GetNineStarKiMonthlyKi(date, "");
            Assert.Equal(2, result);
        }

        [Theory]
        [InlineData(1979, 1976, 1973, 2, 6, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Water, EGender.Female)]
        [InlineData(1979, 1976, 1973, 3, 8, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Soil, EGender.Female)]
        [InlineData(1979, 1976, 1973, 4, 8, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Thunder, EGender.Female)]
        [InlineData(1979, 1976, 1973, 5, 9, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Wind, EGender.Female)]
        [InlineData(1979, 1976, 1973, 6, 9, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.CoreEarth, EGender.Female)]
        [InlineData(1979, 1976, 1973, 7, 9, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Heaven, EGender.Female)]
        [InlineData(1979, 1976, 1973, 8, 10, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Lake, EGender.Female)]
        [InlineData(1979, 1976, 1973, 9, 10, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Mountain, EGender.Female)]
        [InlineData(1979, 1976, 1973, 10, 10, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, EGender.Female)]
        [InlineData(1979, 1976, 1973, 11, 10, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Water, EGender.Female)]
        [InlineData(1979, 1976, 1973, 12, 10, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Soil, EGender.Female)]
        [InlineData(1980, 1977, 1974, 1, 10, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Thunder, EGender.Female)]
        [InlineData(1980, 1977, 1974, 2, 2, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Thunder, EGender.Female, true)]
        [InlineData(1979, 1976, 1973, 2, 3, ENineStarKiEnergy.Soil, ENineStarKiEnergy.Mountain, ENineStarKiEnergy.CoreEarth, ENineStarKiEnergy.Heaven, EGender.Female, true)]
        [InlineData(1979, 1976, 1973, 3, 7, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Soil, EGender.Female)]
        [InlineData(1979, 1976, 1973, 6, 5, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Wind, EGender.Female)]
        [InlineData(1979, 1976, 1973, 7, 6, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.CoreEarth, EGender.Female)]
        [InlineData(1979, 1976, 1973, 8, 6, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Heaven, EGender.Female)]
        [InlineData(1979, 1976, 1973, 9, 7, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Lake, EGender.Female)]
        [InlineData(1979, 1976, 1973, 10, 7, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Mountain, EGender.Female)]
        [InlineData(1979, 1976, 1973, 11, 5, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Fire, EGender.Female)]
        [InlineData(1979, 1976, 1973, 12, 5, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Water, EGender.Female)]
        [InlineData(1980, 1977, 1974, 1, 4, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Soil, EGender.Female)]
        public void MonthEnergy_Yin_Test(int year, int year2, int year3, int month, int day, ENineStarKiEnergy year1Energy, ENineStarKiEnergy year2Energy, ENineStarKiEnergy year3Energy, ENineStarKiEnergy monthEnergy, EGender gender, bool isDebug = false)
        {
            if (isDebug)
                Debugger.Break();

            var ninestar = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = new DateTime(year, month, day),
                Gender = gender
            });
            var ninestar2 = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = new DateTime(year2, month, day),
                Gender = gender
            });
            var ninestar3 = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = new DateTime(year3, month, day),
                Gender = gender
            });
            Assert.Equal(year1Energy, ninestar.MainEnergy.Energy);
            Assert.Equal(year2Energy, ninestar2.MainEnergy.Energy);
            Assert.Equal(year3Energy, ninestar3.MainEnergy.Energy);

            Assert.Equal(monthEnergy, ninestar.CharacterEnergy.Energy);
            Assert.Equal(monthEnergy, ninestar2.CharacterEnergy.Energy);
            Assert.Equal(monthEnergy, ninestar3.CharacterEnergy.Energy);
        }

        [Theory]
        [InlineData(1979, 2, 6, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.CoreEarth, ENineStarKiEnergy.Thunder, EGender.Male)]
        [InlineData(1976, 3, 8, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Wind, ENineStarKiEnergy.Lake, EGender.Male)]
        [InlineData(1973, 4, 8, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Soil, EGender.Male)]
        [InlineData(1979, 5, 8, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Soil, ENineStarKiEnergy.Heaven, EGender.Male)]
        [InlineData(1976, 6, 8, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Water, ENineStarKiEnergy.Water, EGender.Male)]
        [InlineData(1980, 7, 8, ENineStarKiEnergy.Soil, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.Water, EGender.Male)]
        [InlineData(1981, 8, 8, ENineStarKiEnergy.Water, ENineStarKiEnergy.Soil, ENineStarKiEnergy.Wind, EGender.Male)]
        [InlineData(1982, 9, 8, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Lake, ENineStarKiEnergy.Lake, EGender.Male)]
        [InlineData(1983, 10, 9, ENineStarKiEnergy.Mountain, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Water, EGender.Male)]
        [InlineData(1984, 11, 7, ENineStarKiEnergy.Lake, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Thunder, EGender.Male)]
        [InlineData(1985, 3, 5, ENineStarKiEnergy.Heaven, ENineStarKiEnergy.CoreEarth, ENineStarKiEnergy.Heaven, EGender.Male)]
        [InlineData(1986, 12, 9, ENineStarKiEnergy.CoreEarth, ENineStarKiEnergy.Water, ENineStarKiEnergy.Fire, EGender.Male)]
        [InlineData(1987, 2, 4, ENineStarKiEnergy.CoreEarth, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Water, EGender.Male)]
        [InlineData(1988, 3, 6, ENineStarKiEnergy.Thunder, ENineStarKiEnergy.Wind, ENineStarKiEnergy.Wind, EGender.Male)]
        public void SurfaceEnergy_Test(int year, int month, int day, ENineStarKiEnergy yearEnergy, ENineStarKiEnergy monthEnergy, ENineStarKiEnergy surfaceEnergy, EGender gender)
        {
            var ninestar = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = new DateTime(year, month, day),
                Gender = gender
            });

            if (yearEnergy != ninestar.MainEnergy.Energy)
            {
                Debugger.Break();
            }
            if (monthEnergy != ninestar.CharacterEnergy.Energy)
            {
                Debugger.Break();
            }
            if (surfaceEnergy != ninestar.SurfaceEnergy.Energy)
            {
                Debugger.Break();
            }

            Assert.Equal(yearEnergy, ninestar.MainEnergy.Energy);
            Assert.Equal(monthEnergy, ninestar.CharacterEnergy.Energy);
            Assert.Equal(surfaceEnergy, ninestar.SurfaceEnergy.Energy);
        }

        [Theory]
        [InlineData(1979, 6, 16, 2011, 2, 14, EGender.Male, ENineStarKiEnergy.Water, ENineStarKiEnergy.Fire)] // Thunder man lake year
        [InlineData(1979, 6, 16, 2011, 12, 14, EGender.Male, ENineStarKiEnergy.Water, ENineStarKiEnergy.Water, true)] // Thunder man / lake year
        [InlineData(1979, 6, 16, 2011, 12, 14, EGender.Female, ENineStarKiEnergy.Fire, ENineStarKiEnergy.Fire)]
        public void LifeCycle_Test(int birthYear, int birthMonth, int birthDay, int year, int month, int day, EGender gender, ENineStarKiEnergy yearlyCycleEnergy, ENineStarKiEnergy monthlyCycleEnergy, bool isDebug = false)
        {
            if (isDebug)
                Debugger.Break();

            var ninestar = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = new DateTime(birthYear, birthMonth, birthDay),
                Gender = gender
            }, false, false, new DateTime(year, month, day), ECalculationMethod.Chinese,
                true);

            Assert.Equal(ENineStarKiEnergy.Lake, ninestar.GlobalCycleEnergies.Year.Energy);
            Assert.Equal(yearlyCycleEnergy, ninestar.PersonalHousesOccupiedEnergies.Year.Energy);
            Assert.Equal(monthlyCycleEnergy, ninestar.PersonalHousesOccupiedEnergies.Month.Energy);
        }

        [Theory]
        [InlineData(1977, ENineStarKiEnergy.CoreEarth, EGender.Male, // Personal DOB
            1977, 2, // Today's Date
            ENineStarKiEnergy.CoreEarth, // Global Yearly Cycle
            ENineStarKiEnergy.Soil, // Global Monthly Cycle
            ENineStarKiEnergy.CoreEarth, // Personal Yearly Cycle
            ENineStarKiEnergy.Mountain, // Personal Monthly Cycle
            ENineStarKiEnergy.CoreEarth, // Core Earth Yearly Cycle
            ENineStarKiEnergy.Mountain)] // Core Earth Monthly Cycle

        //[InlineData(1978, ENineStarKiEnergy.Wind, EGender.Male, // Personal DOB
        //    1978, 2, // Today's Date
        //    ENineStarKiEnergy.Wind, // Global Yearly Cycle
        //    ENineStarKiEnergy.Mountain, // Global Monthly Cycle
        //    ENineStarKiEnergy.CoreEarth, // Personal Yearly Cycle
        //    ENineStarKiEnergy.Water, // Personal Monthly Cycle
        //    ENineStarKiEnergy.Heaven, // Core Earth Yearly Cycle
        //    ENineStarKiEnergy.Soil)] // Core Earth Monthly Cycle

        //[InlineData(1978, ENineStarKiEnergy.Wind, EGender.Male, // Personal DOB
        //    1978, 3, // Today's Date
        //    ENineStarKiEnergy.Wind, // Global Yearly Cycle
        //    ENineStarKiEnergy.Lake, // Global Monthly Cycle
        //    ENineStarKiEnergy.CoreEarth, // Personal Yearly Cycle
        //    ENineStarKiEnergy.Soil, // Personal Monthly Cycle
        //    ENineStarKiEnergy.Heaven, // Core Earth Yearly Cycle
        //    ENineStarKiEnergy.Thunder)] // Core Earth Monthly Cycle
        public void CalculateDirectionForMonth_Test(
            int birthYear,
            ENineStarKiEnergy mainEnergy,
            EGender gender,
            int todayYear,
            int todayMonth,
            ENineStarKiEnergy globalYearlyCycleEnergy,
            ENineStarKiEnergy globalMonthlyCycleEnergy,
            ENineStarKiEnergy personalYearlyCycleEnergy,
            ENineStarKiEnergy personalMonthlyCycleEnergy,
            ENineStarKiEnergy coreEarthYearlyCycleEnergy,
            ENineStarKiEnergy coreEarthMonthlyCycleEnergy)
        {
            var ninestar = _nineStarKiService.CalculateNineStarKiProfile(
                new PersonModel
                {
                    DateOfBirth = new DateTime(birthYear, 2, 5),
                    Gender = gender,
                },
                false,
                false,
                new DateTime(todayYear, todayMonth, 15),
                ECalculationMethod.Chinese,
                true);

            Assert.Equal(mainEnergy, ninestar.MainEnergy.Energy);
            Assert.Equal(globalYearlyCycleEnergy, ninestar.GlobalCycleEnergies.Year.Energy);
            Assert.Equal(globalMonthlyCycleEnergy, ninestar.GlobalCycleEnergies.Month.Energy);
            Assert.Equal(personalYearlyCycleEnergy, ninestar.PersonalHousesOccupiedEnergies.Year.Energy);
            Assert.Equal(personalMonthlyCycleEnergy, ninestar.PersonalHousesOccupiedEnergies.Month.Energy);
            Assert.Equal(coreEarthYearlyCycleEnergy, ninestar.GlobalCycleEnergies.Year.GetHouseOfFive().Energy);
            Assert.Equal(coreEarthMonthlyCycleEnergy, ninestar.GlobalCycleEnergies.Month.GetHouseOfFive().Energy);
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
        public void CalcualteYearlyCycleWithCycleChange_Test(
            int birthYear,
            ENineStarKiEnergy energy,
            EGender gender,
            int todayYear,
            ENineStarKiEnergy yearlyCycleEnergy,
            bool isDebug = false)
        {
            if (isDebug)
            {
                Debugger.Break();
            }

            var ninestar = _nineStarKiService.CalculateNineStarKiProfile(
                new PersonModel
                {
                    DateOfBirth = new DateTime(birthYear, 2, 5),
                    Gender = gender,
                },
                false,
                false,
                new DateTime(todayYear, 2, 5),
                ECalculationMethod.Chinese,
                true);

            Assert.Equal(energy, ninestar.MainEnergy.Energy);
            Assert.Equal(yearlyCycleEnergy, ninestar.PersonalHousesOccupiedEnergies.Year.Energy);
        }

        [Theory]
        [InlineData(1979, ENineStarKiEnergy.Thunder, EGender.Male, 2025, ENineStarKiEnergy.Heaven, 2, ENineStarKiEnergy.Heaven, true)]
        [InlineData(1979, ENineStarKiEnergy.Thunder, EGender.Male, 2025, ENineStarKiEnergy.Heaven, 3, ENineStarKiEnergy.Lake)]
        [InlineData(1980, ENineStarKiEnergy.Soil, EGender.Male, 2025, ENineStarKiEnergy.CoreEarth, 2, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1980, ENineStarKiEnergy.Soil, EGender.Male, 2025, ENineStarKiEnergy.CoreEarth, 3, ENineStarKiEnergy.Heaven)]
        public void CalcualteMonthlyCycleWithCycleChange_Test(
            int birthYear,
            ENineStarKiEnergy energy,
            EGender gender,
            int todayYear,
            ENineStarKiEnergy yearlyCycleEnergy,
            int monthNumber,
            ENineStarKiEnergy monthlyCycleEnergy,
            bool isDebug = false)
        {

            if (isDebug)
                Debugger.Break();

            var ninestar = _nineStarKiService.CalculateNineStarKiProfile(
                new PersonModel
                {
                    DateOfBirth = new DateTime(birthYear, 2, 7),
                    Gender = gender,
                },
                false,
                false,
                new DateTime(todayYear, monthNumber, 15),
                ECalculationMethod.Chinese,
                true);

            Assert.Equal(energy, ninestar.MainEnergy.Energy);
            Assert.Equal(yearlyCycleEnergy, ninestar.PersonalHousesOccupiedEnergies.Year.Energy);
            Assert.Equal(monthlyCycleEnergy, ninestar.PersonalHousesOccupiedEnergies.Month.Energy);
        }

        [Theory]
        [InlineData(1979, 6, 16, EGender.Male, ENineStarKiEnergy.Thunder)]
        [InlineData(1980, 6, 16, EGender.Male, ENineStarKiEnergy.Soil)]
        [InlineData(1981, 6, 16, EGender.Male, ENineStarKiEnergy.Water)]
        [InlineData(1982, 6, 16, EGender.Male, ENineStarKiEnergy.Fire)]
        [InlineData(1979, 2, 5, EGender.Male, ENineStarKiEnergy.Thunder)]
        [InlineData(1979, 2, 3, EGender.Male, ENineStarKiEnergy.Wind)]
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
            var nineStarKiYear = _astronomyService.GetNineStarKiYearlyKi(new DateTime(birthYear, birthMonth, birthDay, hour, 0, 0), "Europe/London");

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
            var nineStarKiMonth = _astronomyService.GetNineStarKiMonthlyKi(new DateTime(birthYear, birthMonth, birthDay, hour, 0, 0), timeZone);

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
                BirthTimeZoneId = timeZone
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
                BirthTimeZoneId = timeZone
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

            var dayEnergy = _astronomyService.GetNineStarKiDailyKi(today, timeZone);
            Assert.Equal((int)dailyKi, dayEnergy.DailyKi);

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
        [InlineData(1900, 6, 20, ENineStarKiEnergy.Wind)]
        public void CalcualteSwissEphemeris_DailyKi(
            int todayYear,
            int todayMonth,
            int todayDay,
            ENineStarKiEnergy dailyKi)
        {
            var today = new DateTime(todayYear, todayMonth, todayDay, 12, 0, 0);

            var dayEnergy = _astronomyService.GetNineStarKiDailyKi(today, "Europe/London");
            Assert.Equal((int)dailyKi, dayEnergy.DailyKi);
        }

        [Theory]
        [InlineData(1900, 1, 5, ENineStarKiEnergy.Heaven)]
        [InlineData(1900, 2, 5, ENineStarKiEnergy.Water)]
        [InlineData(1900, 6, 19, ENineStarKiEnergy.Fire)]
        // Change
        [InlineData(1900, 6, 20, ENineStarKiEnergy.Wind)]
        [InlineData(1900, 6, 21, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1900, 6, 22, ENineStarKiEnergy.Wind)]
        [InlineData(1900, 7, 7, ENineStarKiEnergy.Lake)]
        [InlineData(1900, 10, 31, ENineStarKiEnergy.Mountain)]
        [InlineData(1900, 12, 7, ENineStarKiEnergy.Lake)]
        [InlineData(1900, 12, 21, ENineStarKiEnergy.Soil)]
        [InlineData(1900, 12, 22, ENineStarKiEnergy.Fire)]
        [InlineData(1900, 12, 23, ENineStarKiEnergy.Water)]
        [InlineData(1900, 12, 31, ENineStarKiEnergy.Fire)]
        [InlineData(1901, 1, 1, ENineStarKiEnergy.Water)]
        [InlineData(1901, 1, 5, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1901, 2, 5, ENineStarKiEnergy.Fire)]
        [InlineData(1901, 2, 15, ENineStarKiEnergy.Water)]
        [InlineData(1901, 3, 15, ENineStarKiEnergy.Soil)]
        [InlineData(1901, 3, 31, ENineStarKiEnergy.Fire)]
        [InlineData(1901, 4, 14, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1901, 4, 15, ENineStarKiEnergy.Heaven)]
        [InlineData(1901, 4, 16, ENineStarKiEnergy.Lake)]
        [InlineData(1901, 4, 20, ENineStarKiEnergy.Soil)]
        [InlineData(1901, 4, 21, ENineStarKiEnergy.Thunder)]
        [InlineData(1901, 4, 22, ENineStarKiEnergy.Wind)]
        [InlineData(1901, 5, 6, ENineStarKiEnergy.Fire)]
        [InlineData(1901, 6, 15, ENineStarKiEnergy.Wind)]
        [InlineData(1901, 6, 21, ENineStarKiEnergy.Water)]
        [InlineData(1901, 6, 22, ENineStarKiEnergy.Mountain)]
        [InlineData(1901, 8, 8, ENineStarKiEnergy.Heaven)]
        [InlineData(1901, 11, 8, ENineStarKiEnergy.Wind)]
        [InlineData(1901, 12, 21, ENineStarKiEnergy.Heaven)]
        [InlineData(1901, 12, 22, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1901, 12, 23, ENineStarKiEnergy.Heaven)]
        [InlineData(1902, 1, 8, ENineStarKiEnergy.Wind)]
        [InlineData(1902, 1, 24, ENineStarKiEnergy.Soil)]
        [InlineData(1902, 4, 5, ENineStarKiEnergy.Water)]
        [InlineData(1902, 6, 6, ENineStarKiEnergy.Fire)]
        [InlineData(1902, 6, 12, ENineStarKiEnergy.Heaven)]
        [InlineData(1902, 6, 21, ENineStarKiEnergy.Heaven)]
        [InlineData(1902, 7, 15, ENineStarKiEnergy.Lake)]
        [InlineData(1902, 9, 18, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1902, 10, 18, ENineStarKiEnergy.Soil)]
        [InlineData(1902, 12, 18, ENineStarKiEnergy.Wind)]
        [InlineData(1902, 12, 28, ENineStarKiEnergy.Lake)]
        [InlineData(1903, 2, 10, ENineStarKiEnergy.Heaven)]
        [InlineData(1903, 5, 18, ENineStarKiEnergy.Wind)]
        [InlineData(1903, 11, 14, ENineStarKiEnergy.Heaven)]
        [InlineData(1904, 2, 10, ENineStarKiEnergy.Soil)]
        [InlineData(1904, 3, 30, ENineStarKiEnergy.Heaven)]
        [InlineData(1904, 4, 23, ENineStarKiEnergy.Thunder)]
        [InlineData(1904, 6, 16, ENineStarKiEnergy.Thunder)]
        [InlineData(1904, 9, 8, ENineStarKiEnergy.Wind)]
        [InlineData(1904, 11, 11, ENineStarKiEnergy.Thunder)]
        [InlineData(1904, 12, 31, ENineStarKiEnergy.Thunder)]
        [InlineData(1905, 3, 11, ENineStarKiEnergy.Water)]
        [InlineData(1905, 3, 25, ENineStarKiEnergy.Heaven)]
        // Change
        [InlineData(1905, 3, 26, ENineStarKiEnergy.Lake)]
        [InlineData(1905, 3, 31, ENineStarKiEnergy.Thunder)]
        [InlineData(1905, 4, 5, ENineStarKiEnergy.Mountain)]
        [InlineData(1905, 4, 20, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1905, 4, 21, ENineStarKiEnergy.Heaven)]
        [InlineData(1905, 4, 25, ENineStarKiEnergy.Water)]
        [InlineData(1905, 11, 17, ENineStarKiEnergy.Water)]

        // Change
        [InlineData(1907, 3, 16, ENineStarKiEnergy.Lake)]
        [InlineData(1907, 3, 17, ENineStarKiEnergy.Mountain)]
        [InlineData(1907, 5, 14, ENineStarKiEnergy.Thunder)]
        [InlineData(1907, 5, 15, ENineStarKiEnergy.Wind)]
        [InlineData(1907, 8, 15, ENineStarKiEnergy.Wind)]
        [InlineData(1907, 10, 13, ENineStarKiEnergy.Mountain)]
        [InlineData(1908, 1, 13, ENineStarKiEnergy.Wind)]
        [InlineData(1908, 3, 13, ENineStarKiEnergy.Water)]
        [InlineData(1908, 4, 21, ENineStarKiEnergy.Wind)]

        [InlineData(1908, 5, 09, ENineStarKiEnergy.Wind)]
        [InlineData(1908, 5, 10, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1908, 12, 30, ENineStarKiEnergy.CoreEarth)]

        [InlineData(1909, 4, 20, ENineStarKiEnergy.Mountain)]
        [InlineData(1909, 4, 21, ENineStarKiEnergy.Fire)]
        [InlineData(1909, 5, 25, ENineStarKiEnergy.Lake)]
        [InlineData(1909, 6, 25, ENineStarKiEnergy.Mountain)]
        [InlineData(1909, 7, 25, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1909, 8, 22, ENineStarKiEnergy.Wind)]
        [InlineData(1909, 9, 22, ENineStarKiEnergy.Fire)]
        [InlineData(1910, 3, 22, ENineStarKiEnergy.Soil)]
        [InlineData(1910, 4, 28, ENineStarKiEnergy.Thunder)]
        [InlineData(1910, 4, 29, ENineStarKiEnergy.Wind)]
        [InlineData(1910, 6, 28, ENineStarKiEnergy.Fire)]
        [InlineData(1910, 8, 27, ENineStarKiEnergy.Thunder)]
        [InlineData(1910, 7, 22, ENineStarKiEnergy.Thunder)]
        [InlineData(1910, 11, 22, ENineStarKiEnergy.Heaven)]
        [InlineData(1911, 3, 22, ENineStarKiEnergy.Lake)]
        [InlineData(1911, 4, 5, ENineStarKiEnergy.Thunder)]
        [InlineData(1911, 4, 22, ENineStarKiEnergy.Soil)]
        [InlineData(1911, 4, 23, ENineStarKiEnergy.Thunder)]
        [InlineData(1911, 8, 21, ENineStarKiEnergy.Wind)]
        [InlineData(1911, 8, 22, ENineStarKiEnergy.Thunder)]
        [InlineData(1911, 9, 22, ENineStarKiEnergy.Mountain)]
        [InlineData(1911, 10, 22, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1911, 11, 22, ENineStarKiEnergy.Water)]
        [InlineData(1912, 4, 7, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1913, 4, 7, ENineStarKiEnergy.Water)]
        [InlineData(1914, 4, 7, ENineStarKiEnergy.Heaven)]
        [InlineData(1914, 8, 8, ENineStarKiEnergy.Lake)]
        [InlineData(1914, 11, 8, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1915, 1, 8, ENineStarKiEnergy.Thunder)]
        [InlineData(1915, 3, 8, ENineStarKiEnergy.Mountain)]
        [InlineData(1915, 6, 8, ENineStarKiEnergy.Water)]
        [InlineData(1915, 8, 11, ENineStarKiEnergy.Mountain)]
        [InlineData(1915, 11, 8, ENineStarKiEnergy.Fire)]
        [InlineData(1915, 12, 8, ENineStarKiEnergy.Heaven)]
        [InlineData(1916, 1, 27, ENineStarKiEnergy.Fire)]
        [InlineData(1916, 3, 27, ENineStarKiEnergy.Heaven)]
        [InlineData(1916, 5, 26, ENineStarKiEnergy.Thunder)]
        [InlineData(1916, 11, 22, ENineStarKiEnergy.Lake)]
        [InlineData(1916, 2, 8, ENineStarKiEnergy.Thunder)]
        [InlineData(1920, 9, 11, ENineStarKiEnergy.Wind)]
        [InlineData(1922, 7, 13, ENineStarKiEnergy.Fire)]
        [InlineData(1922, 6, 25, ENineStarKiEnergy.Fire)]
        [InlineData(1922, 10, 22, ENineStarKiEnergy.Lake)]
        [InlineData(1922, 10, 23, ENineStarKiEnergy.Heaven)]
        [InlineData(1922, 12, 10, ENineStarKiEnergy.Thunder)]
        [InlineData(1928, 7, 21, ENineStarKiEnergy.Soil)]
        [InlineData(2025, 3, 5, ENineStarKiEnergy.Lake)]
        [InlineData(2025, 7, 30, ENineStarKiEnergy.Fire)]
        public void CalcualteSwissEphemeris_DailyKi_Ascending_Descending(
            int todayYear,
            int todayMonth,
            int todayDay,
            ENineStarKiEnergy dailyKi,
            bool isDebug = false)
        {
            if (isDebug)
            {
                Debugger.Break();
            }

            var today = new DateTime(todayYear, todayMonth, todayDay, 12, 0, 0);
            var dayEnergy = _astronomyService.GetNineStarKiDailyKi(today, "Europe/London");

            Assert.Equal((int)dailyKi, dayEnergy.DailyKi);
        }

        [Theory]
        [InlineData(1924, 6, 20, 0, ENineStarKiEnergy.Water)]
        [InlineData(1924, 6, 20, 1, ENineStarKiEnergy.Soil)]
        [InlineData(1924, 6, 20, 3, ENineStarKiEnergy.Thunder)]
        [InlineData(1924, 6, 21, 0, ENineStarKiEnergy.Wind)]
        [InlineData(1924, 11, 21, 13, ENineStarKiEnergy.Mountain)]
        [InlineData(2024, 08, 23, 1, ENineStarKiEnergy.Heaven, "Europe/London")]
        public void CalcualteSwissEphemeris_HourlyKi(
            int todayYear,
            int todayMonth,
            int todayDay,
            int todayHour,
            ENineStarKiEnergy hourlyKi,
            string timeZone = "",
            bool isDebug = false)
        {
            if (isDebug)
            {
                Debugger.Break();
            }

            var today = new DateTime(todayYear, todayMonth, todayDay, todayHour, 0, 0);
            var actualHourlyKi = _astronomyService.GetNineStarKiHourlyKi(today, timeZone);

            Assert.Equal((int)hourlyKi, actualHourlyKi);
        }

        [Theory]
        [InlineData(1991, 2, 5, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1991, 2, 3, ENineStarKiEnergy.Heaven)]
        [InlineData(2000, 2, 7, ENineStarKiEnergy.Wind)]
        [InlineData(2009, 2, 7, ENineStarKiEnergy.Thunder)]
        [InlineData(2018, 2, 7, ENineStarKiEnergy.Soil)]
        [InlineData(2027, 2, 7, ENineStarKiEnergy.Water)]
        public void CalcualteSwissEphemeris_9YearKi(
            int todayYear,
            int todayMonth,
            int todayDay,
            ENineStarKiEnergy energy,
            bool isDebug = false)
        {
            if (isDebug)
            {
                Debugger.Break();
            }

            var today = new DateTime(todayYear, todayMonth, todayDay, 0, 0, 0);
            var actual = _astronomyService.GetNineStarKiNineYearKi(today, "");

            Assert.Equal((int)energy, actual);
        }

        [Theory]
        [InlineData(1955, 2, 5, ENineStarKiEnergy.Fire)]
        [InlineData(2035, 2, 5, ENineStarKiEnergy.Fire)]
        [InlineData(2036, 2, 5, ENineStarKiEnergy.Mountain)]
        public void CalcualteSwissEphemeris_81YearKi(
            int todayYear,
            int todayMonth,
            int todayDay,
            ENineStarKiEnergy energy,
            bool isDebug = false)
        {
            if (isDebug)
            {
                Debugger.Break();
            }

            var today = new DateTime(todayYear, todayMonth, todayDay, 0, 0, 0);
            var actual = _astronomyService.GetNineStarKiEightyOneYearKi(today, "");

            Assert.Equal((int)energy, actual);
        }

        [Theory]
        [InlineData(1981, 2, 8, EGender.Male, ENineStarKiEnergy.Water)]
        [InlineData(1980, 2, 8, EGender.Male, ENineStarKiEnergy.Soil)]
        [InlineData(1979, 2, 8, EGender.Male, ENineStarKiEnergy.Thunder)]
        [InlineData(1978, 2, 8, EGender.Male, ENineStarKiEnergy.Wind)]
        [InlineData(1977, 2, 8, EGender.Male, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1976, 2, 8, EGender.Male, ENineStarKiEnergy.Heaven)]
        [InlineData(1975, 2, 8, EGender.Male, ENineStarKiEnergy.Lake)]
        [InlineData(1974, 2, 8, EGender.Male, ENineStarKiEnergy.Mountain)]
        [InlineData(1973, 2, 8, EGender.Male, ENineStarKiEnergy.Fire)]

        [InlineData(1981, 2, 8, EGender.Female, ENineStarKiEnergy.CoreEarth)]
        [InlineData(1980, 2, 8, EGender.Female, ENineStarKiEnergy.Wind)]
        [InlineData(1979, 2, 8, EGender.Female, ENineStarKiEnergy.Thunder)]
        [InlineData(1978, 2, 8, EGender.Female, ENineStarKiEnergy.Soil)]
        [InlineData(1977, 2, 8, EGender.Female, ENineStarKiEnergy.Water)]
        [InlineData(1976, 2, 8, EGender.Female, ENineStarKiEnergy.Fire)]
        [InlineData(1975, 2, 8, EGender.Female, ENineStarKiEnergy.Mountain)]
        [InlineData(1974, 2, 8, EGender.Female, ENineStarKiEnergy.Lake)]
        [InlineData(1973, 2, 8, EGender.Female, ENineStarKiEnergy.Heaven)]
        public void GlobalCycleEnergyFiveYear_PersonalHousesCoincide_Test(int birthYear, int birthMonth, int birthDay, EGender gender, ENineStarKiEnergy mainEnergy)
        {
            var lakeYear = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = new DateTime(birthYear, birthMonth, birthDay),
                Gender = gender
            }, false, false, new DateTime(2105, 2, 7),
                ECalculationMethod.Chinese,
                true);

            var ninestarCoreEarthYear = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = new DateTime(birthYear, birthMonth, birthDay),
                Gender = gender
            }, false, false, new DateTime(2103, 2, 7),
                ECalculationMethod.Chinese,
                true); // 5 Soil Earth Year

            var ninestarWaterYear = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = new DateTime(birthYear, birthMonth, birthDay),
                Gender = gender
            }, false, false, new DateTime(2098, 2, 7),
                ECalculationMethod.Chinese,
                true);

            var ninestarInvertedCoreEarthYear = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = new DateTime(birthYear, birthMonth, birthDay),
                Gender = gender
            }, false, false, new DateTime(2107, 2, 7),
                ECalculationMethod.Chinese,
                true); // 5 Soil Earth Year

            var ninestarInvertedFireYear = _nineStarKiService.CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = new DateTime(birthYear, birthMonth, birthDay),
                Gender = gender
            }, false, false, new DateTime(2112, 2, 7),
                ECalculationMethod.Chinese,
                true); // 5 Soil Earth Year

            // Double check main energy
            Assert.Equal(mainEnergy, ninestarWaterYear.PersonalChartEnergies.Year.Energy);

            // Double check Global Ki
            Assert.Equal(ENineStarKiEnergy.Lake, lakeYear.GlobalCycleEnergies.Year.Energy);
            Assert.Equal(ENineStarKiEnergy.CoreEarth, ninestarCoreEarthYear.GlobalCycleEnergies.Year.Energy);
            Assert.Equal(ENineStarKiEnergy.CoreEarth, ninestarInvertedCoreEarthYear.GlobalCycleEnergies.Year.Energy);
            Assert.Equal(ENineStarKiEnergy.Water, ninestarWaterYear.GlobalCycleEnergies.Year.Energy);
            Assert.Equal(ENineStarKiEnergy.Fire, ninestarInvertedFireYear.GlobalCycleEnergies.Year.Energy);

            /*******************************
             * Before Cycle Switch
            ********************************/

            // EARTH YEAR
            if (ninestarCoreEarthYear.PersonModel.Gender == EGender.Male)
            {
                // Male energies are in their personal house
                Assert.Equal(ninestarCoreEarthYear.PersonalChartEnergies.Year.Energy, ninestarCoreEarthYear.PersonalHousesOccupiedEnergies.Year.Energy);
            }
            else
            {
                // Female energies are not in their personal house (unless core earth)
                if (ninestarCoreEarthYear.PersonalChartEnergies.Year.Energy != ENineStarKiEnergy.CoreEarth)
                    Assert.NotEqual(ninestarCoreEarthYear.PersonalChartEnergies.Year.Energy, ninestarCoreEarthYear.PersonalHousesOccupiedEnergies.Year.Energy);
            }
            // WATER YEAR
            if (ninestarWaterYear.PersonModel.Gender == EGender.Male)
            {
                // Male energies are not in their personal house
                Assert.NotEqual(ninestarWaterYear.PersonalChartEnergies.Year.Energy, ninestarWaterYear.PersonalHousesOccupiedEnergies.Year.Energy);
            }
            else
            {
                // Female energies are in their inverted personal house
                Assert.Equal(ninestarWaterYear.PersonalChartEnergies.Year.EnergyNumber, ninestarWaterYear.PersonalHousesOccupiedEnergies.Year.EnergyNumber);
            }

            /*******************************
             * After Cycle Switch
            ********************************/

            // EARTH YEAR
            // All energies are in their personal house
            Assert.Equal(ninestarInvertedCoreEarthYear.PersonalChartEnergies.Year.Energy, ninestarInvertedCoreEarthYear.PersonalHousesOccupiedEnergies.Year.Energy);

            // WATER YEAR
            // All energies are not in their personal house
            Assert.NotEqual(ninestarInvertedFireYear.PersonalChartEnergies.Year.Energy, ninestarInvertedFireYear.PersonalHousesOccupiedEnergies.Year.Energy);
        }

        public void Dispose()
        {
            Trace.Listeners.Remove(_listener);
            _listener.Dispose();
        }

    }
}
