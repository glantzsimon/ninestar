using K9.WebApplication.Models;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Mdels
{
    public class NineStarKiModelTests
    {

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
