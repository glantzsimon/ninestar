using K9.Base.DataAccessLayer.Extensions;
using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using K9.SharedLibrary.Helpers;

namespace K9.DataAccessLayer.Database.Seeds
{
    public static class EnergiesSeeder
	{
		public static void Seed(DbContext context)
		{
			{
				var energies = new List<EnergyInfo>
				{
					new EnergyInfo
					{
					    EnergyType = EEnergyType.MainEnergy,
                        Energy = ENineStarEnergy.Water,
                        Trigram = Dictionary.water_trigram,
                        EnergyDescription = Dictionary.water_description
					},
					
				};
			    energies.ForEach(e =>
			    {
			        var original = context.Find<EnergyInfo>(en => en.EnergyType == e.EnergyType && en.Energy == e.Energy).FirstOrDefault();
                    if(original == null)
				    {
				        context.Set<EnergyInfo>().Add(e);
				    }
				    else
                    {
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
				context.SaveChanges();
			}

		}
	}
}
