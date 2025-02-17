using K9.Globalisation;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;

namespace K9.WebApplication.Models
{
    public class NineStarKiEnergyCycleSummary
    {
        private readonly NineStarKiEnergy _nineStarKiEnergy;

        public NineStarKiEnergyCycleSummary(NineStarKiEnergy nineStarKiEnergy)
        {
            _nineStarKiEnergy = nineStarKiEnergy;
        }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EnergyLabel)]
        public string Energy => _nineStarKiEnergy.EnergyName;

        public int EnergyNumber => _nineStarKiEnergy.EnergyNumber;
        
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ElementLabel)]
        public string Element => _nineStarKiEnergy.ElementName;
        
        public string Direction => _nineStarKiEnergy.MetaData.GetDirection();

        public string Season => _nineStarKiEnergy.CycleMetaData.Season;

        public string SeasonDescription => _nineStarKiEnergy.CycleMetaData.SeasonDescription;

        public string PredictionDetails => _nineStarKiEnergy.EnergyType == ENineStarKiEnergyType.MainEnergy ? _nineStarKiEnergy.CycleMetaData.YearlyDescription : _nineStarKiEnergy.CycleMetaData.MonthlyDescription;

    }
}