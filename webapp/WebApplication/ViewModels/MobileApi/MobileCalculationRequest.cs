using K9.Base.DataAccessLayer.Enums;
using K9.DataAccessLayer.Enums;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TimeZoneConverter;

namespace K9.WebApplication.ViewModels.MobileApi
{
    public class MobilePersonRequest : IValidatableObject
    {
        [StringLength(200)]
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public TimeSpan TimeOfBirth { get; set; }
        public bool TimeOfBirthKnown { get; set; }
        public string BirthTimeZoneId { get; set; }
        public EGender Gender { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (!DateOfBirth.HasValue || DateOfBirth.Value.Year < 1800 || DateOfBirth.Value.Year > 2200)
                yield return new ValidationResult("invalid_birth_date", new[] { nameof(DateOfBirth) });
            if (DateOfBirth.HasValue && DateOfBirth.Value.TimeOfDay != TimeSpan.Zero)
                yield return new ValidationResult("birth_date_must_be_date_only", new[] { nameof(DateOfBirth) });
            if (TimeOfBirth < TimeSpan.Zero || TimeOfBirth >= TimeSpan.FromDays(1))
                yield return new ValidationResult("invalid_birth_time", new[] { nameof(TimeOfBirth) });
            if (!Enum.IsDefined(typeof(EGender), Gender))
                yield return new ValidationResult("invalid_gender", new[] { nameof(Gender) });
            if (!IsTimeZone(BirthTimeZoneId))
                yield return new ValidationResult("invalid_birth_timezone", new[] { nameof(BirthTimeZoneId) });
        }

        public PersonModel ToPerson(bool combineBirthTime = true) => new PersonModel
        {
            Name = Name,
            DateOfBirth = DateTime.SpecifyKind(DateOfBirth.Value.Date.Add(combineBirthTime ? TimeOfBirth : TimeSpan.Zero), DateTimeKind.Unspecified),
            TimeOfBirth = TimeOfBirth,
            BirthTimeZoneId = BirthTimeZoneId,
            Gender = Gender
        };

        public static bool IsTimeZone(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 200) return false;
            try { TZConvert.GetTimeZoneInfo(DateTimeHelper.ValidateTimeZoneId(value)); return true; }
            catch (TimeZoneNotFoundException) { return false; }
            catch (InvalidTimeZoneException) { return false; }
            catch (ArgumentException) { return false; }
        }
    }

    public class MobileCalculationRequest : IValidatableObject
    {
        public MobilePersonRequest Person { get; set; }
        public DateTime? SelectedDateTime { get; set; }
        public string UserTimeZoneId { get; set; }
        public ECalculationMethod CalculationMethod { get; set; } = ECalculationMethod.Chinese;
        public ECalculatorType CalculatorType { get; set; } = ECalculatorType.Simple;
        public EHousesDisplay HousesDisplay { get; set; } = EHousesDisplay.SolarHouse;
        public bool InvertDailyAndHourlyKiForSouthernHemisphere { get; set; }
        public bool InvertDailyAndHourlyCycleKiForSouthernHemisphere { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (Person == null) yield return new ValidationResult("person_required");
            else
            {
                var results = new List<ValidationResult>();
                Validator.TryValidateObject(Person, new ValidationContext(Person), results, true);
                foreach (var error in results) yield return error;
            }
            if (!MobilePersonRequest.IsTimeZone(UserTimeZoneId)) yield return new ValidationResult("invalid_user_timezone");
            if (SelectedDateTime.HasValue && (SelectedDateTime.Value.Year < 1800 || SelectedDateTime.Value.Year > 2200 || SelectedDateTime.Value.Kind != DateTimeKind.Unspecified))
                yield return new ValidationResult("selected_date_must_be_local");
            if (!Enum.IsDefined(typeof(ECalculationMethod), CalculationMethod) ||
                !Enum.IsDefined(typeof(ECalculatorType), CalculatorType) || !Enum.IsDefined(typeof(EHousesDisplay), HousesDisplay))
                yield return new ValidationResult("invalid_calculation_settings");
        }

        public DateTime GetSelectedDate(DateTime utcNow) => SelectedDateTime ?? DateTime.SpecifyKind(
            DateTimeHelper.ConvertToLocaleDateTime(utcNow, UserTimeZoneId), DateTimeKind.Unspecified);
    }

    public class MobilePlannerRequest : MobileCalculationRequest
    {
        public EPlannerView View { get; set; } = EPlannerView.Month;
        public EScopeDisplay Display { get; set; } = EScopeDisplay.PersonalKi;
        public EPlannerNavigationDirection NavigationDirection { get; set; } = EPlannerNavigationDirection.None;
        public bool IncludeMoonPhases { get; set; } = true;
    }

    public class MobileCompatibilityRequest : IValidatableObject
    {
        public MobilePersonRequest FirstPerson { get; set; }
        public MobilePersonRequest SecondPerson { get; set; }
        public ECalculationMethod CalculationMethod { get; set; } = ECalculationMethod.Chinese;
        public ECalculatorType CalculatorType { get; set; } = ECalculatorType.Advanced;
        public bool HideSexualChemistry { get; set; } = true;

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            foreach (var person in new[] { FirstPerson, SecondPerson })
            {
                if (person == null) { yield return new ValidationResult("person_required"); continue; }
                var errors = new List<ValidationResult>();
                Validator.TryValidateObject(person, new ValidationContext(person), errors, true);
                foreach (var error in errors) yield return error;
            }
            if (!Enum.IsDefined(typeof(ECalculationMethod), CalculationMethod) || !Enum.IsDefined(typeof(ECalculatorType), CalculatorType))
                yield return new ValidationResult("invalid_calculation_settings");
        }
    }
}
