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
            MonthlyDescriptionName = Strings.Names.WaterMonth)]
        Winter,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.WinterToSpring,
            Element = ENineStarKiElement.Earth,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Planning,
            YearlyDescriptionName = Strings.Names.SoilYear,
            MonthlyDescriptionName = Strings.Names.SoilMonth)]
        WinterToSpring,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.EarlySpring,
            Element = ENineStarKiElement.Tree,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Sprouting,
            YearlyDescriptionName = Strings.Names.ThunderYear,
            MonthlyDescriptionName = Strings.Names.ThunderMonth)]
        EarlySpring,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.LateSpring,
            Element = ENineStarKiElement.Tree,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Blossoming,
            YearlyDescriptionName = Strings.Names.WindYear,
            MonthlyDescriptionName = Strings.Names.WindMonth)]
        LateSpring,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.Centre,
            Element = ENineStarKiElement.Earth,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Consolidating,
            YearlyDescriptionName = Strings.Names.CoreEarthYear,
            MonthlyDescriptionName = Strings.Names.CoreEarthMonth)]
        Centre,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.EarlyAutumn,
            Element = ENineStarKiElement.Metal,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Ripening,
            YearlyDescriptionName = Strings.Names.HeavenYear,
            MonthlyDescriptionName = Strings.Names.HeavenMonth)]
        EarlyAutumn,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.LateAutumn,
            Element = ENineStarKiElement.Metal,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Harvest,
            YearlyDescriptionName = Strings.Names.LakeYear,
            MonthlyDescriptionName = Strings.Names.LakeMonth)]
        LateAutumn,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.AutumnToWinter,
            Element = ENineStarKiElement.Earth,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Revolution,
            YearlyDescriptionName = Strings.Names.MountainYear,
            MonthlyDescriptionName = Strings.Names.MountainMonth)]
        AutumnToWinter,

        [NineStarKiCycleEnumMetaData(ResourceType = typeof(Dictionary),
            Season = Strings.Names.Summer,
            Element = ENineStarKiElement.Fire,
            DescriptiveName = ENineStarKiCycleDescriptiveName.Spotlight,
            YearlyDescriptionName = Strings.Names.FireYear,
            MonthlyDescriptionName = Strings.Names.FireMonth)]
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
        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Water, Colour = ENineStarKiColour.White, Element = ENineStarKiElement.Water, Direction = ENineStarKiDirection.North, FamilyMember = ENineStarKiFamilyMember.MiddleSon, YinYang = ENineStarKiYinYang.Yang, TrigramName = "Kan", DescriptiveName = ENineStarKiDescriptiveName.Diplomat, Modality = ENineStarKiModality.Reflective, Cycle = ENineStarKiCycle.Winter)]
        Water,
        [NineStarKiEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Soil, Colour = ENineStarKiColour.Black, Element = ENineStarKiElement.Earth, Direction = ENineStarKiDirection.SouthWest, FamilyMember = ENineStarKiFamilyMember.Mother, YinYang = ENineStarKiYinYang.Yin, TrigramName = "Kun", DescriptiveName = ENineStarKiDescriptiveName.Nurturer, Modality = ENineStarKiModality.Stable, Cycle = ENineStarKiCycle.WinterToSpring)]
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
        private static readonly Dictionary<ENineStarKiEnergy, string> _trigramDescriptions = new Dictionary<ENineStarKiEnergy, string>
        {
            { ENineStarKiEnergy.Water, Dictionary.water_trigram },
            { ENineStarKiEnergy.Wind, Dictionary.wind_trigram },
            { ENineStarKiEnergy.Lake, Dictionary.lake_trigram },
            { ENineStarKiEnergy.Soil, Dictionary.soil_trigram },
            { ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_trigram },
            { ENineStarKiEnergy.Mountain, Dictionary.mountain_trigram },
            { ENineStarKiEnergy.Thunder, Dictionary.thunder_trigram },
            { ENineStarKiEnergy.Heaven, Dictionary.heaven_trigram },
            { ENineStarKiEnergy.Fire, Dictionary.fire_trigram }
        };

        private static readonly Dictionary<ENineStarKiEnergy, string> _childDescriptions = new Dictionary<ENineStarKiEnergy, string>
        {
            { ENineStarKiEnergy.Water, Dictionary.water_child },
            { ENineStarKiEnergy.Soil, Dictionary.soil_child },
            { ENineStarKiEnergy.Thunder, Dictionary.thunder_child },
            { ENineStarKiEnergy.Wind, Dictionary.wind_child },
            { ENineStarKiEnergy.CoreEarth, Dictionary.coreearth_child },
            { ENineStarKiEnergy.Heaven, Dictionary.heaven_child },
            { ENineStarKiEnergy.Lake, Dictionary.lake_child },
            { ENineStarKiEnergy.Mountain, Dictionary.mountain_child },
            { ENineStarKiEnergy.Fire, Dictionary.fire_child }
        };

        public NineStarKiEnergy(ENineStarKiEnergy energy, ENineStarKiEnergyType type, bool isAdult = true)
        {
            Energy = energy;
            EnergyType = type;
            IsAdult = isAdult;
        }

        public NineStarKiEnergy(ENineStarKiEnergy energy, ENineStarKiEnergyCycleType energyCycleType = ENineStarKiEnergyCycleType.Unspecified)
        {
            Energy = energy;
            EnergyCycleType = energyCycleType;
        }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ChildLabel)]
        public string ChildDescription => EnergyType == ENineStarKiEnergyType.CharacterEnergy ? GetChildDescription() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EnergyLabel)]
        public string EnergyName => MetaData.GetDescription();

        public int EnergyNumber => (int)Energy;
        
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EnergyDescriptionLabel)]
        public string EnergyDescription { get; set; }

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
        
        public string CycleDescription => EnergyType == ENineStarKiEnergyType.MainEnergy ? CycleMetaData.YearlyDescription : CycleMetaData.MonthlyDescription;

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
        [ScriptIgnore] public string EnergyNameAndNumber => $"{EnergyNumber} {EnergyName}";
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
        public string AdultEnergyLabel => Dictionary.MainEnergyLabel;

        [ScriptIgnore]
        public string CharacteEnergyLabel => Dictionary.CharacterEnergyLabel;

        [ScriptIgnore]
        public string CycleDescriptiveName => CycleMetaData.DescriptiveTitle;

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
        
        private string GetChildDescription() => _childDescriptions.TryGetValue(Energy, out var desc) ? desc : string.Empty;
        
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
    }
}