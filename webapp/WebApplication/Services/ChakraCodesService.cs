using K9.WebApplication.Constants;
using K9.WebApplication.Enums;
using K9.WebApplication.Extensions;
using K9.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace K9.WebApplication.Services
{
    public class ChakraCodesService : IChakraCodesService
    {
        public ChakraCodesModel CalculateChakraCodes(ChakraCodesModel model)
        {
            if (model.PersonModel == null)
            {
                return model;
            }

            model.Dominant = CalculateDominant(model.PersonModel.DateOfBirth);
            model.SubDominant = CalculateSubDominant(model.PersonModel.DateOfBirth);
            model.Guide = CalculateGuide(model.PersonModel.DateOfBirth);
            model.Gift = CalculateGift(model.PersonModel.DateOfBirth);
            model.BirthYear = CalculateBirthYear(model.PersonModel);
            model.CurrentYear = CalculateCurrentYear(model.PersonModel);
            model.CurrentMonth = CalculateCurrentMonth(model.PersonModel);
            model.MonthlyPlannerCodes = CalculateMonthChakraCodes(model.PersonModel);
            model.YearlyPlannerCodes = CalculateYearlyPlannerChakraCodes(model.PersonModel);
            model.DailyPlannerCodes = CalculateDailyPlannerChakraCodes(model.PersonModel);
            model.DharmaCodes = CalculateDharmaCodes(model.PersonModel);
            model.YearlyForecast = GetYearlyForecast(model.PersonModel);
            model.MonthlyForecast = GetMonthlyForecast(model.PersonModel);
            model.DailyForecast = GetDailyForecast(model.PersonModel);

            model.IsProcessed = true;

            return model;
        }

        public ChakraCodeDetails CalculateDominant(DateTime date)
        {
            var result = CalculateNumerology(date.Year + date.Month + date.Day);

            return new ChakraCodeDetails
            {
                ChakraCode = (EChakraCode)result,
                Title = "Dominant"
            };
        }

        public ChakraCodeDetails CalculateSubDominant(DateTime date)
        {
            var result = date.Month > 9 ? date.Month - 9 : date.Month;

            return new ChakraCodeDetails
            {
                ChakraCode = (EChakraCode)result,
                IsActive = IsActive(date, 0, 27),
                Title = "Sub Dominant"
            };
        }

        public ChakraCodeDetails CalculateGuide(DateTime date)
        {
            var result = CalculateNumerology(date.Day);

            return new ChakraCodeDetails
            {
                ChakraCode = (EChakraCode)result,
                IsActive = IsActive(date, 27, 54),
                Title = "Guide"
            };
        }

        public ChakraCodeDetails CalculateGift(DateTime date)
        {
            var result = CalculateNumerology(date.Year);

            return new ChakraCodeDetails
            {
                ChakraCode = (EChakraCode)result,
                IsActive = IsActive(date, 54, 81),
                Title = "Gift"
            };
        }

        public ChakraCodeDetails CalculateBirthYear(PersonModel person)
        {
            var result = CalculateDominant(person.DateOfBirth).ChakraCode;

            result = result == EChakraCode.Elder ? EChakraCode.Pioneer : result + 1;

            return new ChakraCodeDetails
            {
                ChakraCode = result,
                Title = "Birth Year"
            };
        }

        public ChakraCodeDetails CalculateCurrentYear(PersonModel person, int? offset = null)
        {
            var result = CalculateBirthYear(person).ChakraCode;
            var year = person.DateOfBirth.Year + person.YearsOld;

            result = (EChakraCode)(((int)result + person.YearsOld) % 9);

            if (result == 0)
            {
                result++;
            }

            var yearEndDate = GetYearEndDate(result, year);
            if (yearEndDate < DateTime.Today)
            {
                result = result.Increment();
            }

            if (offset < 0)
            {
                result = result.Decrement(Math.Abs(offset.Value));
            }

            if (offset > 0)
            {
                result = result.Increment(offset.Value);
            }

            return new ChakraCodeDetails
            {
                ChakraCode = result,
                Title = "Current Year",
                StartDate = GetYearStartDate(result),
                EndDate = GetYearEndDate(result),
                ShowDates = true
            };
        }

        public ChakraCodeDetails CalculateCurrentMonth(PersonModel person, int? offset = null)
        {
            var monthChakraCodes = CalculateMonthChakraCodes(person);
            var chakraCode = monthChakraCodes.FirstOrDefault(e => e.IsCurrent).ChakraCode;

            if (offset < 0)
            {
                chakraCode = chakraCode.Decrement(Math.Abs(offset.Value));
            }

            if (offset > 0)
            {
                chakraCode = chakraCode.Increment(offset.Value);
            }

            var activeMonth = monthChakraCodes.FirstOrDefault(e => e.ChakraCode == chakraCode);

            return activeMonth == null ? null : new ChakraCodeDetails
            {
                ChakraCode = activeMonth.ChakraCode,
                Title = "Current Month",
                StartDate = activeMonth.StartDate,
                EndDate = activeMonth.EndDate
            };
        }

        public List<ChakraCodePlannerModel> CalculateDailyPlannerChakraCodes(PersonModel person)
        {
            var items = new List<ChakraCodePlannerModel>();
            var currentMonth = DateTime.Today.Month;
            var i = 1;
            var day = new DateTime(DateTime.Today.Year, currentMonth, i);

            while (currentMonth == day.Month)
            {
                var offset = day.Day - DateTime.Today.Day;

                items.Add(new ChakraCodePlannerModel
                {
                    ChakraCode = (EChakraCode)CalculateNumerology(day.Day),
                    StartDate = day,
                    EndDate = day,
                    Offset = offset
                });

                day = day.AddDays(i);
            }

            return items;
        }

        public List<ChakraCodePlannerModel> CalculateMonthChakraCodes(PersonModel person)
        {
            var items = new List<ChakraCodePlannerModel>();
            var yearEnergy = CalculateCurrentYear(person);
            var monthEnergy = yearEnergy.ChakraCodeNumber - 6;

            monthEnergy = monthEnergy < 1 ? (9 + monthEnergy) : monthEnergy;

            for (int i = 0; i < 12; i++)
            {
                var monthNumber = i + 1;
                var offset = monthNumber - DateTime.Today.Day;

                items.Add(new ChakraCodePlannerModel
                {
                    ChakraCode = (EChakraCode)monthEnergy,
                    StartDate = GetMonthStartDate((EChakraCode)monthEnergy, monthNumber),
                    EndDate = GetMonthEndDate((EChakraCode)monthEnergy, monthNumber),
                    Offset = offset
                });

                monthEnergy = monthEnergy.Increment();
            }

            return items;
        }

        public List<ChakraCodePlannerModel> CalculateYearlyPlannerChakraCodes(PersonModel person)
        {
            var items = new List<ChakraCodePlannerModel>();
            var currentYear = CalculateCurrentYear(person);
            var year = person.DateOfBirth.Year + person.YearsOld;
            var offset = 5;
            var yearEnergy = currentYear.ChakraCodeNumber.Decrement(offset + 1);

            year = year - offset;

            for (int i = 0; i < 12; i++)
            {
                items.Add(new ChakraCodePlannerModel
                {
                    ChakraCode = (EChakraCode)yearEnergy,
                    StartDate = GetYearStartDate((EChakraCode)yearEnergy, year),
                    EndDate = GetYearEndDate((EChakraCode)yearEnergy, year),
                    Offset = i - offset
                });

                year++;
                yearEnergy = yearEnergy.Increment();
            }

            return items;
        }

        public List<DharmaChakraCodeModel> CalculateDharmaCodes(PersonModel person)
        {
            var items = new List<DharmaChakraCodeModel>();
            var age = 0;
            var year = person.DateOfBirth.Year;
            var yearEnergy = CalculateBirthYear(person).ChakraCodeNumber;

            for (int i = 0; i < 100; i++)
            {
                items.Add(new DharmaChakraCodeModel
                {
                    Age = age,
                    Year = year,
                    ChakraCode = (EChakraCode)yearEnergy
                });

                age++;
                year++;
                yearEnergy = yearEnergy.Increment();
            }

            // Get first 9
            var skip = 0;
            var dharmaBaseCode = 9;
            var code = items.First(e => e.ChakraCodeNumber == 9);

            age = code.Age;

            while (age >= 0)
            {
                code = items.First(e => e.Age == age);
                code.DharmaChakraBaseCode = (EChakraCode)dharmaBaseCode;

                if (skip >= 1)
                {
                    dharmaBaseCode = dharmaBaseCode.Decrement();
                    skip = 0;
                }
                else
                {
                    skip++;
                }

                age--;
            }

            // Get those after 9 
            code = items.First(e => e.ChakraCodeNumber == 9);
            age = code.Age + 1;
            dharmaBaseCode = 1;
            skip = 0;

            while (age < 100)
            {
                code = items.First(e => e.Age == age);
                code.DharmaChakraBaseCode = (EChakraCode)dharmaBaseCode;

                if (skip >= 1)
                {
                    dharmaBaseCode = dharmaBaseCode.Increment();
                    skip = 0;
                }
                else
                {
                    skip++;
                }

                age++;
            }

            // Calculate Dharma Codes
            var birthCode = items.First(e => e.Age == 0);
            var groupCode = birthCode.DharmaChakraBaseCode.Decrement();
            var dharmaCode = (int)groupCode;

            code = items.First(e => e.ChakraCodeNumber == 9);
            age = code.Age;
            skip = 0;

            while (age >= 0)
            {
                code = items.First(e => e.Age == age);
                code.DharmaChakraCode = (EChakraCode)dharmaCode;
                code.DharmaGroupChakraCode = groupCode;

                if (skip >= 1)
                {
                    dharmaCode = dharmaCode.Decrement();
                    skip = 0;
                }
                else
                {
                    skip++;
                }

                age--;
            }

            // Get remaining dharma codes
            code = items.First(e => e.ChakraCodeNumber == 9);
            var y = 1;
            while (code != null)
            {
                code = items.First(e => e.ChakraCodeNumber == 9);
                groupCode = groupCode.Increment();
                dharmaCode = (int)groupCode;
                age = code.Age + (18 * y);
                skip = 0;

                for (int i = 0; i < 18; i++)
                {
                    code = items.FirstOrDefault(e => e.Age == age - i);
                    if (code != null)
                    {
                        code.DharmaChakraCode = (EChakraCode)dharmaCode;
                        code.DharmaGroupChakraCode = groupCode;
                    }

                    if (skip >= 1)
                    {
                        dharmaCode = dharmaCode.Decrement();
                        skip = 0;
                    }
                    else
                    {
                        skip++;
                    }
                }

                y++;
            }

            return items;
        }

        public ChakraCodeForecast GetYearlyForecast(PersonModel person, int? offset = 0)
        {
            var dominant = CalculateDominant(person.DateOfBirth);
            var yearEnergy = CalculateCurrentYear(person, offset);
            var x = CalculateNumerology(dominant.ChakraCodeNumber + yearEnergy.ChakraCodeNumber);

            return new ChakraCodeForecast
            {
                ChartCode = dominant.ChakraCode,
                TopNumber = yearEnergy.ChakraCodeNumber,
                BottomNumber = x,
                ForecastType = EForecastType.Yearly
            };
        }

        public ChakraCodeForecast GetMonthlyForecast(PersonModel person, int? offset = 0)
        {
            var yearEnergy = CalculateCurrentYear(person);
            var currentMonth = CalculateCurrentMonth(person, offset);
            var x = CalculateNumerology(yearEnergy.ChakraCodeNumber + currentMonth.ChakraCodeNumber);

            return new ChakraCodeForecast
            {
                ChartCode = yearEnergy.ChakraCode,
                TopNumber = currentMonth.ChakraCodeNumber,
                BottomNumber = x,
                ForecastType = EForecastType.Monthly
            };
        }

        public ChakraCodeForecast GetDailyForecast(PersonModel person, int? offset = 0)
        {
            var monthEnergy = CalculateCurrentMonth(person);
            var x = CalculateNumerology(DateTime.Today.Day + (offset ?? 0));

            return new ChakraCodeForecast
            {
                ChartCode = monthEnergy.ChakraCode,
                RowNumber = x,
                ForecastType = EForecastType.Daily
            };
        }

        private DateTime GetMonthStartDate(EChakraCode energy, int month)
        {
            var firstOfMonth = new DateTime(DateTime.Now.Year, month, 1);
            var previousMonth = firstOfMonth.AddMonths(-1);

            switch (energy)
            {
                case EChakraCode.Pioneer:
                case EChakraCode.Akashic:
                    return new DateTime(previousMonth.Year, previousMonth.Month, 27);

                case EChakraCode.Peacemaker:
                case EChakraCode.Manifestor:
                case EChakraCode.Royal:
                case EChakraCode.Elder:
                    return new DateTime(previousMonth.Year, previousMonth.Month, 28).AddDays(1);

                case EChakraCode.Warrior:
                    return new DateTime(previousMonth.Year, previousMonth.Month, 23);

                case EChakraCode.Healer:
                    return new DateTime(previousMonth.Year, previousMonth.Month, 17);

                case EChakraCode.Mystic:
                    return new DateTime(previousMonth.Year, previousMonth.Month, 18);

                default:
                    return firstOfMonth;
            }
        }

        private DateTime GetMonthEndDate(EChakraCode energy, int month)
        {
            var firstOfMonth = new DateTime(DateTime.Now.Year, month, 1);
            var previousMonth = firstOfMonth.AddMonths(-1);

            switch (energy)
            {
                case EChakraCode.Pioneer:
                case EChakraCode.Akashic:
                case EChakraCode.Manifestor:
                case EChakraCode.Mystic:
                    return new DateTime(firstOfMonth.Year, firstOfMonth.Month, 28).AddDays(1);

                case EChakraCode.Peacemaker:
                    return new DateTime(firstOfMonth.Year, firstOfMonth.Month, 23);

                case EChakraCode.Warrior:
                    return new DateTime(firstOfMonth.Year, firstOfMonth.Month, 17);

                case EChakraCode.Healer:
                case EChakraCode.Elder:
                    return new DateTime(firstOfMonth.Year, firstOfMonth.Month, 27);

                case EChakraCode.Royal:
                    return new DateTime(firstOfMonth.Year, firstOfMonth.Month, 18);

                default:
                    return firstOfMonth;
            }
        }

        private DateTime GetYearStartDate(EChakraCode energy, int? year = null)
        {
            year = year ?? DateTime.Now.Year;
            var firstOfYear = new DateTime(year.Value, 1, 1);
            var previousYear = firstOfYear.AddYears(-1);

            switch (energy)
            {
                case EChakraCode.Pioneer:
                case EChakraCode.Akashic:
                    return new DateTime(previousYear.Year, 11, 15);

                case EChakraCode.Peacemaker:
                case EChakraCode.Manifestor:
                case EChakraCode.Royal:
                case EChakraCode.Elder:
                    return new DateTime(previousYear.Year, 12, 15);

                case EChakraCode.Warrior:
                    return new DateTime(previousYear.Year, 10, 1);

                case EChakraCode.Healer:
                    return new DateTime(previousYear.Year, 07, 18);

                case EChakraCode.Mystic:
                    return new DateTime(previousYear.Year, 07, 22);

                default:
                    return firstOfYear;
            }
        }

        private DateTime GetYearEndDate(EChakraCode energy, int? year = null)
        {
            year = year ?? DateTime.Now.Year;
            var firstOfYear = new DateTime(year.Value, 1, 1);

            switch (energy)
            {

                case EChakraCode.Pioneer:
                case EChakraCode.Akashic:
                case EChakraCode.Manifestor:
                case EChakraCode.Mystic:
                    return new DateTime(firstOfYear.Year, 12, 15);

                case EChakraCode.Peacemaker:
                    return new DateTime(firstOfYear.Year, 10, 1);

                case EChakraCode.Warrior:
                    return new DateTime(firstOfYear.Year, 07, 18);

                case EChakraCode.Royal:
                    return new DateTime(firstOfYear.Year, 07, 22);

                case EChakraCode.Healer:
                case EChakraCode.Elder:
                    return new DateTime(firstOfYear.Year, 11, 15);

                default:
                    return firstOfYear;
            }
        }

        private static int CalculateNumerology(int value)
        {
            return value.ToNumerology();
        }

        private static bool IsActive(DateTime dateOfBirth, int activeStartYear, int activeEndYear)
        {
            var person = new PersonModel { DateOfBirth = dateOfBirth };
            var personYearsOld = person.YearsOld >= 81 ? person.YearsOld - 81 : person.YearsOld;
            return personYearsOld >= activeStartYear && personYearsOld < activeEndYear;
        }
    }
}