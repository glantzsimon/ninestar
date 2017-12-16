using K9.WebApplication.Models;
using System;

namespace K9.WebApplication.Services
{
    public class NineStarKiService : INineStarKiService
    {
        public NineStarKiModel Calculate(PersonModel model)
        {
            return new NineStarKiModel
            {
                MainEnergy = GetMainEnergy(model.DateOfBirth)
            };
        }

        private NineStarKiEnergy GetMainEnergy(DateTime dateOfBirth)
        {
            var energyNumber = (dateOfBirth.Year - 1979) % 9 + 3;
            var energy = (ENineStarEnergy) energyNumber;
            return new NineStarKiEnergy(energy);
        }
    }
}