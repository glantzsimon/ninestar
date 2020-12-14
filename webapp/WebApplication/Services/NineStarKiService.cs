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
            model.Summary = GetSummary(model);
            model.Overview = GetOverview(model.MainEnergy.Energy);
            model.IsProcessed = true;

            return model;
        }

        private string GetOverview(ENineStarKiEnergy energy)
        {
            switch (energy)
            {
                case ENineStarKiEnergy.Water:
                    return Dictionary.water_overview;

                case ENineStarKiEnergy.Soil:
                    return Dictionary._124;

                case ENineStarKiEnergy.Thunder:
                    return Dictionary._133;

                case ENineStarKiEnergy.Wind:
                    return Dictionary._142;

                case ENineStarKiEnergy.CoreEarth:
                    return Dictionary._151;

                case ENineStarKiEnergy.Heaven:
                    return Dictionary._169;

                case ENineStarKiEnergy.Lake:
                    return Dictionary._178;

                case ENineStarKiEnergy.Mountain:
                    return Dictionary._187;

                case ENineStarKiEnergy.Fire:
                    return Dictionary._196;
            }

            return string.Empty;
        }

        private string GetSummary(NineStarKiModel model)
        {
            switch (model.MainEnergy.Energy)
            {
                case ENineStarKiEnergy.Water:
                    switch (model.EmotionalEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            return Dictionary._115;

                        case ENineStarKiEnergy.Soil:
                            return Dictionary._124;

                        case ENineStarKiEnergy.Thunder:
                            return Dictionary._133;

                        case ENineStarKiEnergy.Wind:
                            return Dictionary._142;

                        case ENineStarKiEnergy.CoreEarth:
                            return Dictionary._151;

                        case ENineStarKiEnergy.Heaven:
                            return Dictionary._169;

                        case ENineStarKiEnergy.Lake:
                            return Dictionary._178;

                        case ENineStarKiEnergy.Mountain:
                            return Dictionary._187;

                        case ENineStarKiEnergy.Fire:
                            return Dictionary._196;
                    }

                    break;

                case ENineStarKiEnergy.Soil:
                    switch (model.EmotionalEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            return Dictionary._216;

                        case ENineStarKiEnergy.Soil:
                            return Dictionary._225;

                        case ENineStarKiEnergy.Thunder:
                            return Dictionary._234;

                        case ENineStarKiEnergy.Wind:
                            return Dictionary._243;

                        case ENineStarKiEnergy.CoreEarth:
                            return Dictionary._252;

                        case ENineStarKiEnergy.Heaven:
                            return Dictionary._261;

                        case ENineStarKiEnergy.Lake:
                            return Dictionary._279;

                        case ENineStarKiEnergy.Mountain:
                            return Dictionary._288;

                        case ENineStarKiEnergy.Fire:
                            return Dictionary._297;
                    }

                    break;

                case ENineStarKiEnergy.Thunder:
                    switch (model.EmotionalEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            return Dictionary._317;

                        case ENineStarKiEnergy.Soil:
                            return Dictionary._326;

                        case ENineStarKiEnergy.Thunder:
                            return Dictionary._335;

                        case ENineStarKiEnergy.Wind:
                            return Dictionary._344;

                        case ENineStarKiEnergy.CoreEarth:
                            return Dictionary._353;

                        case ENineStarKiEnergy.Heaven:
                            return Dictionary._362;

                        case ENineStarKiEnergy.Lake:
                            return Dictionary._371;

                        case ENineStarKiEnergy.Mountain:
                            return Dictionary._389;

                        case ENineStarKiEnergy.Fire:
                            return Dictionary._398;
                    }

                    return string.Empty;
            }

            return string.Empty;
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