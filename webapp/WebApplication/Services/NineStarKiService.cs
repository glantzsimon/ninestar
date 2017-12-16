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
            var year = (dateOfBirth.Month == 2 && dateOfBirth.Day <= 3) || dateOfBirth.Month == 1 ? dateOfBirth.Year + 1 : dateOfBirth.Year;
            var energyNumber = 3 - ((year - 1979) % 9);
            energyNumber = energyNumber < 1 ? (9 + energyNumber) : energyNumber;
            var energy = (ENineStarEnergy) energyNumber;
            return new NineStarKiEnergy(energy);
        }
    }
}