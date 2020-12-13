using K9.Globalisation;
using K9.WebApplication.Models;

namespace K9.WebApplication.Services
{
    public class NineStarKiService : INineStarKiService
    {
        public NineStarKiModel CalculateNineStarKi(PersonModel personModel)
        {
            var model = new NineStarKiModel(personModel);

            model.MainEnergy.EnergyDescription = GetMainEnergyDescription(model.MainEnergy.Energy);
            model.EmotionalEnergy.EnergyDescription = GetEmotionalEnergyDescription(model.EmotionalEnergy.Energy);
            model.SurfaceEnergy.EnergyDescription = GetSurfaceEnergyDescription(model.SurfaceEnergy.Energy);
            model.Health = GetHealth(model.MainEnergy.Energy);
            model.Occupations = GetOccupations(model.MainEnergy.Energy);
            model.PersonalDevelopemnt = GetPersonalDevelopemnt(model.MainEnergy.Energy);
            model.IsProcessed = true;

            return model;
        }

        private string GetMainEnergyDescription(ENineStarKiEnergy energy)
        {
            switch (energy)
            {
                case ENineStarKiEnergy.Water:
                    return Dictionary.water_description;

                case ENineStarKiEnergy.Soil:
                    return Dictionary.soil_description;

                case ENineStarKiEnergy.Thunder:
                    return Dictionary.thunder_description;

                case ENineStarKiEnergy.Wind:
                    return Dictionary.wind_description;

                case ENineStarKiEnergy.CoreEarth:
                    return Dictionary.coreearth_description;

                case ENineStarKiEnergy.Heaven:
                    return Dictionary.heaven_description;

                case ENineStarKiEnergy.Lake:
                    return Dictionary.lake_description;

                case ENineStarKiEnergy.Mountain:
                    return Dictionary.mountain_description;

                case ENineStarKiEnergy.Fire:
                    return Dictionary.fire_description;
            }

            return string.Empty;
        }

        private string GetEmotionalEnergyDescription(ENineStarKiEnergy energy)
        {
            switch (energy)
            {
                case ENineStarKiEnergy.Water:
                    return Dictionary.water_emotional_description;

                case ENineStarKiEnergy.Soil:
                    return Dictionary.soil_emotional_description;

                case ENineStarKiEnergy.Thunder:
                    return Dictionary.thunder_emotional_description;

                case ENineStarKiEnergy.Wind:
                    return Dictionary.wind_emotional_description;

                case ENineStarKiEnergy.CoreEarth:
                    return Dictionary.coreearth_emotional_description;

                case ENineStarKiEnergy.Heaven:
                    return Dictionary.heaven_emotional_description;

                case ENineStarKiEnergy.Lake:
                    return Dictionary.lake_emotional_description;

                case ENineStarKiEnergy.Mountain:
                    return Dictionary.mountain_emotional_description;

                case ENineStarKiEnergy.Fire:
                    return Dictionary.fire_emotional_description;
            }

            return string.Empty;
        }

        private string GetSurfaceEnergyDescription(ENineStarKiEnergy energy)
        {
            switch (energy)
            {
                case ENineStarKiEnergy.Water:
                    return Dictionary.water_surface_description;

                case ENineStarKiEnergy.Soil:
                    return Dictionary.soil_surface_description;

                case ENineStarKiEnergy.Thunder:
                    return Dictionary.thunder_surface_description;

                case ENineStarKiEnergy.Wind:
                    return Dictionary.wind_surface_description;

                case ENineStarKiEnergy.CoreEarth:
                    return Dictionary.coreearth_surface_description;

                case ENineStarKiEnergy.Heaven:
                    return Dictionary.heaven_surface_description;

                case ENineStarKiEnergy.Lake:
                    return Dictionary.lake_surface_description;

                case ENineStarKiEnergy.Mountain:
                    return Dictionary.mountain_surface_description;

                case ENineStarKiEnergy.Fire:
                    return Dictionary.fire_surface_description;
            }

            return string.Empty;
        }

        private string GetHealth(ENineStarKiEnergy energy)
        {
            switch (energy)
            {
                case ENineStarKiEnergy.Water:
                    return Dictionary.water_health;

                case ENineStarKiEnergy.Soil:
                    return Dictionary.soil_health;

                case ENineStarKiEnergy.Thunder:
                    return Dictionary.thunder_health;

                case ENineStarKiEnergy.Wind:
                    return Dictionary.wind_health;

                case ENineStarKiEnergy.CoreEarth:
                    return Dictionary.coreearth_health;

                case ENineStarKiEnergy.Heaven:
                    return Dictionary.heaven_health;

                case ENineStarKiEnergy.Lake:
                    return Dictionary.lake_health;

                case ENineStarKiEnergy.Mountain:
                    return Dictionary.mountain_health;

                case ENineStarKiEnergy.Fire:
                    return Dictionary.fire_health;
            }

            return string.Empty;
        }

        private string GetOccupations(ENineStarKiEnergy energy)
        {
            switch (energy)
            {
                case ENineStarKiEnergy.Water:
                    return Dictionary.water_occupations;

                case ENineStarKiEnergy.Soil:
                    return Dictionary.soil_occupations;

                case ENineStarKiEnergy.Thunder:
                    return Dictionary.thunder_occupations;

                case ENineStarKiEnergy.Wind:
                    return Dictionary.wind_occupations;

                case ENineStarKiEnergy.CoreEarth:
                    return Dictionary.coreearth_occupations;

                case ENineStarKiEnergy.Heaven:
                    return Dictionary.heaven_occupations;

                case ENineStarKiEnergy.Lake:
                    return Dictionary.lake_occupations;

                case ENineStarKiEnergy.Mountain:
                    return Dictionary.mountain_occupations;

                case ENineStarKiEnergy.Fire:
                    return Dictionary.fire_occupations;
            }

            return string.Empty;
        }

        private string GetPersonalDevelopemnt(ENineStarKiEnergy energy)
        {
            switch (energy)
            {
                case ENineStarKiEnergy.Water:
                    return Dictionary.water_personal_development;

                case ENineStarKiEnergy.Soil:
                    return Dictionary.soil_personal_development;

                case ENineStarKiEnergy.Thunder:
                    return Dictionary.thunder_personal_development;

                case ENineStarKiEnergy.Wind:
                    return Dictionary.wind_personal_development;

                case ENineStarKiEnergy.CoreEarth:
                    return Dictionary.coreearth_personal_development;

                case ENineStarKiEnergy.Heaven:
                    return Dictionary.heaven_personal_development;

                case ENineStarKiEnergy.Lake:
                    return Dictionary.lake_personal_development;

                case ENineStarKiEnergy.Mountain:
                    return Dictionary.mountain_personal_development;

                case ENineStarKiEnergy.Fire:
                    return Dictionary.fire_personal_development;
            }

            return string.Empty;
        }
    }
}