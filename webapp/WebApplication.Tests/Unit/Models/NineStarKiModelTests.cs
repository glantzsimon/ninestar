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

        [Theory]
        [InlineData(1, 3, 7)]
        [InlineData(1, 9, 4)]
        [InlineData(2, 8, 2)]
        [InlineData(2, 3, 6)]
        [InlineData(3, 3, 5)]
        [InlineData(3, 4, 6)]
        [InlineData(4, 7, 8)]
        [InlineData(5, 2, 2)]
        [InlineData(6, 1, 9)]
        [InlineData(7, 5, 3)]
        [InlineData(8, 8, 5)]
        [InlineData(8, 9, 6)]
        [InlineData(9, 2, 7)]
        public void MagicSquare_Test(
            int cycleEnergy, int mainEnergy, int houseOccupied)
        {
            var result = NineStarKiModel.GetHouseOccupiedByNumber(cycleEnergy, mainEnergy);
            Assert.Equal(houseOccupied, result);
        }

    }
}
