using System;
using System.Collections.Generic;

namespace K9.WebApplication.ViewModels.MobileApi
{
    // Transport-only types: no EF entities, session access, or calculated getters.
    public class MobilePersonalChartResponse
    {
        public MobileChartPerson Person { get; set; }
        public MobileChartSettings Settings { get; set; }
        public string Culture { get; set; }
        public string Summary { get; set; }
        public string Overview { get; set; }
        public Dictionary<string, MobileChartEnergy> Energies { get; set; }
        public Dictionary<string, MobileChartText> Sections { get; set; }
        public string StrongYinOrgan { get; set; }
        public string StrongYangOrgan { get; set; }
        public string[] WeakYinOrgans { get; set; }
        public string[] WeakYangOrgans { get; set; }
    }

    public class MobileChartPerson
    {
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public string TimeOfBirth { get; set; }
        public bool TimeOfBirthKnown { get; set; }
        public string BirthTimeZoneId { get; set; }
        public string Gender { get; set; }
        public bool IsAdult { get; set; }
        public bool IsSixteenOrOver { get; set; }
    }

    public class MobileChartSettings
    {
        public string CalculationMethod { get; set; }
        public string CalculatorType { get; set; }
        public string HousesDisplay { get; set; }
        public string UserTimeZoneId { get; set; }
        public DateTime? SelectedLocalDateTime { get; set; }
        public bool InvertDailyAndHourlyKiForSouthernHemisphere { get; set; }
        public bool InvertDailyAndHourlyCycleKiForSouthernHemisphere { get; set; }
    }

    public class MobileChartEnergy
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Element { get; set; }
        public string ElementDescription { get; set; }
        public string Modality { get; set; }
        public string ModalityDescription { get; set; }
        public string YinYang { get; set; }
        public string YinYangExpansionDescription { get; set; }
        public string Description { get; set; }
        public string Trigram { get; set; }
        public string TrigramDescription { get; set; }
        public string Direction { get; set; }
        public string FamilyMember { get; set; }
        public string Colour { get; set; }
        public string ImageUrl { get; set; }
    }

    public class MobileChartText
    {
        public string Body { get; set; }
        public string Summary { get; set; }
    }
}
