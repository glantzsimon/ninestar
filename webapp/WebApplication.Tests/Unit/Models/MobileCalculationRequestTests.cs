using K9.Base.DataAccessLayer.Enums;
using K9.WebApplication.ViewModels.MobileApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Models
{
    public class MobileCalculationRequestTests
    {
        private static MobilePersonRequest Person() => new MobilePersonRequest
        {
            DateOfBirth = new DateTime(1979, 6, 16), TimeOfBirth = new TimeSpan(7, 0, 0),
            BirthTimeZoneId = "UTC", Gender = EGender.Male
        };

        [Fact]
        public void ProfileInputCombinesBirthTimeButPlannerInputKeepsItSeparate()
        {
            var request = Person();
            var full = request.ToPerson();
            var dateOnly = request.ToPerson(false);
            Assert.Equal(new DateTime(1979, 6, 16, 7, 0, 0), full.DateOfBirth);
            Assert.Equal(new DateTime(1979, 6, 16), dateOnly.DateOfBirth);
            Assert.Equal(request.TimeOfBirth, dateOnly.TimeOfBirth);
            Assert.Equal(DateTimeKind.Unspecified, full.DateOfBirth.Kind);
            Assert.Equal(new DateTime(1979, 6, 16), request.DateOfBirth.Value);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(24)]
        public void BirthTimeCannotSpillIntoAnAdjacentDay(int hour)
        {
            var person = Person();
            person.TimeOfBirth = TimeSpan.FromHours(hour);
            Assert.False(Validator.TryValidateObject(person, new ValidationContext(person), new List<ValidationResult>(), true));
        }

        [Fact]
        public void DateTimeBearingUtcSuffixCannotBeSilentlyReinterpretedAsLocal()
        {
            var request = new MobileCalculationRequest
            {
                Person = Person(), UserTimeZoneId = "UTC",
                SelectedDateTime = new DateTime(2026, 10, 23, 0, 0, 0, DateTimeKind.Utc)
            };
            Assert.False(Validator.TryValidateObject(request, new ValidationContext(request), new List<ValidationResult>(), true));
            request.SelectedDateTime = DateTime.SpecifyKind(request.SelectedDateTime.Value, DateTimeKind.Unspecified);
            Assert.True(Validator.TryValidateObject(request, new ValidationContext(request), new List<ValidationResult>(), true));
        }

        [Fact]
        public void NestedMissingBirthDetailsAndUnknownZonesAreRejected()
        {
            var request = new MobileCalculationRequest { Person = Person(), UserTimeZoneId = "UTC" };
            request.Person.DateOfBirth = null;
            Assert.False(Validator.TryValidateObject(request, new ValidationContext(request), new List<ValidationResult>(), true));
            request.Person.DateOfBirth = new DateTime(1979, 6, 16);
            request.Person.BirthTimeZoneId = "Invalid/Zone";
            Assert.False(Validator.TryValidateObject(request, new ValidationContext(request), new List<ValidationResult>(), true));
        }
    }
}
