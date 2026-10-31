using K9.Base.DataAccessLayer.Attributes;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels.MobileApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Dictionary = K9.Globalisation.Dictionary;

namespace K9.WebApplication.Helpers
{
    public static class MobilePersonalChartMapper
    {
        // Additive availability metadata for free/partial calculator projections.
        // Serialize through the API's camel-case contract before modifying the JSON.
        public static JObject WithFreeAvailability(object projection, bool isPartial, params string[] lockedFields)
        {
            var serializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var result = JObject.FromObject(projection, serializer);
            result["availability"] = new JObject
            {
                ["isPartial"] = isPartial,
                ["upgradeAvailable"] = true,
                ["lockedFields"] = new JArray(lockedFields ?? new string[0])
            };
            return result;
        }

        public static object FreeProfile(NineStarKiModel model) => new
        {
            model.Summary, model.Overview,
            RoleLabels = RoleLabels(),
            Help = MobileHelpMapper.Chart(model.PersonModel.IsSixteenOrOver(), includeMemberHelp: false),
            Core = model.MainEnergy.EnergyNumber, Character = model.CharacterEnergy.EnergyNumber,
            SocialExpression = model.SurfaceEnergy.EnergyNumber,
            Generation = model.PersonalChartEnergies.Generation.EnergyNumber,
            Epoch = model.PersonalChartEnergies.Epoch.EnergyNumber,
            DayStar = model.PersonalChartEnergies.Day.EnergyNumber,
            Hour = model.PersonalChartEnergies.Hour.EnergyNumber,
            Energies = new Dictionary<string, MobileChartEnergyOverview>
            {
                { "epoch", EnergyOverview(model.PersonalChartEnergies.Epoch) },
                { "generation", EnergyOverview(model.PersonalChartEnergies.Generation) },
                { "core", EnergyOverview(model.MainEnergy) },
                { "character", EnergyOverview(model.CharacterEnergy) },
                { "socialExpression", EnergyOverview(model.SurfaceEnergy) },
                { "dayStar", EnergyOverview(model.PersonalChartEnergies.Day) },
                { "hour", EnergyOverview(model.PersonalChartEnergies.Hour) }
            },
            Sections = new
            {
                Core = Text(model.MainEnergy.EnergyDescription, model.MainEnergy.MainEnergySummary),
                Character = Text(model.CharacterEnergy.EnergyDescription, model.CharacterEnergy.CharacterEnergySummary, title: model.CharacterEnergy.DescriptiveTitle),
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

        public static MobilePersonalChartResponse Map(NineStarKiModel model, bool birthTimeKnown, bool includeHelp = true)
        {
            if (model?.PersonModel == null || model.MainEnergy == null || model.CharacterEnergy == null)
                throw new ArgumentException("A calculated profile is required.", nameof(model));

            var main = model.MainEnergy;
            var character = model.CharacterEnergy;
            var person = model.PersonModel;
            var sections = new Dictionary<string, MobileChartText>
            {
                { "core", Text(main.EnergyDescription, main.MainEnergySummary) },
                { "character", Text(character.EnergyDescription, character.CharacterEnergySummary, title: character.DescriptiveTitle) },
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
                { "childNature", Text(character.ChildDescription, title: Dictionary.ChildNature) },
                { "adultChildRelationship", Text(model.AdultChildRelationsihpDescription, title: Dictionary.AdultChildRelationshipTitle) },
                { "stressResponse", Text(model.StressResponseDetails, title: Dictionary.StressResponse) },
                { "natalHouseStressResponse", Text(model.StressResponseFromNatalHouseDetails, title: Dictionary.StressResponse) },
                { "childStressResponse", Text(character.ChildStressDescription, title: Dictionary.StressResponse) }
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
                RoleLabels = RoleLabels(),
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
                Help = includeHelp ? MobileHelpMapper.Chart(person.IsSixteenOrOver(), includeMemberHelp: true) : null,
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
                Health = new MobileChartHealth
                {
                    StrongOrgans = new MobileChartOrganPanel
                    {
                        Title = K9.Globalisation.Dictionary.StrongOrgans,
                        HtmlBody = K9.Globalisation.Dictionary.StrongOrgansDescription,
                        YinOrgans = Organs(model.StrongYinOrgans.HasValue ? new[] { model.StrongYinOrgans.Value } : null),
                        YangOrgans = Organs(model.StrongYangOrgans.HasValue ? new[] { model.StrongYangOrgans.Value } : null)
                    },
                    WeakOrgans = new MobileChartOrganPanel
                    {
                        Title = K9.Globalisation.Dictionary.WeakOrgans,
                        HtmlBody = K9.Globalisation.Dictionary.WeakOrgansDescription,
                        YinOrgans = Organs(model.WeakYinOrgans),
                        YangOrgans = Organs(model.WeakYangOrgans)
                    }
                },
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

        public static Dictionary<string, string> RoleLabels() => new Dictionary<string, string>
        {
            { "core", Dictionary.MainEnergyLabel },
            { "character", Dictionary.CharacterEnergyLabel },
            { "socialExpression", Dictionary.SurfaceEnergyLabel },
            { "dayStar", Dictionary.DayStarLabel },
            { "generation", Dictionary.GenerationalEnergyLabel },
            { "epoch", Dictionary.EpochEnergyLabel }
        };

        private static MobileChartText Text(string body, string summary = null, string title = null) =>
            new MobileChartText { Title = title, Body = body, Summary = summary };

        private static MobileChartOrgan[] Organs(IEnumerable<EOrgan> organs) =>
            organs?.Select(organ => new MobileChartOrgan
            {
                Value = organ.ToString(),
                Label = organ.GetAttribute<EnumDescriptionAttribute>().GetDescription(),
                ArtworkId = OrganArtworkId(organ.ToString())
            }).ToArray() ?? new MobileChartOrgan[0];

        private static string OrganArtworkId(string organ) =>
            MobileArtworkRegistry.IdForPath("ninestar/organs/" + organ?.ToLowerInvariant() + ".png");

        public static MobileChartEnergyOverview EnergyOverview(NineStarKiEnergy energy) =>
            MapEnergyOverview<MobileChartEnergyOverview>(energy);

        private static T MapEnergyOverview<T>(NineStarKiEnergy energy) where T : MobileChartEnergyOverview, new()
        {
            if (energy == null) return null;
            var metadata = energy.MetaData;
            // Match every formatter in Views/PersonalChart/_ProfileHeaderInfo.
            // Season is its SeasonDescription value, not a cycle interpretation.
            return new T
            {
                Number = energy.EnergyNumber,
                ArtworkId = MobileArtworkRegistry.IdForPath("ninestar/energies/" + energy.EnergyLowerCase + ".png"),
                Colour = metadata.GetColour(),
                Direction = metadata.GetDirection(),
                FamilyMember = metadata.GetFamilyMember(),
                Season = energy.SeasonDescription,
                InternalRepresentation = metadata.GetInternalRepresentation(),
                Physiognomy = metadata.GetPhysiognomy(),
                PrimaryAttributes = metadata.GetPrimaryAttributes(),
                MostHarmoniousCombinations = metadata.GetMostHarmoniousCombinationsString(),
                MostChallengingCombinations = metadata.GetMostChallengingCombinationsString()
            };
        }

        public static MobileChartEnergy Energy(NineStarKiEnergy energy)
        {
            if (energy == null)
                return null;

            var result = MapEnergyOverview<MobileChartEnergy>(energy);
            result.Name = energy.EnergyName;
            result.Title = energy.EnergyTitle;
            result.Element = energy.ElementName;
            result.ElementDescription = energy.ElementDescription;
            result.Modality = energy.ModalityName;
            result.ModalityDescription = energy.ModalityDescription;
            result.YinYang = energy.YinYangName;
            result.YinYangExpansionTitle = energy.YinYangExpansionTitle;
            result.YinYangExpansionDescription = energy.YinYangExpansionDescription;
            result.Description = energy.EnergyDescription;
            result.Trigram = energy.Trigram;
            result.TrigramDescription = energy.TrigramDescription;
            result.ElementArtworkId = MobileArtworkRegistry.IdForPath("ninestar/elements/" + energy.Element.ToString().ToLowerInvariant() + ".png");
            result.TrigramArtworkId = MobileArtworkRegistry.IdForPath("ninestar/trigrams/trigram-" + energy.EnergyLowerCase + "-square.png");
            result.ModalityArtworkId = MobileArtworkRegistry.IdForPath("ninestar/modalities/" + energy.Modality.ToString().ToLowerInvariant() + ".png");
            result.SeasonArtworkId = MobileArtworkRegistry.IdForPath("ninestar/seasons/" + energy.MetaData.Cycle.ToString().ToLowerInvariant() + ".png");
            result.YinYangArtworkId = MobileArtworkRegistry.IdForPath("ninestar/yinyang/" + energy.YinYang.ToString().ToLowerInvariant() + ".png");
            result.ImageUrl = MediaService.BaseImagesPath + "/ninestar/energies/" + energy.EnergyLowerCase + ".png";
            return result;
        }
    }
}
