using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using System.Linq;

namespace K9.WebApplication.Services
{
    public class NineStarKiService : INineStarKiService
    {
        private readonly IRepository<NineStarKiPersonalProfile> _energyRepository;

        public NineStarKiService(IRepository<NineStarKiPersonalProfile> energyRepository)
        {
            _energyRepository = energyRepository;
        }

        public NineStarKiViewModel CalculateNineStarKi(PersonModel personModel)
        {
            var result = new NineStarKiViewModel
            {
                NineStarKiModel = new NineStarKiModel(personModel)
            };
            
            result.MainEnergyInfo = _energyRepository.Find(e => e.EnergyType == EEnergyType.MainEnergy && e.Energy == result.NineStarKiModel.MainEnergy.Energy).FirstOrDefault() ?? new NineStarKiPersonalProfile
            {
                EnergyType = EEnergyType.MainEnergy
            };
            result.EmotionalEnergyInfo = _energyRepository.Find(e => e.EnergyType == EEnergyType.EmotionalEnergy && e.Energy == result.NineStarKiModel.EmotionalEnergy.Energy).FirstOrDefault() ?? new NineStarKiPersonalProfile
            {
                EnergyType = EEnergyType.EmotionalEnergy
            };
            result.SurfaceEnergyInfo = _energyRepository.Find(e => e.EnergyType == EEnergyType.SurfaceEnergy && e.Energy == result.NineStarKiModel.SurfaceEnergy.Energy).FirstOrDefault() ?? new NineStarKiPersonalProfile
            {
                EnergyType = EEnergyType.SurfaceEnergy
            };

            return result;
        }
    }
}