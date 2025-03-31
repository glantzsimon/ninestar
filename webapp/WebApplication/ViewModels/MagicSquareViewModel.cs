using K9.WebApplication.Models;

namespace K9.WebApplication.ViewModels
{
    public class MagicSquareViewModel
    {
        public NineStarKiEnergy GlobalEnergy { get; set; }
        public NineStarKiEnergy PersonalEnergy { get; set; }

        public MagicSquareViewModel SecondMagicSquareViewModel { get; set; }

        public bool IsPersonalChart { get; set; }
        public bool IsSplit { get; set; }
        
        public NineStarKiDirections GetDirections()
        {
            var oppositeHouseOfFiveEnergy = new NineStarKiEnergy((ENineStarKiEnergy)NineStarKiModel.GetOppositeEnergyInMagicSquare(GlobalEnergy.GetHouseOfFive().EnergyNumber));
            var oppositeHouseOccupiedEnergy = new NineStarKiEnergy((ENineStarKiEnergy)NineStarKiModel.GetOppositeEnergyInMagicSquare(PersonalEnergy.EnergyNumber));

            return new NineStarKiDirections(GlobalEnergy.GetHouseOfFive().Direction,
               oppositeHouseOfFiveEnergy.Direction,
                PersonalEnergy.Direction,
                oppositeHouseOccupiedEnergy.Direction);
        }
    }
}