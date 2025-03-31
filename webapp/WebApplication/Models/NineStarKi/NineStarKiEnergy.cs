using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Enums;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Attributes;
using K9.WebApplication.Enums;
using K9.WebApplication.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;

namespace K9.WebApplication.Models
{
    public enum ENineStarKiColour
    {
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.White)]
        White,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Black)]
        Black,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.BrightGreen)]
        BrightGreen,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Green)]
        Green,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Yellow)]
        Yellow,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Red)]
        Red,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Purple)]
        Purple
    }

    public enum ENineStarKiFamilyMember
    {
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.MiddleSon)]
        MiddleSon,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Mother)]
        Mother,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.EldestSon)]
        EldestSon,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.EldestDaughter)]
        EldestDaughter,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.SeventhChild)]
        SeventhChild,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Father)]
        Father,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.YoungestDaughter)]
        YoungestDaughter,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.YoungestSon)]
        YoungestSon,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.MiddleDaughter)]
        MiddleDaughter
    }

    public enum ENineStarKiDescriptiveName
    {
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Diplomat)]
        Diplomat,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Nurturer)]
        Nurturer,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Pioneer)]
        Pioneer,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Influencer)]
        Influencer,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Hub)]
        Hub,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Leader)]
        Leader,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Advisor)]
        Advisor,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Pragmatist)]
        Pragmatist,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Communicator)]
        Communicator
    }

    public enum ENineStarKiCycleDescriptiveName
    {
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Hibernation)]
        Hibernation,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Planning)]
        Planning,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Sprouting)]
        Sprouting,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Blossoming)]
        Blossoming,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Consolidating)]
        Consolidating,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Ripening)]
        Ripening,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Harvest)]
        Harvest,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Revolution)]
        Revolution,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Spotlight)]
        Spotlight
    }

    public enum ENineStarKiCycle
    {
        Unspecified,
        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.Winter,
            Element = ENineStarKiElement.Water,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Hibernation,
            YearlyDescriptionName = Strings.Names.WaterYear,
            MonthlyDescriptionName = Strings.Names.WaterMonth,
            EightyOneYearDescriptionName = Strings.Names.WaterEpoch,
            NineYearDescriptionName = Strings.Names.WaterGeneration,
            DailyDescriptionName = Strings.Names.WaterDayCycle)]
        Winter,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.WinterToSpring,
            Element = ENineStarKiElement.Earth,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Planning,
            YearlyDescriptionName = Strings.Names.SoilYear,
            MonthlyDescriptionName = Strings.Names.SoilMonth,
            EightyOneYearDescriptionName = Strings.Names.SoilEpoch,
            NineYearDescriptionName = Strings.Names.SoilGeneration,
            DailyDescriptionName = Strings.Names.SoilDayCycle)]
        WinterToSpring,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.EarlySpring,
            Element = ENineStarKiElement.Tree,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Sprouting,
            YearlyDescriptionName = Strings.Names.ThunderYear,
            MonthlyDescriptionName = Strings.Names.ThunderMonth,
            EightyOneYearDescriptionName = Strings.Names.ThunderEpoch,
            NineYearDescriptionName = Strings.Names.ThunderGeneration,
            DailyDescriptionName = Strings.Names.ThunderDayCycle)]
        EarlySpring,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.LateSpring,
            Element = ENineStarKiElement.Tree,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Blossoming,
            YearlyDescriptionName = Strings.Names.WindYear,
            MonthlyDescriptionName = Strings.Names.WindMonth,
            EightyOneYearDescriptionName = Strings.Names.WindEpoch,
            NineYearDescriptionName = Strings.Names.WindGeneration,
            DailyDescriptionName = Strings.Names.WindDayCycle)]
        LateSpring,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.Centre,
            Element = ENineStarKiElement.Earth,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Consolidating,
            YearlyDescriptionName = Strings.Names.CoreEarthYear,
            MonthlyDescriptionName = Strings.Names.CoreEarthMonth,
            EightyOneYearDescriptionName = Strings.Names.CoreEarthEpoch,
            NineYearDescriptionName = Strings.Names.CoreEarthGeneration,
            DailyDescriptionName = Strings.Names.CoreEarthDayCycle)]
        Centre,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.EarlyAutumn,
            Element = ENineStarKiElement.Metal,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Ripening,
            YearlyDescriptionName = Strings.Names.HeavenYear,
            MonthlyDescriptionName = Strings.Names.HeavenMonth,
            EightyOneYearDescriptionName = Strings.Names.HeavenEpoch,
            NineYearDescriptionName = Strings.Names.HeavenGeneration,
            DailyDescriptionName = Strings.Names.HeavenDayCycle)]
        EarlyAutumn,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.LateAutumn,
            Element = ENineStarKiElement.Metal,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Harvest,
            YearlyDescriptionName = Strings.Names.LakeYear,
            MonthlyDescriptionName = Strings.Names.LakeMonth,
            EightyOneYearDescriptionName = Strings.Names.LakeEpoch,
            NineYearDescriptionName = Strings.Names.LakeGeneration,
            DailyDescriptionName = Strings.Names.LakeDayCycle)]
        LateAutumn,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.AutumnToWinter,
            Element = ENineStarKiElement.Earth,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Revolution,
            YearlyDescriptionName = Strings.Names.MountainYear,
            MonthlyDescriptionName = Strings.Names.MountainMonth,
            EightyOneYearDescriptionName = Strings.Names.MountainEpoch,
            NineYearDescriptionName = Strings.Names.MountainGeneration,
            DailyDescriptionName = Strings.Names.MountainDayCycle)]
        AutumnToWinter,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.Summer,
            Element = ENineStarKiElement.Fire,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Spotlight,
            YearlyDescriptionName = Strings.Names.FireYear,
            MonthlyDescriptionName = Strings.Names.FireMonth,
            EightyOneYearDescriptionName = Strings.Names.WindEpoch,
            NineYearDescriptionName = Strings.Names.FireGeneration,
            DailyDescriptionName = Strings.Names.FireDayCycle)]
        Summer
    }

    public enum ENineStarKiDirection
    {
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.North)]
        North = 1,

        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.SouthWest)]
        SouthWest,

        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.East)]
        East,

        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.SouthEast)]
        SouthEast,

        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Unspecified)]
        Centre,

        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.NorthWest)]
        NorthWest,

        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.West)]
        West,

        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.NorthEast)]
        NorthEast,

        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.South)]
        South
    }

    public enum ENineStarKiYinYang
    {
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Unspecified)]
        Unspecified,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Yin)]
        Yin,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Yang)]
        Yang
    }

    public enum ENineStarKiModality
    {
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Unspecified)]
        Unspecified,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Reflective)]
        Reflective,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Stable)]
        Stable,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Dynamic)]
        Dynamic
    }

    public enum ENineStarKiEnergyType
    {
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.MainEnergy)]
        MainEnergy,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.CharacterEnergy)]
        CharacterEnergy,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.SurfaceEnergy)]
        SurfaceEnergy,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.DailyEnergy)]
        DailyEnergy,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.HourlyEnergy)]
        HourlyEnergy,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.EpochEnergyLabel)]
        EpochEnergy,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.GenerationalEnergyLabel)]
        GenerationalEnergy,
    }

    public enum ENineStarKiEnergyCycleType
    {
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Unspecified)]
        Unspecified,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.EpochCycleEnergy)]
        EpochEnergy,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.GenerationalCycleEnergy)]
        GenerationalEnergy,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.YearlyCycleEnergy)]
        YearlyCycleEnergy,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.MonthlyCycleEnergy)]
        MonthlyCycleEnergy,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.DailyEnergy)]
        DailyEnergy,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.HourlyEnergy)]
        HourlyEnergy,
    }

    public enum ENineStarKiEnergy
    {
        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Unspecified)]
        Unspecified,
        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Water, Colour = ENineStarKiColour.White, Element = ENineStarKiElement.Water, Direction = ENineStarKiDirection.North, FamilyMember = ENineStarKiFamilyMember.MiddleSon, YinYang = ENineStarKiYinYang.Yang, TrigramName = "Kan", DescriptiveName = ENineStarKiDescriptiveName.Diplomat, Modality = ENineStarKiModality.Reflective, Cycle = ENineStarKiCycle.Winter,
            InternalRepresentation = new[] { EBodyPart.Kidneys, EBodyPart.Bladder, EBodyPart.Bones, EBodyPart.NervousSystem, EBodyPart.SexOrgans },
            Physiognomy = new[] { EBodyPart.Ear },
            PrimaryAttributes = new[] { EPrimaryAttribute.Hardship, EPrimaryAttribute.Danger })]
        Water,

        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Soil, Colour = ENineStarKiColour.Black, Element = ENineStarKiElement.Earth, Direction = ENineStarKiDirection.SouthWest, FamilyMember = ENineStarKiFamilyMember.Mother, YinYang = ENineStarKiYinYang.Yin, TrigramName = "Kun", DescriptiveName = ENineStarKiDescriptiveName.Nurturer, Modality = ENineStarKiModality.Stable, Cycle = ENineStarKiCycle.WinterToSpring,
            InternalRepresentation = new[] { EBodyPart.Stomach, EBodyPart.SpleenPancreas, EBodyPart.LowerBody, EBodyPart.SoftTissue },
            Physiognomy = new[] { EBodyPart.Abdomen },
            PrimaryAttributes = new[] { EPrimaryAttribute.Receptive })]
        Soil,

        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Thunder, Colour = ENineStarKiColour.BrightGreen, Element = ENineStarKiElement.Tree, Direction = ENineStarKiDirection.East, FamilyMember = ENineStarKiFamilyMember.EldestSon, YinYang = ENineStarKiYinYang.Yang, TrigramName = "Chen", DescriptiveName = ENineStarKiDescriptiveName.Pioneer, Modality = ENineStarKiModality.Dynamic, Cycle = ENineStarKiCycle.EarlySpring)]
        Thunder,

        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Wind, Colour = ENineStarKiColour.Green, Element = ENineStarKiElement.Tree, Direction = ENineStarKiDirection.SouthEast, FamilyMember = ENineStarKiFamilyMember.EldestDaughter, YinYang = ENineStarKiYinYang.Yin, TrigramName = "Sun", DescriptiveName = ENineStarKiDescriptiveName.Influencer, Modality = ENineStarKiModality.Reflective, Cycle = ENineStarKiCycle.LateSpring)]
        Wind,

        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.CoreEarth, Colour = ENineStarKiColour.Yellow, Element = ENineStarKiElement.Earth, Direction = ENineStarKiDirection.Centre, FamilyMember = ENineStarKiFamilyMember.SeventhChild, YinYang = ENineStarKiYinYang.Unspecified, TrigramName = "None", DescriptiveName = ENineStarKiDescriptiveName.Hub, Modality = ENineStarKiModality.Stable, Cycle = ENineStarKiCycle.Centre)]
        CoreEarth,

        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Heaven, Colour = ENineStarKiColour.White, Element = ENineStarKiElement.Metal, Direction = ENineStarKiDirection.NorthWest, FamilyMember = ENineStarKiFamilyMember.Father, YinYang = ENineStarKiYinYang.Yang, TrigramName = "Chien", DescriptiveName = ENineStarKiDescriptiveName.Leader, Modality = ENineStarKiModality.Dynamic, Cycle = ENineStarKiCycle.EarlyAutumn)]
        Heaven,

        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Lake, Colour = ENineStarKiColour.Red, Element = ENineStarKiElement.Metal, Direction = ENineStarKiDirection.West, FamilyMember = ENineStarKiFamilyMember.YoungestDaughter, YinYang = ENineStarKiYinYang.Yin, TrigramName = "Tui", DescriptiveName = ENineStarKiDescriptiveName.Advisor, Modality = ENineStarKiModality.Reflective, Cycle = ENineStarKiCycle.LateAutumn)]
        Lake,

        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Mountain, Colour = ENineStarKiColour.White, Element = ENineStarKiElement.Earth, Direction = ENineStarKiDirection.NorthEast, FamilyMember = ENineStarKiFamilyMember.YoungestSon, YinYang = ENineStarKiYinYang.Yang, TrigramName = "Ken", DescriptiveName = ENineStarKiDescriptiveName.Pragmatist, Modality = ENineStarKiModality.Stable, Cycle = ENineStarKiCycle.AutumnToWinter)]
        Mountain,

        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Fire, Colour = ENineStarKiColour.Purple, Element = ENineStarKiElement.Fire, Direction = ENineStarKiDirection.South, FamilyMember = ENineStarKiFamilyMember.MiddleDaughter, YinYang = ENineStarKiYinYang.Yin, TrigramName = "Li", DescriptiveName = ENineStarKiDescriptiveName.Communicator, Modality = ENineStarKiModality.Dynamic, Cycle = ENineStarKiCycle.Summer)]
        Fire
    }

    public class NineStarKiEnergy
    {
        // Dictionaries for fast lookups instead of switch statements
        private static readonly Dictionary<ENineStarKiEnergy, string> _trigramDescriptions =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_trigram},
                {ENineStarKiEnergy.Wind, Dictionary.wind_trigram},
                {ENineStarKiEnergy.Lake, Dictionary.lake_trigram},
                {ENineStarKiEnergy.Soil, Dictionary.soil_trigram},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_trigram},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_trigram},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_trigram},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_trigram},
                {ENineStarKiEnergy.Fire, Dictionary.fire_trigram}
            };

        #region Personal Chart

        private static readonly Dictionary<ENineStarKiEnergy, string> _epochDescriptions =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_epoch},
                {ENineStarKiEnergy.Soil, Dictionary.soil_epoch},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_epoch},
                {ENineStarKiEnergy.Wind, Dictionary.wind_epoch},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_epoch},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_epoch},
                {ENineStarKiEnergy.Lake, Dictionary.lake_epoch},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_epoch},
                {ENineStarKiEnergy.Fire, Dictionary.fire_epoch}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _generationDescriptions =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_generation},
                {ENineStarKiEnergy.Soil, Dictionary.soil_generation},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_generation},
                {ENineStarKiEnergy.Wind, Dictionary.wind_generation},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_generation},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_generation},
                {ENineStarKiEnergy.Lake, Dictionary.lake_generation},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_generation},
                {ENineStarKiEnergy.Fire, Dictionary.fire_generation}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _mainEnergyDescriptions =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_description},
                {ENineStarKiEnergy.Soil, Dictionary.soil_description},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_description},
                {ENineStarKiEnergy.Wind, Dictionary.wind_description},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_description},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_description},
                {ENineStarKiEnergy.Lake, Dictionary.lake_description},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_description},
                {ENineStarKiEnergy.Fire, Dictionary.fire_description}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _emotionalDescriptions =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_emotional_description},
                {ENineStarKiEnergy.Soil, Dictionary.soil_emotional_description},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_emotional_description},
                {ENineStarKiEnergy.Wind, Dictionary.wind_emotional_description},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_emotional_description},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_emotional_description},
                {ENineStarKiEnergy.Lake, Dictionary.lake_emotional_description},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_emotional_description},
                {ENineStarKiEnergy.Fire, Dictionary.fire_emotional_description}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _surfaceEnergyDescriptions =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_surface_description},
                {ENineStarKiEnergy.Soil, Dictionary.soil_surface_description},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_surface_description},
                {ENineStarKiEnergy.Wind, Dictionary.wind_surface_description},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_surface_description},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_surface_description},
                {ENineStarKiEnergy.Lake, Dictionary.lake_surface_description},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_surface_description},
                {ENineStarKiEnergy.Fire, Dictionary.fire_surface_description}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _childDescriptions =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_child},
                {ENineStarKiEnergy.Soil, Dictionary.soil_child},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_child},
                {ENineStarKiEnergy.Wind, Dictionary.wind_child},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_child},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_child},
                {ENineStarKiEnergy.Lake, Dictionary.lake_child},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_child},
                {ENineStarKiEnergy.Fire, Dictionary.fire_child}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _dayStarDescriptions =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_daystar},
                {ENineStarKiEnergy.Soil, Dictionary.soil_daystar},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_daystar},
                {ENineStarKiEnergy.Wind, Dictionary.wind_daystar},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_daystar},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_daystar},
                {ENineStarKiEnergy.Lake, Dictionary.lake_daystar},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_daystar},
                {ENineStarKiEnergy.Fire, Dictionary.fire_daystar}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _overviews
            = new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_overview},
                {ENineStarKiEnergy.Soil, Dictionary.soil_overview},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_overview},
                {ENineStarKiEnergy.Wind, Dictionary.wind_overview},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_overview},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_overview},
                {ENineStarKiEnergy.Lake, Dictionary.lake_overview},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_overview},
                {ENineStarKiEnergy.Fire, Dictionary.fire_overview}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _healthDetails
            = new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_health},
                {ENineStarKiEnergy.Soil, Dictionary.soil_health},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_health},
                {ENineStarKiEnergy.Wind, Dictionary.wind_health},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_health},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_health},
                {ENineStarKiEnergy.Lake, Dictionary.lake_health},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_health},
                {ENineStarKiEnergy.Fire, Dictionary.fire_health}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _careerDetails
            = new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_career},
                {ENineStarKiEnergy.Soil, Dictionary.soil_career},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_career},
                {ENineStarKiEnergy.Wind, Dictionary.wind_career},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_career},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_career},
                {ENineStarKiEnergy.Lake, Dictionary.lake_career},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_career},
                {ENineStarKiEnergy.Fire, Dictionary.fire_career}
            };

        #endregion

        #region Predictions

        private static readonly Dictionary<ENineStarKiEnergy, string> _epochCycleDescriptions =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_epoch_cycle},
                {ENineStarKiEnergy.Soil, Dictionary.soil_epoch_cycle},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_epoch_cycle},
                {ENineStarKiEnergy.Wind, Dictionary.wind_epoch_cycle},
                {ENineStarKiEnergy.CoreEarth, Dictionary.core_earth_epoch_cycle},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_epoch_cycle},
                {ENineStarKiEnergy.Lake, Dictionary.lake_epoch_cycle},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_epoch_cycle},
                {ENineStarKiEnergy.Fire, Dictionary.fire_epoch_cycle}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _generationCycleDescriptions =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_generation_cycle},
                {ENineStarKiEnergy.Soil, Dictionary.soil_generation_cycle},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_generation_cycle},
                {ENineStarKiEnergy.Wind, Dictionary.wind_generation_cycle},
                {ENineStarKiEnergy.CoreEarth, Dictionary.core_earth_generation_cycle},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_generation_cycle},
                {ENineStarKiEnergy.Lake, Dictionary.lake_generation_cycle},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_generation_cycle},
                {ENineStarKiEnergy.Fire, Dictionary.fire_generation_cycle}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _yearlyCycleDescriptions =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_year},
                {ENineStarKiEnergy.Soil, Dictionary.soil_year},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_year},
                {ENineStarKiEnergy.Wind, Dictionary.wind_year},
                {ENineStarKiEnergy.CoreEarth, Dictionary.core_earth_year},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_year},
                {ENineStarKiEnergy.Lake, Dictionary.lake_year},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_year},
                {ENineStarKiEnergy.Fire, Dictionary.fire_year}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _monthlyCycleDescriptions =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_month},
                {ENineStarKiEnergy.Soil, Dictionary.soil_month},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_month},
                {ENineStarKiEnergy.Wind, Dictionary.wind_month},
                {ENineStarKiEnergy.CoreEarth, Dictionary.core_earth_month},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_month},
                {ENineStarKiEnergy.Lake, Dictionary.lake_month},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_month},
                {ENineStarKiEnergy.Fire, Dictionary.fire_month}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _daycycleCycleDescriptions =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_day_cycle},
                {ENineStarKiEnergy.Soil, Dictionary.soil_day_cycle},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_day_cycle},
                {ENineStarKiEnergy.Wind, Dictionary.wind_day_cycle},
                {ENineStarKiEnergy.CoreEarth, Dictionary.core_earth_day_cycle},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_day_cycle},
                {ENineStarKiEnergy.Lake, Dictionary.lake_day_cycle},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_day_cycle},
                {ENineStarKiEnergy.Fire, Dictionary.fire_day_cycle}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _hourCycleDescriptions =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_hour_cycle},
                {ENineStarKiEnergy.Soil, Dictionary.soil_hour_cycle},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_hour_cycle},
                {ENineStarKiEnergy.Wind, Dictionary.wind_hour_cycle},
                {ENineStarKiEnergy.CoreEarth, Dictionary.core_earth_hour_cycle},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_hour_cycle},
                {ENineStarKiEnergy.Lake, Dictionary.lake_hour_cycle},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_hour_cycle},
                {ENineStarKiEnergy.Fire, Dictionary.fire_hour_cycle}
            };

        #endregion

        public NineStarKiEnergy(ENineStarKiEnergy energy, ENineStarKiEnergyType type, bool isAdult = true)
        {
            Energy = energy;
            EnergyType = type;
            IsAdult = isAdult;
        }

        public NineStarKiEnergy(ENineStarKiEnergy energy,
            ENineStarKiEnergyCycleType energyCycleType = ENineStarKiEnergyCycleType.Unspecified)
        {
            Energy = energy;
            EnergyCycleType = energyCycleType;
        }

        public NineStarKiEnergy GetHouseOfFive() => new NineStarKiEnergy((ENineStarKiEnergy)NineStarKiModel.GetNineStarKiNumber(5 + (5 - EnergyNumber)));

        #region Personal Chart

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EpochEnergyLabel)]
        public string EpochDescription =>
            EnergyType == ENineStarKiEnergyType.EpochEnergy ? GetEpochDescription() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.GenerationalEnergyLabel)]
        public string GenerationDescription => EnergyType == ENineStarKiEnergyType.GenerationalEnergy
            ? GetGenerationDescription()
            : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.MainEnergyLabel)]
        public string MainEnergyDescription =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetMainEnergyDescription() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.CharacterEnergyLabel)]
        public string EmotionalEnergyDescription => EnergyType == ENineStarKiEnergyType.CharacterEnergy
            ? GetEmotionalEnergyDescription()
            : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SurfaceEnergyLabel)]
        public string SurfaceEnergyDescription => EnergyType == ENineStarKiEnergyType.SurfaceEnergy
            ? GetSurfaceEnergyDescription()
            : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ChildLabel)]
        public string ChildDescription =>
            EnergyType == ENineStarKiEnergyType.CharacterEnergy ? GetChildDescription() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DayStarLabel)]
        public string DayStarDescription =>
            EnergyType == ENineStarKiEnergyType.DailyEnergy ? GetDayStarDescription() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DayStarLabel)]
        public string OverviewDescription =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetOverview() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DayStarLabel)]
        public string HealthDescription =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetHealthDescription() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DayStarLabel)]
        public string CareerDescription =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetCareerDescription() : string.Empty;

        #endregion

        #region Cycles

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Names.EpochCycleEnergy)]
        public string EpochCycleDescription => EnergyCycleType == ENineStarKiEnergyCycleType.EpochEnergy
            ? GetEpochCycleDescription()
            : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Names.GenerationalCycleEnergy)]
        public string GenerationCycleDescription => EnergyCycleType == ENineStarKiEnergyCycleType.GenerationalEnergy
            ? GetGenerationCycleDescription()
            : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Names.YearlyCycleEnergy)]
        public string YearlyCycleDescription => EnergyCycleType == ENineStarKiEnergyCycleType.YearlyCycleEnergy
            ? GetYearlyCycleDescription()
            : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Names.MonthlyCycleEnergy)]
        public string MonthlyCycleDescription => EnergyCycleType == ENineStarKiEnergyCycleType.MonthlyCycleEnergy
            ? GetMonthlyCycleDescription()
            : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Names.DailyCycleEnergy)]
        public string DailyCycleDescription => EnergyCycleType == ENineStarKiEnergyCycleType.DailyEnergy
            ? GetDailyCycleDescription()
            : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Names.HourlyEnergy)]
        public string HourlyCycleDescription => EnergyCycleType == ENineStarKiEnergyCycleType.HourlyEnergy
            ? GetHourlyCycleDescription()
            : string.Empty;

        #endregion

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EnergyLabel)]
        public string EnergyName => MetaData.GetDescription();

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EnergyLabel)]
        public string EnergyUIName => EnergyName.RemoveSpaces().ToLower();

        public int EnergyNumber => (int)Energy;

        public int[] GetMagicSquareNumbers() => new[] {
            NineStarKiModel.GetNineStarKiNumber(EnergyNumber - 4),
            NineStarKiModel.GetNineStarKiNumber(EnergyNumber - 3),
            NineStarKiModel.GetNineStarKiNumber(EnergyNumber - 2),
            NineStarKiModel.GetNineStarKiNumber(EnergyNumber - 1),
            EnergyNumber,
            NineStarKiModel.GetNineStarKiNumber(EnergyNumber + 1),
            NineStarKiModel.GetNineStarKiNumber(EnergyNumber + 2),
            NineStarKiModel.GetNineStarKiNumber(EnergyNumber + 3),
            NineStarKiModel.GetNineStarKiNumber(EnergyNumber + 4)
        };

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EnergyDescriptionLabel)]
        public string EnergyDescription => GetEnergyDescription();

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ElementLabel)]
        public string ElementName => MetaData.GetElement();

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ElementLabel)]
        public string ElementDescription => MetaData.GetElementDescription();

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.FamilyMemberLabel)]
        public string FamilyMember => MetaData.GetFamilyMember();

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ModalityLabel)]
        public string ModalityName => Modality.ToString();

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ElementLabel)]
        public string ElementTitle => $"{ElementName} {Dictionary.Element}";

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ModalityLabel)]
        public string ModalityTitle => $"{ModalityName} {Dictionary.ModalityLabel}";

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ModalityDescriptionLabel)]
        public string ModalityDescription => MetaData.ModalityDescription;

        public ENineStarKiDirection Direction => MetaData.Direction;
        public string DirectionName => MetaData.GetDirection();

        public string Season => CycleMetaData.Season;
        public string SeasonDescription => CycleMetaData.SeasonDescription;

        public string CycleDescription => GetCycleDescription();

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.TrigramLabel)]
        public string Trigram => MetaData.GetTrigram();

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.TrigramLabel)]
        public string TrigramTitle => $"{Dictionary.Trigram} '{Trigram}'";

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.TrigramLabel)]
        public string TrigramDescription => _trigramDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        public string YinYangName => YinYang == ENineStarKiYinYang.Unspecified ? string.Empty : YinYang.ToString();

        [ScriptIgnore] public ENineStarKiEnergy Energy { get; }
        [ScriptIgnore] public ENineStarKiEnergy RelatedEnergy { get; set; }
        [ScriptIgnore] public EGender Gender { get; set; }
        [ScriptIgnore] public bool IsAdult { get; set; }
        [ScriptIgnore] public ENineStarKiEnergyType EnergyType { get; }
        [ScriptIgnore] public ENineStarKiEnergyCycleType EnergyCycleType { get; set; }
        [ScriptIgnore] public string EnergyLowerCase => Energy.ToString().ToLower();
        [ScriptIgnore] public string ElementNameAndNumber => $"{EnergyNumber} {ElementName}";
        [ScriptIgnore] public string HouseName => $"{ElementNameAndNumber} {Dictionary.House}";
        [ScriptIgnore] public string EnergyNameNumberAndElement => ElementName == EnergyName ? $"{EnergyNumber} {ElementName}" : $"{EnergyNumber} {ElementName} - {EnergyName}";
        [ScriptIgnore] public string EnergyTitle => $"{EnergyNumber} {ElementName} / {EnergyName} - {DescriptiveTitle}";
        [ScriptIgnore] public string DescriptiveTitle => $"The {MetaData.GetDescriptiveTitle()}";

        [ScriptIgnore]
        public ENineStarKiYinYang YinYang => GetYinYang();

        [ScriptIgnore]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ElementLabel)]
        public ENineStarKiElement Element => MetaData.Element;

        [ScriptIgnore]
        public string ElementWithYingYang => $"{YinYangName} {ElementName}".Trim();

        [ScriptIgnore]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ColourLabel)]
        public string Colour => MetaData.GetColour();

        [ScriptIgnore]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ModalityLabel)]
        public ENineStarKiModality Modality => MetaData.Modality;

        [ScriptIgnore]
        public string EpochEnergyLabel => Dictionary.EpochEnergyLabel;

        [ScriptIgnore]
        public string GenerationalEnergyLabel => Dictionary.GenerationalEnergyLabel;

        [ScriptIgnore]
        public string AdultEnergyLabel => Dictionary.MainEnergyLabel;

        [ScriptIgnore]
        public string CharacteEnergyLabel => Dictionary.CharacterEnergyLabel;

        [ScriptIgnore]
        public string SurfaceEnergyLabel => Dictionary.SurfaceEnergyLabel;

        [ScriptIgnore]
        public string DayStarLabel => Dictionary.DayStarLabel;

        public string CycleDescriptiveName => CycleMetaData.DescriptiveTitle;

        [ScriptIgnore]
        public string EpochCycleEnergyLabel => Dictionary.EpochCycleEnergy;

        [ScriptIgnore]
        public string GenerationalCycleEnergyLabel => Dictionary.GenerationalCycleEnergy;

        [ScriptIgnore]
        public string YearlyCycleEnergyLabel => Dictionary.YearlyCycleEnergy;

        [ScriptIgnore]
        public string MonthlyCycleEnergyLabel => Dictionary.MonthlyCycleEnergy;

        [ScriptIgnore]
        public string DailyCycleEnergyLabel => Dictionary.DailyCycleEnergy;

        private NineStarKiEnumMetaDataAttribute _metaData;

        [ScriptIgnore]
        public NineStarKiEnumMetaDataAttribute MetaData
        {
            get
            {
                if (_metaData == null)
                {
                    _metaData = Energy.GetAttribute<NineStarKiEnumMetaDataAttribute>();
                }
                return _metaData;
            }
        }

        private NineStarKiCycleEnumMetaDataAttribute _cycleMetaData;
        internal NineStarKiCycleEnumMetaDataAttribute CycleMetaData
        {
            get
            {
                if (_cycleMetaData == null)
                {
                    _cycleMetaData = MetaData.Cycle.GetAttribute<NineStarKiCycleEnumMetaDataAttribute>();
                }
                return _cycleMetaData;
            }
        }

        private ENineStarKiYinYang GetYinYang()
        {
            if (Energy == ENineStarKiEnergy.CoreEarth && RelatedEnergy == ENineStarKiEnergy.CoreEarth)
            {
                return Gender.IsYin() ? ENineStarKiYinYang.Yin : ENineStarKiYinYang.Yang;
            }
            if (Energy == ENineStarKiEnergy.CoreEarth && RelatedEnergy != ENineStarKiEnergy.Unspecified)
            {
                return RelatedEnergy.GetAttribute<NineStarKiEnumMetaDataAttribute>().YinYang;
            }
            return MetaData.YinYang;
        }

        private string GetEnergyDescription()
        {
            switch (EnergyType)
            {
                case ENineStarKiEnergyType.EpochEnergy:
                    return EpochDescription;

                case ENineStarKiEnergyType.GenerationalEnergy:
                    return GenerationDescription;

                case ENineStarKiEnergyType.MainEnergy:
                    return MainEnergyDescription;

                case ENineStarKiEnergyType.CharacterEnergy:
                    return EmotionalEnergyDescription;

                case ENineStarKiEnergyType.SurfaceEnergy:
                    return SurfaceEnergyDescription;

                case ENineStarKiEnergyType.DailyEnergy:
                    return DayStarDescription;

                default:
                    return string.Empty;
            }
        }

        private string GetCycleDescription()
        {
            switch (EnergyCycleType)
            {
                case ENineStarKiEnergyCycleType.EpochEnergy:
                    return CycleMetaData.EightyOneYearDescription;

                case ENineStarKiEnergyCycleType.GenerationalEnergy:
                    return CycleMetaData.NineYearDescription;

                case ENineStarKiEnergyCycleType.YearlyCycleEnergy:
                    return CycleMetaData.YearlyDescription;

                case ENineStarKiEnergyCycleType.MonthlyCycleEnergy:
                    return CycleMetaData.MonthlyDescription;

                case ENineStarKiEnergyCycleType.DailyEnergy:
                    return CycleMetaData.DailyDescription;

                default:
                    return string.Empty;
            }
        }

        private string GetEpochDescription() => _epochDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetGenerationDescription() => _generationDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetMainEnergyDescription() => _mainEnergyDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetEmotionalEnergyDescription() => _emotionalDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetSurfaceEnergyDescription() => _surfaceEnergyDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetChildDescription() => _childDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetDayStarDescription() => _dayStarDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetOverview() => _overviews.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetHealthDescription() => _healthDetails.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetCareerDescription() => _careerDetails.TryGetValue(Energy, out var desc) ? desc : string.Empty;


        private string GetEpochCycleDescription() => _epochCycleDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetGenerationCycleDescription() => _generationCycleDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetYearlyCycleDescription() => _yearlyCycleDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetMonthlyCycleDescription() => _monthlyCycleDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetDailyCycleDescription() => _daycycleCycleDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetHourlyCycleDescription() => _hourCycleDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;
    }
}