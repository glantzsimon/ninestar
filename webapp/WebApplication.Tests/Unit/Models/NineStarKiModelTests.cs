using K9.Base.DataAccessLayer.Enums;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using System;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Mdels
{
    public class NineStarKiModelTests
    {

        [Theory]
        [InlineData(1979, 6, 16, EGender.Male, ESexualityRelationType.MatchMatch)]
        [InlineData(1979, 7, 16, EGender.Male, ESexualityRelationType.MatchOpposite)]
        [InlineData(1984, 6, 21, EGender.Male, ESexualityRelationType.OppositeOpposite)]
        [InlineData(1984, 7, 21, EGender.Male, ESexualityRelationType.OppositeMatch)]
        [InlineData(1984, 6, 21, EGender.Female, ESexualityRelationType.OppositeOpposite)]
        public void SexualityRelationType_HappyPath(int year, int month, int day, EGender gender, ESexualityRelationType relationType)
        {
            var personModel = new PersonModel
            {
                DateOfBirth = new DateTime(year, month, day),
                Gender = gender
            };

            var nineStarKiModel = new NineStarKiModel(personModel);

            Assert.Equal(relationType, nineStarKiModel.SexualityRelationType);
        }

    }
}
