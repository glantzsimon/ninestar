using K9.WebApplication.Models;

namespace K9.WebApplication.ViewModels
{
    public class MagicSquareViewModel
    {
        public NineStarKiEnergy GlobalKi { get; set; }
        public NineStarKiEnergy PersonalHouseOccupied { get; set; }
        public NineStarKiEnergy PersonalChartEnergy { get; set; }

        public MagicSquareViewModel SecondMagicSquareViewModel { get; set; }

        public bool IsPersonalChart { get; set; }
        public bool IsSplit { get; set; }
        
        public NineStarKiDirections GetDirections()
        {
            var oppositeHouseOfFiveEnergy = new NineStarKiEnergy((ENineStarKiEnergy)NineStarKiModel.GetOppositeEnergyInMagicSquare(GlobalKi.GetHouseOfFive().EnergyNumber), GlobalKi.EnergyCycleType);
            var oppositeHouseOccupiedEnergy = new NineStarKiEnergy((ENineStarKiEnergy)NineStarKiModel.GetOppositeEnergyInMagicSquare(PersonalHouseOccupied.EnergyNumber), PersonalHouseOccupied.EnergyCycleType);

            return new NineStarKiDirections(GlobalKi.GetHouseOfFive(),
               oppositeHouseOfFiveEnergy,
                PersonalHouseOccupied,
                oppositeHouseOccupiedEnergy,
                PersonalChartEnergy);
        }
    }
}