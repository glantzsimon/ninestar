using K9.WebApplication.Models;

namespace K9.WebApplication.ViewModels
{
    public class MagicSquareViewModel
    {
        public NineStarKiEnergy GlobalEnergy { get; set; }
        public NineStarKiEnergy HouseOccupiedEnergy { get; set; }

        public NineStarKiDirections GetDirections()
        {
            var oppositeHouseOfFiveEnergy = new NineStarKiEnergy((ENineStarKiEnergy)NineStarKiModel.GetOppositeEnergyInMagicSquare(GlobalEnergy.HouseOfFive.EnergyNumber));
            var oppositeHouseOccupiedEnergy = new NineStarKiEnergy((ENineStarKiEnergy)NineStarKiModel.GetOppositeEnergyInMagicSquare(HouseOccupiedEnergy.EnergyNumber));

            return new NineStarKiDirections(GlobalEnergy.HouseOfFive.Direction,
               oppositeHouseOfFiveEnergy.Direction,
                HouseOccupiedEnergy.Direction,
                oppositeHouseOccupiedEnergy.Direction);
        }
    }
}