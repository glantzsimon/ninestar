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

            GlobalYearlyDescriptionName = Strings.Names.GlobalWaterYearCycle,
            GlobalMonthlyDescriptionName = Strings.Names.GlobalWaterMonthCycle,
            GlobalEightyOneYearDescriptionName = Strings.Names.GlobalWaterEpochCycle,
            GlobalNineYearDescriptionName = Strings.Names.GlobalWaterGenerationCycle,
            GlobalDailyDescriptionName = Strings.Names.GlobalWaterDayCycle,
            GlobalHourlyDescriptionName = Strings.Names.GlobalWaterHourlyCycle,

            YearlyDescriptionName = Strings.Names.WaterYearCycle,
            MonthlyDescriptionName = Strings.Names.WaterMonthCycle,
            EightyOneYearDescriptionName = Strings.Names.WaterEpochCycle,
            NineYearDescriptionName = Strings.Names.WaterGenerationCycle,
            DailyDescriptionName = Strings.Names.WaterDayCycle,
            HourlyDescriptionName = Strings.Names.WaterHourlyCycle)]
        Winter,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.WinterToSpring,
            Element = ENineStarKiElement.Earth,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Planning,

            GlobalYearlyDescriptionName = Strings.Names.GlobalSoilYearCycle,
            GlobalMonthlyDescriptionName = Strings.Names.GlobalSoilMonthCycle,
            GlobalEightyOneYearDescriptionName = Strings.Names.GlobalSoilEpochCycle,
            GlobalNineYearDescriptionName = Strings.Names.GlobalSoilGenerationCycle,
            GlobalDailyDescriptionName = Strings.Names.GlobalSoilDayCycle,
            GlobalHourlyDescriptionName = Strings.Names.GlobalSoilHourlyCycle,

            YearlyDescriptionName = Strings.Names.SoilYearCycle,
            MonthlyDescriptionName = Strings.Names.SoilMonthCycle,
            EightyOneYearDescriptionName = Strings.Names.SoilEpochCycle,
            NineYearDescriptionName = Strings.Names.SoilGenerationCycle,
            DailyDescriptionName = Strings.Names.SoilDayCycle,
            HourlyDescriptionName = Strings.Names.SoilHourlyCycle)]
        WinterToSpring,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.EarlySpring,
            Element = ENineStarKiElement.Tree,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Sprouting,

            GlobalYearlyDescriptionName = Strings.Names.GlobalThunderYearCycle,
            GlobalMonthlyDescriptionName = Strings.Names.GlobalThunderMonthCycle,
            GlobalEightyOneYearDescriptionName = Strings.Names.GlobalThunderEpochCycle,
            GlobalNineYearDescriptionName = Strings.Names.GlobalThunderGenerationCycle,
            GlobalDailyDescriptionName = Strings.Names.GlobalThunderDayCycle,
            GlobalHourlyDescriptionName = Strings.Names.GlobalThunderHourlyCycle,

            YearlyDescriptionName = Strings.Names.ThunderYearCycle,
            MonthlyDescriptionName = Strings.Names.ThunderMonthCycle,
            EightyOneYearDescriptionName = Strings.Names.ThunderEpochCycle,
            NineYearDescriptionName = Strings.Names.ThunderGenerationCycle,
            DailyDescriptionName = Strings.Names.ThunderDayCycle,
            HourlyDescriptionName = Strings.Names.ThunderHourlyCycle)]
        EarlySpring,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.LateSpring,
            Element = ENineStarKiElement.Tree,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Blossoming,

            GlobalYearlyDescriptionName = Strings.Names.GlobalWindYearCycle,
            GlobalMonthlyDescriptionName = Strings.Names.GlobalWindMonthCycle,
            GlobalEightyOneYearDescriptionName = Strings.Names.GlobalWindEpochCycle,
            GlobalNineYearDescriptionName = Strings.Names.GlobalWindGenerationCycle,
            GlobalDailyDescriptionName = Strings.Names.GlobalWindDayCycle,
            GlobalHourlyDescriptionName = Strings.Names.GlobalWindHourlyCycle,

            YearlyDescriptionName = Strings.Names.WindYearCycle,
            MonthlyDescriptionName = Strings.Names.WindMonthCycle,
            EightyOneYearDescriptionName = Strings.Names.WindEpochCycle,
            NineYearDescriptionName = Strings.Names.WindGenerationCycle,
            DailyDescriptionName = Strings.Names.WindDayCycle,
            HourlyDescriptionName = Strings.Names.WindHourlyCycle)]
        LateSpring,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.Centre,
            Element = ENineStarKiElement.Earth,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Consolidating,

            GlobalYearlyDescriptionName = Strings.Names.GlobalCoreEarthYearCycle,
            GlobalMonthlyDescriptionName = Strings.Names.GlobalCoreEarthMonthCycle,
            GlobalEightyOneYearDescriptionName = Strings.Names.GlobalCoreEarthEpochCycle,
            GlobalNineYearDescriptionName = Strings.Names.GlobalCoreEarthGenerationCycle,
            GlobalDailyDescriptionName = Strings.Names.GlobalCoreEarthDayCycle,
            GlobalHourlyDescriptionName = Strings.Names.GlobalCoreEarthHourlyCycle,

            YearlyDescriptionName = Strings.Names.CoreEarthYearCycle,
            MonthlyDescriptionName = Strings.Names.CoreEarthMonthCycle,
            EightyOneYearDescriptionName = Strings.Names.CoreEarthEpochCycle,
            NineYearDescriptionName = Strings.Names.CoreEarthGenerationCycle,
            DailyDescriptionName = Strings.Names.CoreEarthDayCycle,
            HourlyDescriptionName = Strings.Names.CoreEarthHourlyCycle)]
        Centre,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.EarlyAutumn,
            Element = ENineStarKiElement.Metal,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Ripening,

            GlobalYearlyDescriptionName = Strings.Names.GlobalHeavenYearCycle,
            GlobalMonthlyDescriptionName = Strings.Names.GlobalHeavenMonthCycle,
            GlobalEightyOneYearDescriptionName = Strings.Names.GlobalHeavenEpochCycle,
            GlobalNineYearDescriptionName = Strings.Names.GlobalHeavenGenerationCycle,
            GlobalDailyDescriptionName = Strings.Names.GlobalHeavenDayCycle,
            GlobalHourlyDescriptionName = Strings.Names.GlobalHeavenHourlyCycle,

            YearlyDescriptionName = Strings.Names.HeavenYearCycle,
            MonthlyDescriptionName = Strings.Names.HeavenMonthCycle,
            EightyOneYearDescriptionName = Strings.Names.HeavenEpochCycle,
            NineYearDescriptionName = Strings.Names.HeavenGenerationCycle,
            DailyDescriptionName = Strings.Names.HeavenDayCycle,
            HourlyDescriptionName = Strings.Names.HeavenHourlyCycle)]
        EarlyAutumn,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.LateAutumn,
            Element = ENineStarKiElement.Metal,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Harvest,

            GlobalYearlyDescriptionName = Strings.Names.GlobalLakeYearCycle,
            GlobalMonthlyDescriptionName = Strings.Names.GlobalLakeMonthCycle,
            GlobalEightyOneYearDescriptionName = Strings.Names.GlobalLakeEpochCycle,
            GlobalNineYearDescriptionName = Strings.Names.GlobalLakeGenerationCycle,
            GlobalDailyDescriptionName = Strings.Names.GlobalLakeDayCycle,
            GlobalHourlyDescriptionName = Strings.Names.GlobalLakeHourlyCycle,

            YearlyDescriptionName = Strings.Names.LakeYearCycle,
            MonthlyDescriptionName = Strings.Names.LakeMonthCycle,
            EightyOneYearDescriptionName = Strings.Names.LakeEpochCycle,
            NineYearDescriptionName = Strings.Names.LakeGenerationCycle,
            DailyDescriptionName = Strings.Names.LakeDayCycle,
            HourlyDescriptionName = Strings.Names.LakeHourlyCycle)]
        LateAutumn,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.AutumnToWinter,
            Element = ENineStarKiElement.Earth,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Revolution,

            GlobalYearlyDescriptionName = Strings.Names.GlobalMountainYearCycle,
            GlobalMonthlyDescriptionName = Strings.Names.GlobalMountainMonthCycle,
            GlobalEightyOneYearDescriptionName = Strings.Names.GlobalMountainEpochCycle,
            GlobalNineYearDescriptionName = Strings.Names.GlobalMountainGenerationCycle,
            GlobalDailyDescriptionName = Strings.Names.GlobalMountainDayCycle,
            GlobalHourlyDescriptionName = Strings.Names.GlobalMountainHourlyCycle,

            YearlyDescriptionName = Strings.Names.MountainYearCycle,
            MonthlyDescriptionName = Strings.Names.MountainMonthCycle,
            EightyOneYearDescriptionName = Strings.Names.MountainEpochCycle,
            NineYearDescriptionName = Strings.Names.MountainGenerationCycle,
            DailyDescriptionName = Strings.Names.MountainDayCycle,
            HourlyDescriptionName = Strings.Names.MountainHourlyCycle)]
        AutumnToWinter,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.Summer,
            Element = ENineStarKiElement.Fire,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Spotlight,

            GlobalYearlyDescriptionName = Strings.Names.GlobalFireYearCycle,
            GlobalMonthlyDescriptionName = Strings.Names.GlobalFireMonthCycle,
            GlobalEightyOneYearDescriptionName = Strings.Names.GlobalFireEpochCycle,
            GlobalNineYearDescriptionName = Strings.Names.GlobalFireGenerationCycle,
            GlobalDailyDescriptionName = Strings.Names.GlobalFireDayCycle,
            GlobalHourlyDescriptionName = Strings.Names.GlobalFireHourlyCycle,

            YearlyDescriptionName = Strings.Names.FireYearCycle,
            MonthlyDescriptionName = Strings.Names.FireMonthCycle,
            EightyOneYearDescriptionName = Strings.Names.WindEpochCycle,
            NineYearDescriptionName = Strings.Names.FireGenerationCycle,
            DailyDescriptionName = Strings.Names.FireDayCycle,
            HourlyDescriptionName = Strings.Names.FireHourlyCycle)]
        Summer
    }

    public enum ENineStarKiDirection
    {
        [Display(Order = 1)]
        [NineStarKiDirectionAttribute(DisplayDirection = South)]
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.North)]
        North = 1,

        [Display(Order = 6)]
        [NineStarKiDirectionAttribute(DisplayDirection = NorthEast)]
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.SouthWest)]
        SouthWest,

        [Display(Order = 3)]
        [NineStarKiDirectionAttribute(DisplayDirection = West)]
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.East)]
        East,

        [Display(Order = 4)]
        [NineStarKiDirectionAttribute(DisplayDirection = NorthWest)]
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.SouthEast)]
        SouthEast,

        [Display(Order = 0)]
        [NineStarKiDirectionAttribute(DisplayDirection = Centre)]
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Centre)]
        Centre,

        [Display(Order = 8)]
        [NineStarKiDirectionAttribute(DisplayDirection = SouthEast)]
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.NorthWest)]
        NorthWest,

        [Display(Order = 7)]
        [NineStarKiDirectionAttribute(DisplayDirection = East)]
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.West)]
        West,

        [Display(Order = 2)]
        [NineStarKiDirectionAttribute(DisplayDirection = SouthWest)]
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.NorthEast)]
        NorthEast,

        [Display(Order = 5)]
        [NineStarKiDirectionAttribute(DisplayDirection = North)]
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

        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.ChildNatalHouseEnergyLabel)]
        ChildNatalHouseEnergy,
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
            MostHarmoniousCombinations = new[] { 3, 4, 6, 7 },
            MostChallengingCombinations = new[] { 2, 5, 8, 9 },
            InternalRepresentation = new[] { EBodyPart.Kidneys, EBodyPart.Bladder, EBodyPart.Bones, EBodyPart.NervousSystem, EBodyPart.SexOrgans },
            Physiognomy = new[] { EBodyPart.Ear },
            PrimaryAttributes = new[] { EPrimaryAttribute.Hardship, EPrimaryAttribute.Danger })]
        Water,

        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Soil, Colour = ENineStarKiColour.Black, Element = ENineStarKiElement.Earth, Direction = ENineStarKiDirection.SouthWest, FamilyMember = ENineStarKiFamilyMember.Mother, YinYang = ENineStarKiYinYang.Yin, TrigramName = "Kun", DescriptiveName = ENineStarKiDescriptiveName.Nurturer, Modality = ENineStarKiModality.Stable, Cycle = ENineStarKiCycle.WinterToSpring,
            MostHarmoniousCombinations = new[] { 5, 6, 7, 8, 9 },
            MostChallengingCombinations = new[] { 1, 3, 4 },
            InternalRepresentation = new[] { EBodyPart.Stomach, EBodyPart.Spleen, EBodyPart.LowerBody, EBodyPart.SoftTissue },
            Physiognomy = new[] { EBodyPart.Abdomen },
            PrimaryAttributes = new[] { EPrimaryAttribute.Receptive })]
        Soil,

        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Thunder, Colour = ENineStarKiColour.BrightGreen, Element = ENineStarKiElement.Tree, Direction = ENineStarKiDirection.East, FamilyMember = ENineStarKiFamilyMember.EldestSon, YinYang = ENineStarKiYinYang.Yang, TrigramName = "Chen", DescriptiveName = ENineStarKiDescriptiveName.Pioneer, Modality = ENineStarKiModality.Dynamic, Cycle = ENineStarKiCycle.EarlySpring,
            MostHarmoniousCombinations = new[] { 1, 4, 9 },
            MostChallengingCombinations = new[] { 2, 5, 6, 7, 8 },
            InternalRepresentation = new[] { EBodyPart.Liver, EBodyPart.Feet, EBodyPart.Muscles, EBodyPart.ConnectiveTissue },
            Physiognomy = new[] { EBodyPart.VocalAnatomy },
            PrimaryAttributes = new[] { EPrimaryAttribute.Rousing })]
        Thunder,

        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Wind, Colour = ENineStarKiColour.Green, Element = ENineStarKiElement.Tree, Direction = ENineStarKiDirection.SouthEast, FamilyMember = ENineStarKiFamilyMember.EldestDaughter, YinYang = ENineStarKiYinYang.Yin, TrigramName = "Sun", DescriptiveName = ENineStarKiDescriptiveName.Influencer, Modality = ENineStarKiModality.Reflective, Cycle = ENineStarKiCycle.LateSpring,
            MostHarmoniousCombinations = new[] { 1, 3, 9 },
            MostChallengingCombinations = new[] { 2, 5, 6, 7, 8 },
            InternalRepresentation = new[] { EBodyPart.NervousSystem, EBodyPart.Diaphragm, EBodyPart.Gallbladder },
            Physiognomy = new[] { EBodyPart.Legs, EBodyPart.Eyes },
            PrimaryAttributes = new[] { EPrimaryAttribute.Penetrating })]
        Wind,

        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.CoreEarth, Colour = ENineStarKiColour.Yellow, Element = ENineStarKiElement.Earth, Direction = ENineStarKiDirection.Centre, FamilyMember = ENineStarKiFamilyMember.SeventhChild, YinYang = ENineStarKiYinYang.Unspecified, TrigramName = "None", DescriptiveName = ENineStarKiDescriptiveName.Hub, Modality = ENineStarKiModality.Stable, Cycle = ENineStarKiCycle.Centre,
            MostHarmoniousCombinations = new[] { 2, 6, 7, 8, 9 },
            MostChallengingCombinations = new[] { 1, 3, 4 },
            InternalRepresentation = new[] { EBodyPart.Stomach, EBodyPart.Spleen, EBodyPart.Pancreas, EBodyPart.LymphaticSystem },
            Physiognomy = new[] { EBodyPart.Hands, EBodyPart.Abdomen },
            PrimaryAttributes = new[] { EPrimaryAttribute.Catalyst })]
        CoreEarth,

        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Heaven, Colour = ENineStarKiColour.White, Element = ENineStarKiElement.Metal, Direction = ENineStarKiDirection.NorthWest, FamilyMember = ENineStarKiFamilyMember.Father, YinYang = ENineStarKiYinYang.Yang, TrigramName = "Chien", DescriptiveName = ENineStarKiDescriptiveName.Leader, Modality = ENineStarKiModality.Dynamic, Cycle = ENineStarKiCycle.EarlyAutumn,
            MostHarmoniousCombinations = new[] { 1, 2, 5, 7, 8 },
            MostChallengingCombinations = new[] { 3, 4, 9 },
            InternalRepresentation = new[] { EBodyPart.PinealGland, EBodyPart.Lungs, EBodyPart.LargeIntestine, EBodyPart.Skin },
            Physiognomy = new[] { EBodyPart.Skull, EBodyPart.Head },
            PrimaryAttributes = new[] { EPrimaryAttribute.Creative })]
        Heaven,

        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Lake, Colour = ENineStarKiColour.Red, Element = ENineStarKiElement.Metal, Direction = ENineStarKiDirection.West, FamilyMember = ENineStarKiFamilyMember.YoungestDaughter, YinYang = ENineStarKiYinYang.Yin, TrigramName = "Tui", DescriptiveName = ENineStarKiDescriptiveName.Advisor, Modality = ENineStarKiModality.Reflective, Cycle = ENineStarKiCycle.LateAutumn,
            MostHarmoniousCombinations = new[] { 1, 2, 5, 6, 8 },
            MostChallengingCombinations = new[] { 3, 4, 9 },
            InternalRepresentation = new[] { EBodyPart.SpeechAnatomy, EBodyPart.Lungs, EBodyPart.LargeIntestine, EBodyPart.Skin },
            Physiognomy = new[] { EBodyPart.Mouth, EBodyPart.Lips },
            PrimaryAttributes = new[] { EPrimaryAttribute.Ease })]
        Lake,

        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Mountain, Colour = ENineStarKiColour.White, Element = ENineStarKiElement.Earth, Direction = ENineStarKiDirection.NorthEast, FamilyMember = ENineStarKiFamilyMember.YoungestSon, YinYang = ENineStarKiYinYang.Yang, TrigramName = "Ken", DescriptiveName = ENineStarKiDescriptiveName.Pragmatist, Modality = ENineStarKiModality.Stable, Cycle = ENineStarKiCycle.AutumnToWinter,
            MostHarmoniousCombinations = new[] { 2, 5, 6, 7, 9 },
            MostChallengingCombinations = new[] { 1, 3, 4 },
            InternalRepresentation = new[] { EBodyPart.Back, EBodyPart.Spleen, EBodyPart.Pancreas, EBodyPart.Stomach },
            Physiognomy = new[] { EBodyPart.Hands },
            PrimaryAttributes = new[] { EPrimaryAttribute.Stillness })]
        Mountain,

        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Fire, Colour = ENineStarKiColour.Purple, Element = ENineStarKiElement.Fire, Direction = ENineStarKiDirection.South, FamilyMember = ENineStarKiFamilyMember.MiddleDaughter, YinYang = ENineStarKiYinYang.Yin, TrigramName = "Li", DescriptiveName = ENineStarKiDescriptiveName.Communicator, Modality = ENineStarKiModality.Dynamic, Cycle = ENineStarKiCycle.Summer,
            MostHarmoniousCombinations = new[] { 2, 3, 4, 5, 8 },
            MostChallengingCombinations = new[] { 1, 6, 7 },
            InternalRepresentation = new[] { EBodyPart.CardiovascularSystem, EBodyPart.Heart, EBodyPart.HeartGovernor, EBodyPart.SmallIntestine, EBodyPart.TripleHeater },
            Physiognomy = new[] { EBodyPart.Eyes },
            PrimaryAttributes = new[] { EPrimaryAttribute.Clarity })]
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

        private static readonly Dictionary<ENineStarKiEnergy, string> _mainEnergySummaries =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_description_summary},
                {ENineStarKiEnergy.Soil, Dictionary.soil_description_summary},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_description_summary},
                {ENineStarKiEnergy.Wind, Dictionary.wind_description_summary},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_description_summary},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_description_summary},
                {ENineStarKiEnergy.Lake, Dictionary.lake_description_summary},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_description_summary},
                {ENineStarKiEnergy.Fire, Dictionary.fire_description_summary}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _characterEnergySummaries =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_emotional_description_summary},
                {ENineStarKiEnergy.Soil, Dictionary.soil_emotional_description_summary},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_emotional_description_summary},
                {ENineStarKiEnergy.Wind, Dictionary.wind_emotional_description_summary},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_emotional_description_summary},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_emotional_description_summary},
                {ENineStarKiEnergy.Lake, Dictionary.lake_emotional_description_summary},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_emotional_description_summary},
                {ENineStarKiEnergy.Fire, Dictionary.fire_emotional_description_summary}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _surfaceEnergySummaries =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_surface_description_summary},
                {ENineStarKiEnergy.Soil, Dictionary.soil_surface_description_summary},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_surface_description_summary},
                {ENineStarKiEnergy.Wind, Dictionary.wind_surface_description_summary},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_surface_description_summary},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_surface_description_summary},
                {ENineStarKiEnergy.Lake, Dictionary.lake_surface_description_summary},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_surface_description_summary},
                {ENineStarKiEnergy.Fire, Dictionary.fire_surface_description_summary}
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

        private static readonly Dictionary<ENineStarKiEnergy, string> _occupations =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_occupations},
                {ENineStarKiEnergy.Soil, Dictionary.soil_occupations},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_occupations},
                {ENineStarKiEnergy.Wind, Dictionary.wind_occupations},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_occupations},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_occupations},
                {ENineStarKiEnergy.Lake, Dictionary.lake_occupations},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_occupations},
                {ENineStarKiEnergy.Fire, Dictionary.fire_occupations}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _careerDescriptions =
            new Dictionary<ENineStarKiEnergy, string>
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

        private static readonly Dictionary<ENineStarKiEnergy, string> _careerSummaries =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_career_summary},
                {ENineStarKiEnergy.Soil, Dictionary.soil_career_summary},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_career_summary},
                {ENineStarKiEnergy.Wind, Dictionary.wind_career_summary},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_career_summary},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_career_summary},
                {ENineStarKiEnergy.Lake, Dictionary.lake_career_summary},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_career_summary},
                {ENineStarKiEnergy.Fire, Dictionary.fire_career_summary}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _emotionalLandscapeDescriptions =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_emotional_landscape},
                {ENineStarKiEnergy.Soil, Dictionary.soil_emotional_landscape},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_emotional_landscape},
                {ENineStarKiEnergy.Wind, Dictionary.wind_emotional_landscape},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_emotional_landscape},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_emotional_landscape},
                {ENineStarKiEnergy.Lake, Dictionary.lake_emotional_landscape},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_emotional_landscape},
                {ENineStarKiEnergy.Fire, Dictionary.fire_emotional_landscape}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _emotionalLandscapeSummaries =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_emotional_landscape_summary},
                {ENineStarKiEnergy.Soil, Dictionary.soil_emotional_landscape_summary},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_emotional_landscape_summary},
                {ENineStarKiEnergy.Wind, Dictionary.wind_emotional_landscape_summary},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_emotional_landscape_summary},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_emotional_landscape_summary},
                {ENineStarKiEnergy.Lake, Dictionary.lake_emotional_landscape_summary},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_emotional_landscape_summary},
                {ENineStarKiEnergy.Fire, Dictionary.fire_emotional_landscape_summary}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _finances =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_finances},
                {ENineStarKiEnergy.Soil, Dictionary.soil_finances},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_finances},
                {ENineStarKiEnergy.Wind, Dictionary.wind_finances},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_finances},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_finances},
                {ENineStarKiEnergy.Lake, Dictionary.lake_finances},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_finances},
                {ENineStarKiEnergy.Fire, Dictionary.fire_finances}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _financesSummaries =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_finances_summary},
                {ENineStarKiEnergy.Soil, Dictionary.soil_finances_summary},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_finances_summary},
                {ENineStarKiEnergy.Wind, Dictionary.wind_finances_summary},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_finances_summary},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_finances_summary},
                {ENineStarKiEnergy.Lake, Dictionary.lake_finances_summary},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_finances_summary},
                {ENineStarKiEnergy.Fire, Dictionary.fire_finances_summary}
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

        private static readonly Dictionary<ENineStarKiEnergy, string> _healthSummaries
            = new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_health_summary},
                {ENineStarKiEnergy.Soil, Dictionary.soil_health_summary},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_health_summary},
                {ENineStarKiEnergy.Wind, Dictionary.wind_health_summary},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_health_summary},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_health_summary},
                {ENineStarKiEnergy.Lake, Dictionary.lake_health_summary},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_health_summary},
                {ENineStarKiEnergy.Fire, Dictionary.fire_health_summary}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _illnesses
            = new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_illnesses},
                {ENineStarKiEnergy.Soil, Dictionary.soil_illnesses},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_illnesses},
                {ENineStarKiEnergy.Wind, Dictionary.wind_illnesses},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_illnesses},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_illnesses},
                {ENineStarKiEnergy.Lake, Dictionary.lake_illnesses},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_illnesses},
                {ENineStarKiEnergy.Fire, Dictionary.fire_illnesses}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _intellectualQualities
            = new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_intellectual_qualities},
                {ENineStarKiEnergy.Soil, Dictionary.soil_intellectual_qualities},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_intellectual_qualities},
                {ENineStarKiEnergy.Wind, Dictionary.wind_intellectual_qualities},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_intellectual_qualities},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_intellectual_qualities},
                {ENineStarKiEnergy.Lake, Dictionary.lake_intellectual_qualities},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_intellectual_qualities},
                {ENineStarKiEnergy.Fire, Dictionary.fire_intellectual_qualities}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _intellectualQualitiesSummaries
            = new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_intellectual_qualities_summary},
                {ENineStarKiEnergy.Soil, Dictionary.soil_intellectual_qualities_summary},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_intellectual_qualities_summary},
                {ENineStarKiEnergy.Wind, Dictionary.wind_intellectual_qualities_summary},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_intellectual_qualities_summary},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_intellectual_qualities_summary},
                {ENineStarKiEnergy.Lake, Dictionary.lake_intellectual_qualities_summary},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_intellectual_qualities_summary},
                {ENineStarKiEnergy.Fire, Dictionary.fire_intellectual_qualities_summary}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _interpersonalQualities
            = new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_interpersonal_qualities},
                {ENineStarKiEnergy.Soil, Dictionary.soil_interpersonal_qualities},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_interpersonal_qualities},
                {ENineStarKiEnergy.Wind, Dictionary.wind_interpersonal_qualities},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_interpersonal_qualities},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_interpersonal_qualities},
                {ENineStarKiEnergy.Lake, Dictionary.lake_interpersonal_qualities},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_interpersonal_qualities},
                {ENineStarKiEnergy.Fire, Dictionary.fire_interpersonal_qualities}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _interpersonalQualitiesSummaries
            = new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_interpersonal_qualities_summary},
                {ENineStarKiEnergy.Soil, Dictionary.soil_interpersonal_qualities_summary},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_interpersonal_qualities_summary},
                {ENineStarKiEnergy.Wind, Dictionary.wind_interpersonal_qualities_summary},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_interpersonal_qualities_summary},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_interpersonal_qualities_summary},
                {ENineStarKiEnergy.Lake, Dictionary.lake_interpersonal_qualities_summary},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_interpersonal_qualities_summary},
                {ENineStarKiEnergy.Fire, Dictionary.fire_interpersonal_qualities_summary}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _spiritualityDetails
            = new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_spirituality},
                {ENineStarKiEnergy.Soil, Dictionary.soil_spirituality},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_spirituality},
                {ENineStarKiEnergy.Wind, Dictionary.wind_spirituality},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_spirituality},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_spirituality},
                {ENineStarKiEnergy.Lake, Dictionary.lake_spirituality},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_spirituality},
                {ENineStarKiEnergy.Fire, Dictionary.fire_spirituality}
            };

        private static readonly Dictionary<ENineStarKiEnergy, string> _spiritualitySummaries
            = new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.water_spirituality_summary},
                {ENineStarKiEnergy.Soil, Dictionary.soil_spirituality_summary},
                {ENineStarKiEnergy.Thunder, Dictionary.thunder_spirituality_summary},
                {ENineStarKiEnergy.Wind, Dictionary.wind_spirituality_summary},
                {ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_spirituality_summary},
                {ENineStarKiEnergy.Heaven, Dictionary.heaven_spirituality_summary},
                {ENineStarKiEnergy.Lake, Dictionary.lake_spirituality_summary},
                {ENineStarKiEnergy.Mountain, Dictionary.mountain_spirituality_summary},
                {ENineStarKiEnergy.Fire, Dictionary.fire_spirituality_summary}
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

        private static readonly Dictionary<ENineStarKiEnergy, string> _yearlyGlobalCycleDescriptions =
            new Dictionary<ENineStarKiEnergy, string>
            {
                {ENineStarKiEnergy.Water, Dictionary.global_water_year},
                {ENineStarKiEnergy.Soil, Dictionary.global_soil_year},
                {ENineStarKiEnergy.Thunder, Dictionary.global_thunder_year},
                {ENineStarKiEnergy.Wind, Dictionary.global_wind_year},
                {ENineStarKiEnergy.CoreEarth, Dictionary.global_core_earth_year},
                {ENineStarKiEnergy.Heaven, Dictionary.global_heaven_year},
                {ENineStarKiEnergy.Lake, Dictionary.global_lake_year},
                {ENineStarKiEnergy.Mountain, Dictionary.global_mountain_year},
                {ENineStarKiEnergy.Fire, Dictionary.global_fire_year}
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

        /// <summary>
        /// Flag for displaying Global energy description
        /// </summary>
        public bool IsGlobal { get; set; }

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

        public NineStarKiEnergy GetHouseOfFive() => new NineStarKiEnergy((ENineStarKiEnergy)NineStarKiModel.GetNineStarKiNumber(5 + (5 - EnergyNumber)), EnergyCycleType);

        #region Personal Chart

        public string EnergyEssenceTitle => $"{ElementNameAndNumber} {Dictionary.Essence}";

        public string EnergyIntellectualQualitiesTitle => $"{ElementNameAndNumber} {Dictionary.IntellectualQualities}";

        public string EnergyInterpersonalQualitiesTitle => $"{ElementNameAndNumber} {Dictionary.InterpersonalQualities}";

        public string EnergyEmotionalLandscapeTitle => $"{ElementNameAndNumber} {Dictionary.EmotionalLandscape}";

        public string EnergyCareerTitle => $"{ElementNameAndNumber} {Dictionary.Career}";

        public string EnergyOccupationsTitle => $"{ElementNameAndNumber} {Dictionary.OccupationsLabel}";

        public string EnergyFinancesTitle => $"{ElementNameAndNumber} {Dictionary.Finances}";

        public string EnergyHealthTitle => $"{ElementNameAndNumber} {Dictionary.Health}";

        public string EnergyIllnessesTitle => $"{ElementNameAndNumber} {Dictionary.Illnesses}";

        public string EnergySpiritualityTitle => $"{ElementNameAndNumber} {Dictionary.Spirituality}";

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

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.MainEnergyLabel)]
        public string MainEnergySummary =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetMainEnergySummary() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.CharacterEnergyLabel)]
        public string CharacterEnergySummary =>
            EnergyType == ENineStarKiEnergyType.CharacterEnergy ? GetCharacterEnergySummary() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.CharacterEnergyLabel)]
        public string EmotionalEnergyDescription => EnergyType == ENineStarKiEnergyType.CharacterEnergy
            ? GetEmotionalEnergyDescription()
            : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SurfaceEnergyLabel)]
        public string SurfaceEnergyDescription => EnergyType == ENineStarKiEnergyType.SurfaceEnergy
            ? GetSurfaceEnergyDescription()
            : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SurfaceEnergyLabel)]
        public string SurfaceEnergySummary =>
            EnergyType == ENineStarKiEnergyType.SurfaceEnergy ? GetSurfaceEnergySummary() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ChildLabel)]
        public string ChildDescription =>
            EnergyType == ENineStarKiEnergyType.CharacterEnergy ? GetChildDescription() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DayStarLabel)]
        public string DayStarDescription =>
            EnergyType == ENineStarKiEnergyType.DailyEnergy ? GetDayStarDescription() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.OverviewLabel)]
        public string OverviewDescription =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetOverview() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.CareerLabel)]
        public string CareerDescription =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetCareerDescription() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.CareerLabel)]
        public string CareerSummary =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetCareerSummary() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.OccupationsLabel)]
        public string Occupations =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetOccupationsDescription() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EmotionalLandscapeLabel)]
        public string EmotionalLandscape =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetEmotionalLandscape() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EmotionalLandscapeLabel)]
        public string EmotionalLandscapeSummary =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetEmotionalLandscapeSummary() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.FinancesLabel)]
        public string Finances =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetFinancesDescription() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.FinancesLabel)]
        public string FinancesSummary =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetFinancesSummary() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.HealthLabel)]
        public string Health =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetHealthDescription() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.HealthLabel)]
        public string HealthSummary =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetHealthSummary() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.IllnessesLabel)]
        public string Illnesses =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetIllnesses() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.IntellectualQualitiesLabel)]
        public string IntellectualQualities =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetIntellectualQualities() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.IntellectualQualitiesLabel)]
        public string IntellectualQualitiesSummary =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetIntellectualQualitiesSummary() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.InterpersonalQualitiesLabel)]
        public string InterpersonalQualities =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetInterpersonalQualities() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.InterpersonalQualitiesLabel)]
        public string InterpersonalQualitiesSummary =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetInterpersonalQualitiesSummary() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SpiritualityLabel)]
        public string Spirituality =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetSpiritualityDescription() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SpiritualityLabel)]
        public string SpiritualitySummary =>
            EnergyType == ENineStarKiEnergyType.MainEnergy ? GetSpiritualitySummary() : string.Empty;

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

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Names.YearlyCycleEnergy)]
        public string YearlyGlobalCycleDescription => EnergyCycleType == ENineStarKiEnergyCycleType.YearlyCycleEnergy
            ? GetGlobalYearlyCycleDescription()
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

        public string CycleDescription => IsGlobal ? GetGlobalCycleDescription() : GetCycleDescription();

        public string GlobalCycleDescription => GetGlobalCycleDescription();

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

                case ENineStarKiEnergyCycleType.HourlyEnergy:
                    return CycleMetaData.HourlyDescription;

                default:
                    return string.Empty;
            }
        }

        private string GetGlobalCycleDescription()
        {
            switch (EnergyCycleType)
            {
                case ENineStarKiEnergyCycleType.EpochEnergy:
                    return CycleMetaData.GlobalEightyOneYearDescription;

                case ENineStarKiEnergyCycleType.GenerationalEnergy:
                    return CycleMetaData.GlobalNineYearDescription;

                case ENineStarKiEnergyCycleType.YearlyCycleEnergy:
                    return CycleMetaData.GlobalYearlyDescription;

                case ENineStarKiEnergyCycleType.MonthlyCycleEnergy:
                    return CycleMetaData.GlobalMonthlyDescription;

                case ENineStarKiEnergyCycleType.DailyEnergy:
                    return CycleMetaData.GlobalDailyDescription;

                default:
                    return string.Empty;
            }
        }

        private string GetEpochDescription() => _epochDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetGenerationDescription() => _generationDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetMainEnergyDescription() => _mainEnergyDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetCharacterEnergySummary() => _characterEnergySummaries.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetSurfaceEnergySummary() => _surfaceEnergySummaries.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetMainEnergySummary() => _mainEnergySummaries.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetEmotionalLandscape() => _emotionalLandscapeDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetEmotionalLandscapeSummary() => _emotionalLandscapeSummaries.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetFinancesDescription() => _finances.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetFinancesSummary() => _financesSummaries.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetHealthDescription() => _healthDetails.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetHealthSummary() => _healthSummaries.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetIllnesses() => _illnesses.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetIntellectualQualities() => _intellectualQualities.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetIntellectualQualitiesSummary() => _intellectualQualitiesSummaries.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetInterpersonalQualities() => _interpersonalQualities.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetInterpersonalQualitiesSummary() => _interpersonalQualitiesSummaries.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetCareerDescription() => _careerDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetCareerSummary() => _careerSummaries.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetOccupationsDescription() => _occupations.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetSpiritualityDescription() => _spiritualityDetails.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetSpiritualitySummary() => _spiritualitySummaries.TryGetValue(Energy, out var desc) ? desc : string.Empty;



        private string GetSurfaceEnergyDescription() => _surfaceEnergyDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetChildDescription() => _childDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetEmotionalEnergyDescription() => _emotionalDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetDayStarDescription() => _dayStarDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetOverview() => _overviews.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetEpochCycleDescription() => _epochCycleDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetGenerationCycleDescription() => _generationCycleDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetYearlyCycleDescription() => _yearlyCycleDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetGlobalYearlyCycleDescription() => _yearlyGlobalCycleDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetMonthlyCycleDescription() => _monthlyCycleDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetDailyCycleDescription() => _daycycleCycleDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;

        private string GetHourlyCycleDescription() => _hourCycleDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;
    }
}