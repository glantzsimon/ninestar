using System;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Models
{
    public class NumerologyModel
    {
        public NumerologyModel()
        {
            var dateOfBirth = new DateTime(DateTime.Now.Year - (27), DateTime.Now.Month, DateTime.Now.Day);
            PersonModel = new PersonModel
            {
                DateOfBirth = dateOfBirth
            };
        }

        public NumerologyModel(PersonModel personModel)
        {
            PersonModel = personModel;
        }

        public PersonModel PersonModel { get; set; }

        public NumerologyCodeDetails Primary { get; set; }

        public NumerologyCodeDetails Emergence { get; set; }

        public string EmergenceTitle =>
            $"{Globalisation.Dictionary.Emergence} ({Globalisation.Dictionary.EmergenceYears})";

        public NumerologyCodeDetails Actualisation { get; set; }

        public string ActualisationTitle =>
            $"{Globalisation.Dictionary.Actualisation} ({Globalisation.Dictionary.ActualisationYears})";

        public NumerologyCodeDetails Mastery { get; set; }

        public string MasteryTitle =>
            $"{Globalisation.Dictionary.Mastery} ({Globalisation.Dictionary.MasteryYears})";

        public NumerologyCodeDetails BirthYear { get; set; }

        public NumerologyCodeDetails CurrentYear { get; set; }

        public NumerologyCodeDetails CurrentMonth { get; set; }

        public List<NumerologyPlannerModel> MonthlyPlannerCodes { get; set; }

        public List<NumerologyPlannerModel> YearlyPlannerCodes { get; set; }

        public List<NumerologyPlannerModel> DailyPlannerCodes { get; set; }

        public List<DharmaNumerologyCodeModel> DharmaCodes { get; set; }

        public NumerologyForecast YearlyForecast { get; set; }

        public NumerologyForecast MonthlyForecast { get; set; }

        public NumerologyForecast DailyForecast { get; set; }

        public bool IsProcessed { get; set; }

        public List<DharmaNumerologyCodeModel> DharmaCodesFoundation => DharmaCodes.Where(e => e.Age < 27).ToList();

        public List<DharmaNumerologyCodeModel> DharmaCodesContribution => DharmaCodes.Where(e => e.Age >= 27 && e.Age < 54).ToList();

        public List<DharmaNumerologyCodeModel> DharmaCodesAscension => DharmaCodes.Where(e => e.Age >= 54 && e.Age < 81).ToList();

        public List<DharmaNumerologyCodeModel> DharmaCodesElders => DharmaCodes.Where(e => e.Age >= 81 && e.Age < 108).ToList();

        public DharmaNumerologyCodeModel FirstDharmaCode => DharmaCodes.FirstOrDefault();

    }
}