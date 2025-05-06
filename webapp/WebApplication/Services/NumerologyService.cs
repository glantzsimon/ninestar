using K9.SharedLibrary.Extensions;
using K9.WebApplication.Enums;
using K9.WebApplication.Extensions;
using K9.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

            var dob = model.PersonModel.DateOfBirth;
            model.Primary = CalculateDominant(dob);
            
            model.Emergence = CalculateSubDominant(dob);
            model.Emergence.
            
            model.Actualisation = CalculateGuide(dob);

            model.Mastery = CalculateGiftCode(dob);

            model.BirthYear = CalculateBirthCode(model.PersonModel);
            model.CurrentYear = CalculateCurrentYearCode(model.PersonModel);
            model.CurrentMonth = CalculateCurrentMonth(model.PersonModel);
            model.MonthlyPlannerCodes = CalculateMonthlyPlannerCodes(model.PersonModel);
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
                Title =  $"{Globalisation.Dictionary.Emergence} ({Globalisation.Dictionary.EmergenceYears})"
            };
        }

        public NumerologyCodeDetails CalculateGuide(DateTime date)
        {
            var result = CalculateNumerology(date.Day);
            return new NumerologyCodeDetails
            {
                NumerologyCode = (ENumerologyCode)result,
                IsActive = IsActive(date, 27, 54),
                Title =  $"{Globalisation.Dictionary.Actualisation} ({Globalisation.Dictionary.ActualisationYears})"
            };
        }

        public NumerologyCodeDetails CalculateGiftCode(DateTime date)
        {
            var result = CalculateNumerology(date.Year);
            return new NumerologyCodeDetails
            {
                NumerologyCode = (ENumerologyCode)result,
                IsActive = IsActive(date, 54, 81),
                Title = $"{Globalisation.Dictionary.Mastery} ({Globalisation.Dictionary.MasteryYears})"
            };
        }

        public NumerologyCodeDetails CalculateBirthCode(PersonModel person)
        {
            var dominant = CalculateDominant(person.DateOfBirth).NumerologyCode;
            // If the dominant code is Elder, return Pioneer; otherwise, increment the code.
            var result = dominant == ENumerologyCode.Elder ? ENumerologyCode.Pioneer : dominant.Increment();
            return new NumerologyCodeDetails
            {
                NumerologyCode = result,
                Title = "Birth Year"
            };
        }

        public NumerologyCodeDetails CalculateCurrentYearCode(PersonModel person, int? offset = null)
        {
            var birthCode = CalculateBirthCode(person).NumerologyCode;
            var year = person.DateOfBirth.Year + person.YearsOld;
            // Add the person’s years-old to the birth code and adjust modulus 9
            var result = (ENumerologyCode)(((int)birthCode + person.YearsOld) % 9);
            if ((int)result == 0)
            {
                result = (ENumerologyCode)1;
            }
            var yearEndDate = GetYearEndDate(result, year);
            if (yearEndDate < DateTime.Today)
            {
                result = result.Increment();
            }
            result = AdjustCode(result, offset);
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
            var monthCodes = CalculateMonthlyPlannerCodes(person);
            var currentModel = monthCodes.FirstOrDefault(e => e.IsCurrent);
            if (currentModel == null)
            {
                return null;
            }
            var code = AdjustCode(currentModel.NumerologyCode, offset);
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
            var today = DateTime.Today;
            var currentMonth = today.Month;
            var day = new DateTime(today.Year, currentMonth, 1);
            while (day.Month == currentMonth)
            {
                var offset = day.Day - today.Day;
                items.Add(new NumerologyPlannerModel
                {
                    NumerologyCode = (ENumerologyCode)CalculateNumerology(day.Day),
                    StartDate = day,
                    EndDate = day,
                    Offset = offset
                });
                day = day.AddDays(1);
            }
            return items;
        }

        public List<NumerologyPlannerModel> CalculateMonthlyPlannerCodes(PersonModel person)
        {
            var items = new List<NumerologyPlannerModel>();
            var yearEnergy = CalculateCurrentYearCode(person);
            int monthEnergy = yearEnergy.CodeNumber - 6;
            if (monthEnergy < 1)
            {
                monthEnergy = 9 + monthEnergy;
            }

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
                    Offset = 0 // temporary; will be set below
                };

                items.Add(model);

                if (DateTime.Today >= startDate && DateTime.Today <= endDate)
                {
                    currentMonthModel = model;
                }

                // Increment month energy using the provided extension
                var en = (ENumerologyCode)monthEnergy;
                monthEnergy = (int)en.Increment();
            }

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
            var birthdayFactor = person.DateOfBirth.HasBirthdayPassed() ? -1 : 0;
            int year = person.DateOfBirth.Year + person.YearsOld + birthdayFactor;
            int offset = 5;
            var yearEnergy = currentYear.CodeNumber.Decrement(offset + 1);
            year -= offset;

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
            int age = 0;
            int year = person.DateOfBirth.Year;
            var birthCode = CalculateBirthCode(person).CodeNumber;
            int yearEnergy = (int)birthCode;

            // Build a 100-year list
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

            // Cache the first occurrence where NumerologyCodeNumber is 9.
            int indexNine = items.FindIndex(e => e.NumerologyCodeNumber == 9);
            if (indexNine == -1)
            {
                return items;
            }

            // Assign DharmaNumerologyBaseCode for ages up to the index where code equals 9.
            int skip = 0;
            int dharmaBaseCode = 9;
            for (int i = indexNine; i >= 0; i--)
            {
                items[i].DharmaNumerologyBaseCode = (ENumerologyCode)dharmaBaseCode;
                if (skip >= 1)
                {
                    dharmaBaseCode = dharmaBaseCode.Decrement();
                    skip = 0;
                }
                else
                {
                    skip++;
                }
            }

            // For ages after the code 9, assign base codes incrementally.
            skip = 0;
            dharmaBaseCode = 1;
            for (int i = indexNine + 1; i < items.Count; i++)
            {
                items[i].DharmaNumerologyBaseCode = (ENumerologyCode)dharmaBaseCode;
                if (skip >= 1)
                {
                    dharmaBaseCode = dharmaBaseCode.Increment();
                    skip = 0;
                }
                else
                {
                    skip++;
                }
            }

            // Calculate Dharma Codes for ages at or below the reference (first 9).
            var birthItem = items.First(e => e.Age == 0);
            var groupCode = birthItem.DharmaNumerologyBaseCode.Decrement();
            int dharmaCode = (int)groupCode;
            skip = 0;
            for (int i = indexNine; i >= 0; i--)
            {
                items[i].DharmaNumerologyCode = (ENumerologyCode)dharmaCode;
                items[i].DharmaGroupNumerologyCode = groupCode;
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

            // Process remaining Dharma Codes in blocks of 18.
            groupCode = items[indexNine].DharmaGroupNumerologyCode;
            int y = 1;
            while (true)
            {
                // Use the reference age plus an 18-year block.
                var refItem = items[indexNine];
                groupCode = groupCode.Increment();
                dharmaCode = (int)groupCode;
                int startAge = refItem.Age + (18 * y);
                if (startAge >= items.Count)
                {
                    break;
                }
                skip = 0;
                for (int i = 0; i < 18; i++)
                {
                    int targetAge = startAge - i;
                    if (targetAge < 0 || targetAge >= items.Count)
                    {
                        continue;
                    }
                    items[targetAge].DharmaNumerologyCode = (ENumerologyCode)dharmaCode;
                    items[targetAge].DharmaGroupNumerologyCode = groupCode;
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
            var x = CalculateNumerology((int)dominant.CodeNumber + yearEnergy.CodeNumber);
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

        #region Private Helper Methods

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
            int yr = year ?? DateTime.Now.Year;
            var firstOfYear = new DateTime(yr, 1, 1);
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
                    return new DateTime(previousYear.Year, 7, 18);
                case ENumerologyCode.Mystic:
                    return new DateTime(previousYear.Year, 7, 22);
                default:
                    return firstOfYear;
            }
        }

        private DateTime GetYearEndDate(ENumerologyCode energy, int? year = null)
        {
            int yr = year ?? DateTime.Now.Year;
            var firstOfYear = new DateTime(yr, 1, 1);
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
                    return new DateTime(firstOfYear.Year, 7, 18);
                case ENumerologyCode.Mentor:
                    return new DateTime(firstOfYear.Year, 7, 22);
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
            // Reuse the PersonModel’s YearsOld property instead of creating a new instance repeatedly.
            var yearsOld = new PersonModel { DateOfBirth = dateOfBirth }.YearsOld;
            if (yearsOld >= 81)
            {
                yearsOld -= 81;
            }
            return yearsOld >= activeStartYear && yearsOld < activeEndYear;
        }

        private ENumerologyCode AdjustCode(ENumerologyCode code, int? offset)
        {
            if (offset.HasValue)
            {
                if (offset.Value < 0)
                {
                    return code.Decrement(Math.Abs(offset.Value));
                }
                else if (offset.Value > 0)
                {
                    return code.Increment(offset.Value);
                }
            }
            return code;
        }

        #endregion
    }

}