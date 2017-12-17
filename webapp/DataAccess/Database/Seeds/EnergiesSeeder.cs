using K9.Base.DataAccessLayer.Extensions;
using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using System.Collections.Generic;
using System.Data.Entity;
using K9.Globalisation;

namespace K9.DataAccessLayer.Database.Seeds
{
    public static class EnergiesSeeder
	{
		public static void SeedEnergies(DbContext context)
		{
			{
				var energies = new List<EnergyInfo>
				{
					new EnergyInfo
					{
					    EnergyType = EEnergyType.MainEnergy,
                        Energy = ENineStarEnergy.Water,
                        Trigram = Dictionary.water_trigram
					},
					//new EnergyInfo {FirstMidName = "Meredith", LastName = "Alonso", EnrollmentDate = DateTime.Parse("2002-09-01")},
				};
			    energies.ForEach(e =>
				{
				    if (!context.Exists<EnergyInfo>(en => en.EnergyType == e.EnergyType && en.Energy == e.Energy))
				    {
				        context.Set<EnergyInfo>().Add(e);
				    }
				    else
				    {
                        context.Set<EnergyInfo>().Attach(e);
				        context.Entry(e).State = EntityState.Modified;
                    }
				});
				context.SaveChanges();
			}

		}
	}
}
