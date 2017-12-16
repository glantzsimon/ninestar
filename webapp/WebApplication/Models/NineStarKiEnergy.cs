using System.ComponentModel.DataAnnotations;
using K9.Base.DataAccessLayer.Attributes;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Attributes;

namespace K9.WebApplication.Models
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
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Wood)]
        Wood,
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

    public enum ENineStarKiGender
    {
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Unspecified)]
        Unspecified,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Yin)]
        Yin,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Yang)]
        Yang
    }

    public enum ENineStarEnergy
    {
        Unspecified,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Water, Colour = ENineStarKiColour.White, Element = ENineStarKiElenement.Water, Direction = ENineStarKiDirection.North, FamilyMember = ENineStarKiFamilyMember.MiddleSon, Gender = ENineStarKiGender.Yang)]
        Water,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Soil, Colour = ENineStarKiColour.Black, Element = ENineStarKiElenement.Earth, Direction = ENineStarKiDirection.SouthWest, FamilyMember = ENineStarKiFamilyMember.Mother, Gender = ENineStarKiGender.Yin)]
        Soil,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Thunder, Colour = ENineStarKiColour.BrightGreen, Element = ENineStarKiElenement.Wood, Direction = ENineStarKiDirection.East, FamilyMember = ENineStarKiFamilyMember.EldestSon, Gender = ENineStarKiGender.Yang)]
        Thunder,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Wind, Colour = ENineStarKiColour.Green, Element = ENineStarKiElenement.Wood, Direction = ENineStarKiDirection.SouthEast, FamilyMember = ENineStarKiFamilyMember.EldestDaughter, Gender = ENineStarKiGender.Yin)]
        Wind,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.CoreEarth, Colour = ENineStarKiColour.Yellow, Element = ENineStarKiElenement.Earth, Direction = ENineStarKiDirection.Centre, FamilyMember = ENineStarKiFamilyMember.SeventhChild, Gender = ENineStarKiGender.Unspecified)]
        CoreEarth,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Heaven, Colour = ENineStarKiColour.White, Element = ENineStarKiElenement.Metal, Direction = ENineStarKiDirection.NorthWest, FamilyMember = ENineStarKiFamilyMember.Father, Gender = ENineStarKiGender.Unspecified)]
        Heaven,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Lake, Colour = ENineStarKiColour.Red, Element = ENineStarKiElenement.Metal, Direction = ENineStarKiDirection.West, FamilyMember = ENineStarKiFamilyMember.YoungestDaughter, Gender = ENineStarKiGender.Yin)]
        Lake,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Mountain, Colour = ENineStarKiColour.White, Element = ENineStarKiElenement.Earth, Direction = ENineStarKiDirection.NorthEast, FamilyMember = ENineStarKiFamilyMember.YoungestSon, Gender = ENineStarKiGender.Yang)]
        Mountain,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Fire, Colour = ENineStarKiColour.Purple, Element = ENineStarKiElenement.Fire, Direction = ENineStarKiDirection.South, FamilyMember = ENineStarKiFamilyMember.MiddleDaughter, Gender = ENineStarKiGender.Yin)]
        Fire
    }

    public class NineStarKiEnergy
    {
        public ENineStarEnergy Energy { get; set; }

        public int EnergyNumber => (int)Energy;

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.GenderLabel)]
        public ENineStarKiGender NineStarKiGender { get; set; }

        public NineStarEnumMetaDataAttribute MetaData => NineStarKiGender.GetAttribute<NineStarEnumMetaDataAttribute>();

        public string YinYang => MetaData.GetGender();

        public string FamilyMember => MetaData.GetFamilyMember();

        public string Element => MetaData.GetElement();

        public string Colour => MetaData.GetColour();

        public string Direction => MetaData.GetDirection();

    }
}