using K9.Base.DataAccessLayer.Enums;
using K9.Base.DataAccessLayer.Extensions;
using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace K9.DataAccessLayer.Database.Seeds
{
    public static class EnergiesSeeder
    {
        public static void Seed(DbContext context)
        {
            var languages = new List<ELanguage>
                {
                    ELanguage.English
                };

            foreach (var language in languages)
            {
                var energies = new List<EnergyInfo>
                {
                    new EnergyInfo
                    {
                        Language = language,
                        EnergyType = EEnergyType.MainEnergy,
                        Energy = ENineStarEnergy.Water,
                        Trigram = Dictionary.water_trigram,
                        EnergyDescription = Dictionary.water_description,
                        Health = Dictionary.water_health,
                        Occupations = Dictionary.water_occupations,
                        PersonalDevelopemnt = Dictionary.water_personal_development
                    },
                    new EnergyInfo
                    {
                        Language = language,
                        EnergyType = EEnergyType.MainEnergy,
                        Energy = ENineStarEnergy.Soil,
                        Trigram = Dictionary.soil_trigram,
                        EnergyDescription = Dictionary.soil_description,
                        Health = Dictionary.soil_health,
                        Occupations = Dictionary.soil_occupations,
                        PersonalDevelopemnt = Dictionary.soil_personal_development
                    },
                    new EnergyInfo
                    {
                        Language = language,
                        EnergyType = EEnergyType.MainEnergy,
                        Energy = ENineStarEnergy.Thunder,
                        Trigram = Dictionary.thunder_trigram,
                        EnergyDescription = Dictionary.thunder_description,
                        Health = Dictionary.thunder_health,
                        Occupations = Dictionary.thunder_occupations,
                        PersonalDevelopemnt = Dictionary.thunder_personal_development
                    },
                    new EnergyInfo
                    {
                        Language = language,
                        EnergyType = EEnergyType.MainEnergy,
                        Energy = ENineStarEnergy.Wind,
                        Trigram = Dictionary.wind_trigram,
                        EnergyDescription = Dictionary.wind_description,
                        Health = Dictionary.wind_health,
                        Occupations = Dictionary.thunder_occupations,
                        PersonalDevelopemnt = Dictionary.thunder_personal_development
                    }

                };
                energies.ForEach(e =>
                {
                    var original = context.Find<EnergyInfo>(en => en.EnergyType == e.EnergyType && en.Energy == e.Energy).FirstOrDefault();
                    if (original == null)
                    {
                        context.Set<EnergyInfo>().Add(e);
                    }
                    else
                    {
                        original.Language = language;
                        original.Trigram = e.Trigram;
                        original.EnergyDescription = e.EnergyDescription;
                        original.Childhood = e.Childhood;
                        original.Examples = e.Examples;
                        original.Health = e.Health;
                        original.Occupations = e.Occupations;
                        original.PersonalDevelopemnt = e.PersonalDevelopemnt;

                        context.Entry(original).State = EntityState.Modified;
                    }
                });
            }

            context.SaveChanges();

        }
    }
}
