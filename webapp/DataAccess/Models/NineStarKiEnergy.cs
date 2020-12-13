using System.ComponentModel.DataAnnotations;
using K9.Base.DataAccessLayer.Attributes;
using K9.DataAccessLayer.Attributes;
using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Extensions;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;

namespace K9.DataAccessLayer.Models
{
    public enum ENineStarKiColour
    {
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.White)]
        White,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Black)]
        Black,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.BrightGreen)]
        BrightGreen,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Green)]
        Green,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Yellow)]
        Yellow,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Red)]
        Red,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Purple)]
        Purple
    }

    public enum ENineStarKiElenement
    {
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Water)]
        Water,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Earth)]
        Earth,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Tree)]
        Tree,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Metal)]
        Metal,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Fire)]
        Fire
    }

    public enum ENineStarKiFamilyMember
    {
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.MiddleSon)]
        MiddleSon,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Mother)]
        Mother,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.EldestSon)]
        EldestSon,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.EldestDaughter)]
        EldestDaughter,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.SeventhChild)]
        SeventhChild,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Father)]
        Father,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.YoungestDaughter)]
        YoungestDaughter,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.YoungestSon)]
        YoungestSon,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.MiddleDaughter)]
        MiddleDaughter
    }

    public enum ENineStarKiDirection
    {
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Unspecified)]
        Centre,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.North)]
        North,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.NorthWest)]
        NorthWest,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.West)]
        West,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.SouthWest)]
        SouthWest,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.South)]
        South,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.SouthEast)]
        SouthEast,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.East)]
        East,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.NorthEast)]
        NorthEast
    }

    public enum ENineStarKiYinYang
    {
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Unspecified)]
        Unspecified,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Yin)]
        Yin,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Yang)]
        Yang
    }

    public enum ENineStarKiModality
    {
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Unspecified)]
        Unspecified,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Flexible)]
        Flexible,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Static)]
        Static,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Instigative)]
        Instigative
    }

    public enum ENineStarEnergy
    {
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Unspecified)]
        Unspecified,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Water, Colour = ENineStarKiColour.White, Element = ENineStarKiElenement.Water, Direction = ENineStarKiDirection.North, FamilyMember = ENineStarKiFamilyMember.MiddleSon, YinYang = ENineStarKiYinYang.Yang, TrigramName = "Kan")]
        Water,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Soil, Colour = ENineStarKiColour.Black, Element = ENineStarKiElenement.Earth, Direction = ENineStarKiDirection.SouthWest, FamilyMember = ENineStarKiFamilyMember.Mother, YinYang = ENineStarKiYinYang.Yin, TrigramName = "Kun")]
        Soil,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Thunder, Colour = ENineStarKiColour.BrightGreen, Element = ENineStarKiElenement.Tree, Direction = ENineStarKiDirection.East, FamilyMember = ENineStarKiFamilyMember.EldestSon, YinYang = ENineStarKiYinYang.Yang, TrigramName = "Chen")]
        Thunder,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Wind, Colour = ENineStarKiColour.Green, Element = ENineStarKiElenement.Tree, Direction = ENineStarKiDirection.SouthEast, FamilyMember = ENineStarKiFamilyMember.EldestDaughter, YinYang = ENineStarKiYinYang.Yin, TrigramName = "Sun")]
        Wind,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.CoreEarth, Colour = ENineStarKiColour.Yellow, Element = ENineStarKiElenement.Earth, Direction = ENineStarKiDirection.Centre, FamilyMember = ENineStarKiFamilyMember.SeventhChild, YinYang = ENineStarKiYinYang.Unspecified, TrigramName = "None")]
        CoreEarth,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Heaven, Colour = ENineStarKiColour.White, Element = ENineStarKiElenement.Metal, Direction = ENineStarKiDirection.NorthWest, FamilyMember = ENineStarKiFamilyMember.Father, YinYang = ENineStarKiYinYang.Unspecified, TrigramName = "Chien")]
        Heaven,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Lake, Colour = ENineStarKiColour.Red, Element = ENineStarKiElenement.Metal, Direction = ENineStarKiDirection.West, FamilyMember = ENineStarKiFamilyMember.YoungestDaughter, YinYang = ENineStarKiYinYang.Yin, TrigramName = "Tui")]
        Lake,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Mountain, Colour = ENineStarKiColour.White, Element = ENineStarKiElenement.Earth, Direction = ENineStarKiDirection.NorthEast, FamilyMember = ENineStarKiFamilyMember.YoungestSon, YinYang = ENineStarKiYinYang.Yang, TrigramName = "Ken")]
        Mountain,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Fire, Colour = ENineStarKiColour.Purple, Element = ENineStarKiElenement.Fire, Direction = ENineStarKiDirection.South, FamilyMember = ENineStarKiFamilyMember.MiddleDaughter, YinYang = ENineStarKiYinYang.Yin, TrigramName = "Li")]
        Fire
    }

    public class NineStarKiEnergy
    {
        public NineStarKiEnergy(ENineStarEnergy energy)
        {
            Energy = energy;
        }

        public ENineStarEnergy Energy { get; }

        /// <summary>
        /// Used to determine YinYang of 5 energies
        /// </summary>
        public ENineStarEnergy RelatedEnergy { get; set; }

        /// <summary>
        /// Used to determine YinYang of 5.5.5 energies
        /// </summary>
        public EGender Gender { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.EnergyLabel)]
        public string EnergyName => MetaData.GetDescription();

        public int EnergyNumber => (int)Energy;

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.YinYangLabel)]
        public ENineStarKiYinYang YinYang => GetYinYang();

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.FamilyMemberLabel)]
        public string FamilyMember => MetaData.GetFamilyMember();

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.FamilyMemberLabel)]
        public string Trigram => MetaData.GetTrigram();

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.FamilyMemberLabel)]
        public string Element => MetaData.GetElement();

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.ElementLabel)]
        public string ElementDescription => MetaData.GetElement();
        
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.ColourLabel)]
        public string Colour => MetaData.GetColour();

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.ModalityLabel)]
        public ENineStarKiModality Modality => GetModality();

        public string Direction => MetaData.GetDirection();

        private NineStarEnumMetaDataAttribute MetaData => Energy.GetAttribute<NineStarEnumMetaDataAttribute>();
        private NineStarEnumMetaDataAttribute RelatedMetaData => RelatedEnergy.GetAttribute<NineStarEnumMetaDataAttribute>();

        private ENineStarKiYinYang GetYinYang()
        {
            if (Energy == ENineStarEnergy.CoreEarth && RelatedEnergy == ENineStarEnergy.CoreEarth)
            {
                return Gender.IsYin() ? ENineStarKiYinYang.Yin : ENineStarKiYinYang.Yang;
            }
            if (Energy == ENineStarEnergy.CoreEarth && RelatedEnergy != ENineStarEnergy.Unspecified)
            {
                return RelatedMetaData.YinYang;
            }
            return MetaData.YinYang;
        }

        private ENineStarKiModality GetModality()
        {
            switch (Energy)
            {
                case ENineStarEnergy.Water:
                case ENineStarEnergy.Wind:
                case ENineStarEnergy.Lake:
                    return ENineStarKiModality.Flexible;

                case ENineStarEnergy.Soil:
                case ENineStarEnergy.CoreEarth:
                case ENineStarEnergy.Mountain:
                    return ENineStarKiModality.Static;

                case ENineStarEnergy.Thunder:
                case ENineStarEnergy.Heaven:
                case ENineStarEnergy.Fire:
                    return ENineStarKiModality.Instigative;
            }

            return ENineStarKiModality.Unspecified;
        }

    }
}