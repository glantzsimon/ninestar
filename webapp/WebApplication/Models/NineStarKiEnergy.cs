using System;
using K9.Base.DataAccessLayer.Attributes;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Attributes;
using K9.WebApplication.Enums;
using K9.WebApplication.Extensions;
using System.ComponentModel.DataAnnotations;

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

    public enum ENineStarKiElenement
    {
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Water)]
        Water,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Earth)]
        Earth,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Tree)]
        Tree,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Metal)]
        Metal,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Fire)]
        Fire
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

    public enum ENineStarKiDirection
    {
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Unspecified)]
        Centre,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.North)]
        North,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.NorthWest)]
        NorthWest,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.West)]
        West,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.SouthWest)]
        SouthWest,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.South)]
        South,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.SouthEast)]
        SouthEast,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.East)]
        East,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.NorthEast)]
        NorthEast
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
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Flexible)]
        Flexible,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Static)]
        Static,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Instigative)]
        Instigative
    }

    public enum ENineStarKiEnergyType
    {
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.MainEnergy)]
        MainEnergy,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.EmotionalEnergy)]
        EmotionalEnergy,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.SurfaceEnergy)]
        SurfaceEnergy,
    }

    public enum ENineStarKiEnergy
    {
        [NineStarEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Unspecified)]
        Unspecified,
        [NineStarEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Water, Colour = ENineStarKiColour.White, Element = ENineStarKiElenement.Water, Direction = ENineStarKiDirection.North, FamilyMember = ENineStarKiFamilyMember.MiddleSon, YinYang = ENineStarKiYinYang.Yang, TrigramName = "Kan", DescriptiveName = ENineStarKiDescriptiveName.Diplomat)]
        Water,
        [NineStarEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Soil, Colour = ENineStarKiColour.Black, Element = ENineStarKiElenement.Earth, Direction = ENineStarKiDirection.SouthWest, FamilyMember = ENineStarKiFamilyMember.Mother, YinYang = ENineStarKiYinYang.Yin, TrigramName = "Kun", DescriptiveName = ENineStarKiDescriptiveName.Nurturer)]
        Soil,
        [NineStarEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Thunder, Colour = ENineStarKiColour.BrightGreen, Element = ENineStarKiElenement.Tree, Direction = ENineStarKiDirection.East, FamilyMember = ENineStarKiFamilyMember.EldestSon, YinYang = ENineStarKiYinYang.Yang, TrigramName = "Chen", DescriptiveName = ENineStarKiDescriptiveName.Pioneer)]
        Thunder,
        [NineStarEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Wind, Colour = ENineStarKiColour.Green, Element = ENineStarKiElenement.Tree, Direction = ENineStarKiDirection.SouthEast, FamilyMember = ENineStarKiFamilyMember.EldestDaughter, YinYang = ENineStarKiYinYang.Yin, TrigramName = "Sun", DescriptiveName = ENineStarKiDescriptiveName.Influencer)]
        Wind,
        [NineStarEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.CoreEarth, Colour = ENineStarKiColour.Yellow, Element = ENineStarKiElenement.Earth, Direction = ENineStarKiDirection.Centre, FamilyMember = ENineStarKiFamilyMember.SeventhChild, YinYang = ENineStarKiYinYang.Unspecified, TrigramName = "None", DescriptiveName = ENineStarKiDescriptiveName.Hub)]
        CoreEarth,
        [NineStarEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Heaven, Colour = ENineStarKiColour.White, Element = ENineStarKiElenement.Metal, Direction = ENineStarKiDirection.NorthWest, FamilyMember = ENineStarKiFamilyMember.Father, YinYang = ENineStarKiYinYang.Yang, TrigramName = "Chien", DescriptiveName = ENineStarKiDescriptiveName.Leader)]
        Heaven,
        [NineStarEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Lake, Colour = ENineStarKiColour.Red, Element = ENineStarKiElenement.Metal, Direction = ENineStarKiDirection.West, FamilyMember = ENineStarKiFamilyMember.YoungestDaughter, YinYang = ENineStarKiYinYang.Yin, TrigramName = "Tui", DescriptiveName = ENineStarKiDescriptiveName.Advisor)]
        Lake,
        [NineStarEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Mountain, Colour = ENineStarKiColour.White, Element = ENineStarKiElenement.Earth, Direction = ENineStarKiDirection.NorthEast, FamilyMember = ENineStarKiFamilyMember.YoungestSon, YinYang = ENineStarKiYinYang.Yang, TrigramName = "Ken", DescriptiveName = ENineStarKiDescriptiveName.Pragmatist)]
        Mountain,
        [NineStarEnumMetaData(ResourceType = typeof(Dictionary), Name = Strings.Names.Fire, Colour = ENineStarKiColour.Purple, Element = ENineStarKiElenement.Fire, Direction = ENineStarKiDirection.South, FamilyMember = ENineStarKiFamilyMember.MiddleDaughter, YinYang = ENineStarKiYinYang.Yin, TrigramName = "Li", DescriptiveName = ENineStarKiDescriptiveName.Communicator)]
        Fire
    }

    public class NineStarKiEnergy
    {
        public NineStarKiEnergy(ENineStarKiEnergy energy, ENineStarKiEnergyType type)
        {
            Energy = energy;
            EnergyType = type;
        }

        public ENineStarKiEnergy Energy { get; }

        /// <summary>
        /// Used to determine YinYang of 5 energies
        /// </summary>
        public ENineStarKiEnergy RelatedEnergy { get; set; }

        /// <summary>
        /// Used to determine YinYang of 5.5.5 energies
        /// </summary>
        public EGender Gender { get; set; }

        public ENineStarKiEnergyType EnergyType { get; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EnergyDescriptionLabel)]
        public string EnergyDescription { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ChildLabel)]
        public string ChildDescription => EnergyType == ENineStarKiEnergyType.EmotionalEnergy ? GetChild() : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EnergyLabel)]
        public string EnergyName => MetaData.GetDescription();

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EnergyLabel)]
        public string DescriptiveTitle => $"The {MetaData.GetDescriptiveTitle()}";

        public string FullEnergyName => GetFullEnergyName();

        public string FullEnergyDetailsTitle => GetFullEnergyDetailsTitle();

        public int EnergyNumber => (int)Energy;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.YinYangLabel)]
        public ENineStarKiYinYang YinYang => GetYinYang();

        public string YinYangName => YinYang == ENineStarKiYinYang.Unspecified ? string.Empty : YinYang.ToString();

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.FamilyMemberLabel)]
        public string FamilyMember => MetaData.GetFamilyMember();

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.TrigramLabel)]
        public string Trigram => MetaData.GetTrigram();

        public string TrigramUIName => $"{MetaData.TrigramName}{EnergyType}";

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.TrigramLabel)]
        public string TrigramDescription => GetTrigramDescription();

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ElementLabel)]
        public string Element => MetaData.GetElement();

        public string ElementWithYingYang => $"{YinYangName} {Element}".Trim();

        public string ElementTitle => $"{Element} {Dictionary.Element}";

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ElementLabel)]
        public string ElementDescription => MetaData.GetElementDescription();

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ColourLabel)]
        public string Colour => MetaData.GetColour();

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ModalityLabel)]
        public ENineStarKiModality Modality => GetModality();

        public string Direction => MetaData.GetDirection();

        private string GetFullEnergyDetailsTitle()
        {
            return EnergyType == ENineStarKiEnergyType.MainEnergy
                ? $"{FullEnergyName} - {DescriptiveTitle}"
                : FullEnergyName;
        }

        private string GetFullEnergyName()
        {
            switch (EnergyNumber)
            {
                case 1:
                case 5:
                case 9:
                    return $"{EnergyNumber} {EnergyName}".Trim();

                default:
                    return $"{EnergyNumber} {EnergyName} / {ElementWithYingYang}".Trim();
            }
        }

        private NineStarEnumMetaDataAttribute MetaData => Energy.GetAttribute<NineStarEnumMetaDataAttribute>();

        private NineStarEnumMetaDataAttribute RelatedMetaData => RelatedEnergy.GetAttribute<NineStarEnumMetaDataAttribute>();

        private ENineStarKiYinYang GetYinYang()
        {
            if (Energy == ENineStarKiEnergy.CoreEarth && RelatedEnergy == ENineStarKiEnergy.CoreEarth)
            {
                return Gender.IsYin() ? ENineStarKiYinYang.Yin : ENineStarKiYinYang.Yang;
            }
            if (Energy == ENineStarKiEnergy.CoreEarth && RelatedEnergy != ENineStarKiEnergy.Unspecified)
            {
                return RelatedMetaData.YinYang;
            }
            return MetaData.YinYang;
        }

        private ENineStarKiModality GetModality()
        {
            switch (Energy)
            {
                case ENineStarKiEnergy.Water:
                case ENineStarKiEnergy.Wind:
                case ENineStarKiEnergy.Lake:
                    return ENineStarKiModality.Flexible;

                case ENineStarKiEnergy.Soil:
                case ENineStarKiEnergy.CoreEarth:
                case ENineStarKiEnergy.Mountain:
                    return ENineStarKiModality.Static;

                case ENineStarKiEnergy.Thunder:
                case ENineStarKiEnergy.Heaven:
                case ENineStarKiEnergy.Fire:
                    return ENineStarKiModality.Instigative;
            }

            return ENineStarKiModality.Unspecified;
        }

        private string GetChild()
        {
            switch (Energy)
            {
                case ENineStarKiEnergy.Water:
                    return Dictionary.water_child;

                case ENineStarKiEnergy.Soil:
                    return Dictionary.soil_child;

                case ENineStarKiEnergy.Thunder:
                    return Dictionary.thunder_child;

                case ENineStarKiEnergy.Wind:
                    return Dictionary.wind_child;

                case ENineStarKiEnergy.CoreEarth:
                    return Dictionary.coreearth_child;

                case ENineStarKiEnergy.Heaven:
                    return Dictionary.heaven_child;

                case ENineStarKiEnergy.Lake:
                    return Dictionary.lake_child;

                case ENineStarKiEnergy.Mountain:
                    return Dictionary.mountain_child;

                case ENineStarKiEnergy.Fire:
                    return Dictionary.fire_child;
            }

            return string.Empty;
        }

        private string GetTrigramDescription()
        {
            switch (Energy)
            {
                case ENineStarKiEnergy.Water:
                    return Dictionary.water_trigram;
                case ENineStarKiEnergy.Wind:
                    return Dictionary.wind_trigram;
                case ENineStarKiEnergy.Lake:
                    return Dictionary.lake_trigram;
                case ENineStarKiEnergy.Soil:
                    return Dictionary.soil_trigram;
                case ENineStarKiEnergy.CoreEarth:
                    return Dictionary.coreearth_trigram;
                case ENineStarKiEnergy.Mountain:
                    return Dictionary.mountain_trigram;
                case ENineStarKiEnergy.Thunder:
                    return Dictionary.thunder_trigram;
                case ENineStarKiEnergy.Heaven:
                    return Dictionary.heaven_trigram;
                case ENineStarKiEnergy.Fire:
                    return Dictionary.fire_trigram;
            }

            return String.Empty;
        }
    }
}