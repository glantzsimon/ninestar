using K9.WebApplication.Constants;
using K9.WebApplication.Enums;
using K9.WebApplication.Extensions;
using K9.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using K9.SharedLibrary.Extensions;

namespace K9.WebApplication.Services
{
    public class NumerologyService : INumerologyService
    {
        public NumerologyModel Calculate(NumerologyModel model)
        {
            if (model.PersonModel == null)
            {
                return model;
            }

            model.Primary = CalculateDominant(model.PersonModel.DateOfBirth);
            model.Emergence = CalculateSubDominant(model.PersonModel.DateOfBirth);
            model.Actualisation = CalculateGuide(model.PersonModel.DateOfBirth);
            model.Mastery = CalculateGiftCode(model.PersonModel.DateOfBirth);
            model.BirthYear = CalculateBirthCode(model.PersonModel);
            model.CurrentYear = CalculateCurrentYearCode(model.PersonModel);
            model.CurrentMonth = CalculateCurrentMonth(model.PersonModel);
            model.MonthlyPlannerCodes = CalculateMonthCodes(model.PersonModel);
            model.YearlyPlannerCodes = CalculateYearlyPlannerCodes(model.PersonModel);
            model.DailyPlannerCodes = CalculateDailyPlannerCodes(model.PersonModel);
            model.DharmaCodes = CalculateDharmaCodes(model.PersonModel);
            model.YearlyForecast = GetYearlyForecast(model.PersonModel);
            model.MonthlyForecast = GetMonthlyForecast(model.PersonModel);
            model.DailyForecast = GetDailyForecast(model.PersonModel);

            model.IsProcessed = true;

            return model;
        }

        public NumerologyCodeDetails CalculateDominant(DateTime date)
        {
            var result = CalculateNumerology(date.Year + date.Month + date.Day);

            return new NumerologyCodeDetails
            {
                NumerologyCode = (ENumerologyCode)result,
                Title = "Dominant"
            };
        }

        public NumerologyCodeDetails CalculateSubDominant(DateTime date)
        {
            var result = date.Month > 9 ? date.Month - 9 : date.Month;

            return new NumerologyCodeDetails
            {
                NumerologyCode = (ENumerologyCode)result,
                IsActive = IsActive(date, 0, 27),
                Title = "Sub Dominant"
            };
        }

        public NumerologyCodeDetails CalculateGuide(DateTime date)
        {
            var result = CalculateNumerology(date.Day);

            return new NumerologyCodeDetails
            {
                NumerologyCode = (ENumerologyCode)result,
                IsActive = IsActive(date, 27, 54),
                Title = "Guide"
            };
        }

        public NumerologyCodeDetails CalculateGiftCode(DateTime date)
        {
            var result = CalculateNumerology(date.Year);

            return new NumerologyCodeDetails
            {
                NumerologyCode = (ENumerologyCode)result,
                IsActive = IsActive(date, 54, 81),
                Title = "Gift"
            };
        }

        public NumerologyCodeDetails CalculateBirthCode(PersonModel person)
        {
            var result = CalculateDominant(person.DateOfBirth).NumerologyCode;

            result = result == ENumerologyCode.Elder ? ENumerologyCode.Pioneer : result + 1;

            return new NumerologyCodeDetails
            {
                NumerologyCode = result,
                Title = "Birth Year"
            };
        }

        public NumerologyCodeDetails CalculateCurrentYearCode(PersonModel person, int? offset = null)
        {
            var result = CalculateBirthCode(person).NumerologyCode;
            var year = person.DateOfBirth.Year + person.YearsOld;

            result = (ENumerologyCode)(((int)result + person.YearsOld) % 9);

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

            return new NumerologyCodeDetails
            {
                NumerologyCode = result,
                Title = "Current Year",
                StartDate = GetYearStartDate(result),
                EndDate = GetYearEndDate(result),
                ShowDates = true
            };
        }

        public NumerologyCodeDetails CalculateCurrentMonth(PersonModel person, int? offset = null)
        {
            var monthCodes = CalculateMonthCodes(person);
            var code = monthCodes.FirstOrDefault(e => e.IsCurrent).NumerologyCode;

            if (offset < 0)
            {
                code = code.Decrement(Math.Abs(offset.Value));
            }

            if (offset > 0)
            {
                code = code.Increment(offset.Value);
            }

            var activeMonth = monthCodes.FirstOrDefault(e => e.NumerologyCode == code);

            return activeMonth == null ? null : new NumerologyCodeDetails
            {
                NumerologyCode = activeMonth.NumerologyCode,
                Title = "Current Month",
                StartDate = activeMonth.StartDate,
                EndDate = activeMonth.EndDate
            };
        }

        public List<NumerologyPlannerModel> CalculateDailyPlannerCodes(PersonModel person)
        {
            var items = new List<NumerologyPlannerModel>();
            var currentMonth = DateTime.Today.Month;
            var i = 1;
            var day = new DateTime(DateTime.Today.Year, currentMonth, i);

            while (currentMonth == day.Month)
            {
                var offset = day.Day - DateTime.Today.Day;

                items.Add(new NumerologyPlannerModel
                {
                    NumerologyCode = (ENumerologyCode)CalculateNumerology(day.Day),
                    StartDate = day,
                    EndDate = day,
                    Offset = offset
                });

                day = day.AddDays(i);
            }

            return items;
        }

        public List<NumerologyPlannerModel> CalculateMonthCodes(PersonModel person)
        {
            var items = new List<NumerologyPlannerModel>();
            var yearEnergy = CalculateCurrentYearCode(person);
            var monthEnergy = yearEnergy.CodeNumber - 6;

            monthEnergy = monthEnergy < 1 ? (9 + monthEnergy) : monthEnergy;

            // List to store month models
            NumerologyPlannerModel currentMonthModel = null;

            for (int i = 0; i < 12; i++)
            {
                var monthNumber = i + 1;
                var startDate = GetMonthStartDate((ENumerologyCode)monthEnergy, monthNumber);
                var endDate = GetMonthEndDate((ENumerologyCode)monthEnergy, monthNumber);

                var model = new NumerologyPlannerModel
                {
                    NumerologyCode = (ENumerologyCode)monthEnergy,
                    StartDate = startDate,
                    EndDate = endDate,
                    Offset = 0 // Temporary
                };

                items.Add(model);

                // Check if this is the current month
                if (DateTime.Today >= startDate && DateTime.Today <= endDate)
                {
                    currentMonthModel = model;
                }

                monthEnergy = monthEnergy.Increment();
            }

            // Calculate offsets relative to the current month
            if (currentMonthModel != null)
            {
                int currentIndex = items.IndexOf(currentMonthModel);

                for (int i = 0; i < items.Count; i++)
                {
                    items[i].Offset = i - currentIndex;
                }
            }

            return items;
        }


        public List<NumerologyPlannerModel> CalculateYearlyPlannerCodes(PersonModel person)
        {
            var items = new List<NumerologyPlannerModel>();
            var currentYear = CalculateCurrentYearCode(person);
            var endOfYear = GetYearEndDate(currentYear.NumerologyCode);
            var birthdayFactor = person.DateOfBirth.HasBirthdayPassedThisYear() ? -1 : 0;
            var year = person.DateOfBirth.Year + person.YearsOld + birthdayFactor;
            var offset = 5;
            var yearEnergy = currentYear.CodeNumber.Decrement(offset + 1);

            year = year - offset;

            for (int i = 0; i < 12; i++)
            {
                items.Add(new NumerologyPlannerModel
                {
                    NumerologyCode = (ENumerologyCode)yearEnergy,
                    StartDate = GetYearStartDate((ENumerologyCode)yearEnergy, year),
                    EndDate = GetYearEndDate((ENumerologyCode)yearEnergy, year),
                    Offset = i - offset
                });

                year++;
                yearEnergy = yearEnergy.Increment();
            }

            return items;
        }

        public List<DharmaNumerologyCodeModel> CalculateDharmaCodes(PersonModel person)
        {
            var items = new List<DharmaNumerologyCodeModel>();
            var age = 0;
            var year = person.DateOfBirth.Year;
            var yearEnergy = CalculateBirthCode(person).CodeNumber;

            for (int i = 0; i < 100; i++)
            {
                items.Add(new DharmaNumerologyCodeModel
                {
                    Age = age,
                    Year = year,
                    NumerologyCode = (ENumerologyCode)yearEnergy
                });

                age++;
                year++;
                yearEnergy = yearEnergy.Increment();
            }

            // Get first 9
            var skip = 0;
            var dharmaBaseCode = 9;
            var code = items.First(e => e.NumerologyCodeNumber == 9);

            age = code.Age;

            while (age >= 0)
            {
                code = items.First(e => e.Age == age);
                code.DharmaNumerologyBaseCode = (ENumerologyCode)dharmaBaseCode;

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
            code = items.First(e => e.NumerologyCodeNumber == 9);
            age = code.Age + 1;
            dharmaBaseCode = 1;
            skip = 0;

            while (age < 100)
            {
                code = items.First(e => e.Age == age);
                code.DharmaNumerologyBaseCode = (ENumerologyCode)dharmaBaseCode;

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
            var groupCode = birthCode.DharmaNumerologyBaseCode.Decrement();
            var dharmaCode = (int)groupCode;

            code = items.First(e => e.NumerologyCodeNumber == 9);
            age = code.Age;
            skip = 0;

            while (age >= 0)
            {
                code = items.First(e => e.Age == age);
                code.DharmaNumerologyCode = (ENumerologyCode)dharmaCode;
                code.DharmaGroupNumerologyCode = groupCode;

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
            code = items.First(e => e.NumerologyCodeNumber == 9);
            var y = 1;
            while (code != null)
            {
                code = items.First(e => e.NumerologyCodeNumber == 9);
                groupCode = groupCode.Increment();
                dharmaCode = (int)groupCode;
                age = code.Age + (18 * y);
                skip = 0;

                for (int i = 0; i < 18; i++)
                {
                    code = items.FirstOrDefault(e => e.Age == age - i);
                    if (code != null)
                    {
                        code.DharmaNumerologyCode = (ENumerologyCode)dharmaCode;
                        code.DharmaGroupNumerologyCode = groupCode;
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

        public NumerologyForecast GetYearlyForecast(PersonModel person, int? offset = 0)
        {
            var dominant = CalculateDominant(person.DateOfBirth);
            var yearEnergy = CalculateCurrentYearCode(person, offset);
            var x = CalculateNumerology(dominant.CodeNumber + yearEnergy.CodeNumber);

            return new NumerologyForecast
            {
                ChartCode = dominant.NumerologyCode,
                TopNumber = yearEnergy.CodeNumber,
                BottomNumber = x,
                ForecastType = EForecastType.Yearly
            };
        }

        public NumerologyForecast GetMonthlyForecast(PersonModel person, int? offset = 0)
        {
            var yearEnergy = CalculateCurrentYearCode(person);
            var currentMonth = CalculateCurrentMonth(person, offset);
            var x = CalculateNumerology(yearEnergy.CodeNumber + currentMonth.CodeNumber);

            return new NumerologyForecast
            {
                ChartCode = yearEnergy.NumerologyCode,
                TopNumber = currentMonth.CodeNumber,
                BottomNumber = x,
                ForecastType = EForecastType.Monthly
            };
        }

        public NumerologyForecast GetDailyForecast(PersonModel person, int? offset = 0)
        {
            var monthEnergy = CalculateCurrentMonth(person);
            var x = CalculateNumerology(DateTime.Today.Day + (offset ?? 0));

            return new NumerologyForecast
            {
                ChartCode = monthEnergy.NumerologyCode,
                RowNumber = x,
                ForecastType = EForecastType.Daily
            };
        }

        private DateTime GetMonthStartDate(ENumerologyCode energy, int month)
        {
            var firstOfMonth = new DateTime(DateTime.Now.Year, month, 1);
            var previousMonth = firstOfMonth.AddMonths(-1);

            switch (energy)
            {
                case ENumerologyCode.Pioneer:
                case ENumerologyCode.Visionary:
                    return new DateTime(previousMonth.Year, previousMonth.Month, 27);

                case ENumerologyCode.Peacemaker:
                case ENumerologyCode.Manifestor:
                case ENumerologyCode.Mentor:
                case ENumerologyCode.Elder:
                    return new DateTime(previousMonth.Year, previousMonth.Month, 28).AddDays(1);

                case ENumerologyCode.Revolutionary:
                    return new DateTime(previousMonth.Year, previousMonth.Month, 23);

                case ENumerologyCode.Healer:
                    return new DateTime(previousMonth.Year, previousMonth.Month, 17);

                case ENumerologyCode.Mystic:
                    return new DateTime(previousMonth.Year, previousMonth.Month, 18);

                default:
                    return firstOfMonth;
            }
        }

        private DateTime GetMonthEndDate(ENumerologyCode energy, int month)
        {
            var firstOfMonth = new DateTime(DateTime.Now.Year, month, 1);
            var previousMonth = firstOfMonth.AddMonths(-1);

            switch (energy)
            {
                case ENumerologyCode.Pioneer:
                case ENumerologyCode.Visionary:
                case ENumerologyCode.Manifestor:
                case ENumerologyCode.Mystic:
                    return new DateTime(firstOfMonth.Year, firstOfMonth.Month, 28).AddDays(1);

                case ENumerologyCode.Peacemaker:
                    return new DateTime(firstOfMonth.Year, firstOfMonth.Month, 23);

                case ENumerologyCode.Revolutionary:
                    return new DateTime(firstOfMonth.Year, firstOfMonth.Month, 17);

                case ENumerologyCode.Healer:
                case ENumerologyCode.Elder:
                    return new DateTime(firstOfMonth.Year, firstOfMonth.Month, 27);

                case ENumerologyCode.Mentor:
                    return new DateTime(firstOfMonth.Year, firstOfMonth.Month, 18);

                default:
                    return firstOfMonth;
            }
        }

        private DateTime GetYearStartDate(ENumerologyCode energy, int? year = null)
        {
            year = year ?? DateTime.Now.Year;
            var firstOfYear = new DateTime(year.Value, 1, 1);
            var previousYear = firstOfYear.AddYears(-1);

            switch (energy)
            {
                case ENumerologyCode.Pioneer:
                case ENumerologyCode.Visionary:
                    return new DateTime(previousYear.Year, 11, 15);

                case ENumerologyCode.Peacemaker:
                case ENumerologyCode.Manifestor:
                case ENumerologyCode.Mentor:
                case ENumerologyCode.Elder:
                    return new DateTime(previousYear.Year, 12, 15);

                case ENumerologyCode.Revolutionary:
                    return new DateTime(previousYear.Year, 10, 1);

                case ENumerologyCode.Healer:
                    return new DateTime(previousYear.Year, 07, 18);

                case ENumerologyCode.Mystic:
                    return new DateTime(previousYear.Year, 07, 22);

                default:
                    return firstOfYear;
            }
        }

        private DateTime GetYearEndDate(ENumerologyCode energy, int? year = null)
        {
            year = year ?? DateTime.Now.Year;
            var firstOfYear = new DateTime(year.Value, 1, 1);

            switch (energy)
            {

                case ENumerologyCode.Pioneer:
                case ENumerologyCode.Visionary:
                case ENumerologyCode.Manifestor:
                case ENumerologyCode.Mystic:
                    return new DateTime(firstOfYear.Year, 12, 15);

                case ENumerologyCode.Peacemaker:
                    return new DateTime(firstOfYear.Year, 10, 1);

                case ENumerologyCode.Revolutionary:
                    return new DateTime(firstOfYear.Year, 07, 18);

                case ENumerologyCode.Mentor:
                    return new DateTime(firstOfYear.Year, 07, 22);

                case ENumerologyCode.Healer:
                case ENumerologyCode.Elder:
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