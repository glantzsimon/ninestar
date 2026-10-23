using K9.WebApplication.Models;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels.MobileApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace K9.WebApplication.Helpers
{
    public static class MobilePersonalChartMapper
    {
        public static object FreeProfile(NineStarKiModel model) => new
        {
            model.Summary, model.Overview,
            Core = model.MainEnergy.EnergyNumber, Character = model.CharacterEnergy.EnergyNumber,
            SocialExpression = model.SurfaceEnergy.EnergyNumber,
            Generation = model.PersonalChartEnergies.Generation.EnergyNumber,
            Epoch = model.PersonalChartEnergies.Epoch.EnergyNumber,
            DayStar = model.PersonalChartEnergies.Day.EnergyNumber,
            Hour = model.PersonalChartEnergies.Hour.EnergyNumber,
            Sections = new
            {
                Core = Text(model.MainEnergy.EnergyDescription, model.MainEnergy.MainEnergySummary),
                Character = Text(model.CharacterEnergy.EnergyDescription, model.CharacterEnergy.CharacterEnergySummary),
                IntellectualQualities = Text(model.MainEnergy.IntellectualQualities, model.MainEnergy.IntellectualQualitiesSummary)
            }
        };

        public static object Numbers(NineStarKiModel model) => new
        {
            model.PersonModel.Name, model.Summary,
            Core = model.MainEnergy.EnergyNumber, Character = model.CharacterEnergy.EnergyNumber,
            SocialExpression = model.SurfaceEnergy.EnergyNumber,
            Generation = model.PersonalChartEnergies.Generation.EnergyNumber,
            Epoch = model.PersonalChartEnergies.Epoch.EnergyNumber,
            DayStar = model.PersonalChartEnergies.Day.EnergyNumber,
            Hour = model.PersonalChartEnergies.Hour.EnergyNumber
        };

        public static MobilePersonalChartResponse Map(NineStarKiModel model, bool birthTimeKnown)
        {
            if (model?.PersonModel == null || model.MainEnergy == null || model.CharacterEnergy == null)
                throw new ArgumentException("A calculated profile is required.", nameof(model));

            var main = model.MainEnergy;
            var character = model.CharacterEnergy;
            var person = model.PersonModel;
            var sections = new Dictionary<string, MobileChartText>
            {
                { "core", Text(main.EnergyDescription, main.MainEnergySummary) },
                { "character", Text(character.EnergyDescription, character.CharacterEnergySummary) },
                { "socialExpression", Text(model.SurfaceEnergy?.EnergyDescription, model.SurfaceEnergy?.SurfaceEnergySummary) },
                { "intellectualQualities", Text(main.IntellectualQualities, main.IntellectualQualitiesSummary) },
                { "interpersonalQualities", Text(main.InterpersonalQualities, main.InterpersonalQualitiesSummary) },
                { "emotionalLandscape", Text(main.EmotionalLandscape, main.EmotionalLandscapeSummary) },
                { "career", Text(model.Career, main.CareerSummary) },
                { "occupations", Text(main.Occupations) },
                { "finances", Text(main.Finances, main.FinancesSummary) },
                { "health", Text(model.Health, main.HealthSummary) },
                { "illnesses", Text(main.Illnesses) },
                { "spirituality", Text(main.Spirituality, main.SpiritualitySummary) },
                { "childNature", Text(character.ChildDescription) },
                { "adultChildRelationship", Text(model.AdultChildRelationsihpDescription) },
                { "stressResponse", Text(model.StressResponseDetails) },
                { "natalHouseStressResponse", Text(model.StressResponseFromNatalHouseDetails) },
                { "childStressResponse", Text(character.ChildStressDescription) }
            };
            // Match the relationship tab's age gate in _ProfileMainEnergyDetails.
            if (person.IsSixteenOrOver())
            {
                sections.Add("relationships", Text(model.MainEnergyRelationshipsDetails, model.MainEnergyRelationshipsSummary));
                sections.Add("relationshipsStraight", Text(model.SexualityRelationTypeDetailsStraight, model.SexualityRelationTypeSummaryStraight));
                sections.Add("relationshipsLgbtq", Text(model.SexualityRelationTypeDetailsGay, model.SexualityRelationTypeSummaryGay));
            }

            return new MobilePersonalChartResponse
            {
                Culture = (K9.Globalisation.Dictionary.Culture ?? CultureInfo.CurrentUICulture).Name,
                Person = new MobileChartPerson
                {
                    Name = person.Name,
                    DateOfBirth = person.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    TimeOfBirth = person.TimeOfBirth.ToString("c", CultureInfo.InvariantCulture),
                    TimeOfBirthKnown = birthTimeKnown,
                    BirthTimeZoneId = person.BirthTimeZoneId,
                    Gender = person.Gender.ToString(),
                    IsAdult = person.IsAdult(),
                    IsSixteenOrOver = person.IsSixteenOrOver()
                },
                Settings = new MobileChartSettings
                {
                    CalculationMethod = model.CalculationMethod.ToString(),
                    CalculatorType = model.CalculatorType.ToString(),
                    HousesDisplay = model.HousesDisplay.ToString(),
                    UserTimeZoneId = model.UserTimeZoneId,
                    SelectedLocalDateTime = model.SelectedDate,
                    InvertDailyAndHourlyKiForSouthernHemisphere = model.InvertDailyAndHourlyKiForSouthernHemisphere,
                    InvertDailyAndHourlyCycleKiForSouthernHemisphere = model.InvertDailyAndHourlyCycleKiForSouthernHemisphere
                },
                Summary = model.Summary,
                Overview = model.Overview,
                Energies = new Dictionary<string, MobileChartEnergy>
                {
                    { "epoch", Energy(model.PersonalChartEnergies.Epoch) },
                    { "generation", Energy(model.PersonalChartEnergies.Generation) },
                    { "core", Energy(main) },
                    { "character", Energy(character) },
                    { "socialExpression", Energy(model.SurfaceEnergy) },
                    { "dayStar", Energy(model.PersonalChartEnergies.Day) },
                    { "hour", Energy(model.PersonalChartEnergies.Hour) }
                },
                Sections = sections,
                StrongYinOrganArtworkId = OrganArtworkId(model.StrongYinOrgans?.ToString()),
                StrongYangOrganArtworkId = OrganArtworkId(model.StrongYangOrgans?.ToString()),
                WeakYinOrganArtworkIds = model.WeakYinOrgans?.Select(e => OrganArtworkId(e.ToString())).ToArray() ?? new string[0],
                WeakYangOrganArtworkIds = model.WeakYangOrgans?.Select(e => OrganArtworkId(e.ToString())).ToArray() ?? new string[0],
                StrongYinOrgan = model.StrongYinOrgans?.ToString(),
                StrongYangOrgan = model.StrongYangOrgans?.ToString(),
                WeakYinOrgans = model.WeakYinOrgans?.Select(e => e.ToString()).ToArray() ?? new string[0],
                WeakYangOrgans = model.WeakYangOrgans?.Select(e => e.ToString()).ToArray() ?? new string[0]
            };
        }

        private static MobileChartText Text(string body, string summary = null) =>
            new MobileChartText { Body = body, Summary = summary };

        private static string OrganArtworkId(string organ) =>
            MobileArtworkRegistry.IdForPath("ninestar/organs/" + organ?.ToLowerInvariant() + ".png");

        public static MobileChartEnergy Energy(NineStarKiEnergy energy)
        {
            if (energy == null)
                return null;

            return new MobileChartEnergy
            {
                Number = energy.EnergyNumber,
                Name = energy.EnergyName,
                Title = energy.EnergyTitle,
                Element = energy.ElementName,
                ElementDescription = energy.ElementDescription,
                Modality = energy.ModalityName,
                ModalityDescription = energy.ModalityDescription,
                YinYang = energy.YinYangName,
                YinYangExpansionDescription = energy.YinYangExpansionDescription,
                Description = energy.EnergyDescription,
                Trigram = energy.Trigram,
                TrigramDescription = energy.TrigramDescription,
                Direction = energy.DirectionName,
                FamilyMember = energy.FamilyMember,
                Colour = energy.Colour,
                ArtworkId = MobileArtworkRegistry.IdForPath("ninestar/energies/" + energy.EnergyLowerCase + ".png"),
                ElementArtworkId = MobileArtworkRegistry.IdForPath("ninestar/elements/" + energy.Element.ToString().ToLowerInvariant() + ".png"),
                TrigramArtworkId = MobileArtworkRegistry.IdForPath("ninestar/trigrams/trigram-" + energy.EnergyLowerCase + "-square.png"),
                ModalityArtworkId = MobileArtworkRegistry.IdForPath("ninestar/modalities/" + energy.Modality.ToString().ToLowerInvariant() + ".png"),
                SeasonArtworkId = MobileArtworkRegistry.IdForPath("ninestar/seasons/" + energy.MetaData.Cycle.ToString().ToLowerInvariant() + ".png"),
                YinYangArtworkId = MobileArtworkRegistry.IdForPath("ninestar/yinyang/" + energy.YinYang.ToString().ToLowerInvariant() + ".png"),
                ImageUrl = MediaService.BaseImagesPath + "/ninestar/energies/" + energy.EnergyLowerCase + ".png"
            };
        }
    }
}
