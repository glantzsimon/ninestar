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
        public Dictionary<string, string> RoleLabels { get; set; }
        public Dictionary<string, MobileHelpText> Help { get; set; }
        public Dictionary<string, MobileChartEnergy> Energies { get; set; }
        public Dictionary<string, MobileChartText> Sections { get; set; }
        public MobileChartHealth Health { get; set; }
        public string StrongYinOrganArtworkId { get; set; }
        public string StrongYangOrganArtworkId { get; set; }
        public string[] WeakYinOrganArtworkIds { get; set; }
        public string[] WeakYangOrganArtworkIds { get; set; }
        public string StrongYinOrgan { get; set; }
        public string StrongYangOrgan { get; set; }
        public string[] WeakYinOrgans { get; set; }
        public string[] WeakYangOrgans { get; set; }
    }

    public class MobileChartHealth
    {
        public MobileChartOrganPanel StrongOrgans { get; set; }
        public MobileChartOrganPanel WeakOrgans { get; set; }
    }

    public class MobileChartOrganPanel
    {
        public string Title { get; set; }
        public string HtmlBody { get; set; }
        public MobileChartOrgan[] YinOrgans { get; set; }
        public MobileChartOrgan[] YangOrgans { get; set; }
    }

    public class MobileChartOrgan
    {
        public string Value { get; set; }
        public string Label { get; set; }
        public string ArtworkId { get; set; }
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

    // Free header-table values only. Do not add interpretation/prose fields here.
    public class MobileChartEnergyOverview
    {
        public int Number { get; set; }
        public string Direction { get; set; }
        public string FamilyMember { get; set; }
        public string Colour { get; set; }
        public string Season { get; set; }
        public string InternalRepresentation { get; set; }
        public string Physiognomy { get; set; }
        public string PrimaryAttributes { get; set; }
        public string MostHarmoniousCombinations { get; set; }
        public string MostChallengingCombinations { get; set; }
    }

    public class MobileChartEnergy : MobileChartEnergyOverview
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Element { get; set; }
        public string ElementDescription { get; set; }
        public string Modality { get; set; }
        public string ModalityDescription { get; set; }
        public string YinYang { get; set; }
        public string YinYangExpansionTitle { get; set; }
        public string YinYangExpansionDescription { get; set; }
        public string Description { get; set; }
        public string Trigram { get; set; }
        public string TrigramDescription { get; set; }
        public string ArtworkId { get; set; }
        public string ElementArtworkId { get; set; }
        public string TrigramArtworkId { get; set; }
        public string ModalityArtworkId { get; set; }
        public string SeasonArtworkId { get; set; }
        public string YinYangArtworkId { get; set; }
        public string ImageUrl { get; set; }
    }

    public class MobileHelpText
    {
        public string Title { get; set; }
        public string HtmlBody { get; set; }
        public string Culture { get; set; }
    }

    public class MobileChartText
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Summary { get; set; }
    }
}
