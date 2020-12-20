using System;
using K9.WebApplication.Models;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Mdels
{
    public class PersonModelTests
    {
        [Theory]
        [InlineData(1979, 6, 16, 41)]
        public void GetYearsOld_HappyPath(int year, int month, int day, int age)
        {
            var now = new DateTime(2020, 12, 17);
            var personModel = new PersonModel
            {
                DateOfBirth = new DateTime(year, month, day)
            };

            var yearsOld = personModel.YearsOld;

            Assert.True(yearsOld >= 41);
        }

    }
}
