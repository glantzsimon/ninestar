﻿using System.ComponentModel.DataAnnotations;
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

    public enum ENineStarKiYinYang
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
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Water, Colour = ENineStarKiColour.White, Element = ENineStarKiElenement.Water, Direction = ENineStarKiDirection.North, FamilyMember = ENineStarKiFamilyMember.MiddleSon, YinYang = ENineStarKiYinYang.Yang)]
        Water,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Soil, Colour = ENineStarKiColour.Black, Element = ENineStarKiElenement.Earth, Direction = ENineStarKiDirection.SouthWest, FamilyMember = ENineStarKiFamilyMember.Mother, YinYang = ENineStarKiYinYang.Yin)]
        Soil,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Thunder, Colour = ENineStarKiColour.BrightGreen, Element = ENineStarKiElenement.Wood, Direction = ENineStarKiDirection.East, FamilyMember = ENineStarKiFamilyMember.EldestSon, YinYang = ENineStarKiYinYang.Yang)]
        Thunder,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Wind, Colour = ENineStarKiColour.Green, Element = ENineStarKiElenement.Wood, Direction = ENineStarKiDirection.SouthEast, FamilyMember = ENineStarKiFamilyMember.EldestDaughter, YinYang = ENineStarKiYinYang.Yin)]
        Wind,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.CoreEarth, Colour = ENineStarKiColour.Yellow, Element = ENineStarKiElenement.Earth, Direction = ENineStarKiDirection.Centre, FamilyMember = ENineStarKiFamilyMember.SeventhChild, YinYang = ENineStarKiYinYang.Unspecified)]
        CoreEarth,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Heaven, Colour = ENineStarKiColour.White, Element = ENineStarKiElenement.Metal, Direction = ENineStarKiDirection.NorthWest, FamilyMember = ENineStarKiFamilyMember.Father, YinYang = ENineStarKiYinYang.Unspecified)]
        Heaven,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Lake, Colour = ENineStarKiColour.Red, Element = ENineStarKiElenement.Metal, Direction = ENineStarKiDirection.West, FamilyMember = ENineStarKiFamilyMember.YoungestDaughter, YinYang = ENineStarKiYinYang.Yin)]
        Lake,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Mountain, Colour = ENineStarKiColour.White, Element = ENineStarKiElenement.Earth, Direction = ENineStarKiDirection.NorthEast, FamilyMember = ENineStarKiFamilyMember.YoungestSon, YinYang = ENineStarKiYinYang.Yang)]
        Mountain,
        [NineStarEnumMetaData(ResourceType = typeof(K9.Globalisation.Dictionary), Name = Strings.Names.Fire, Colour = ENineStarKiColour.Purple, Element = ENineStarKiElenement.Fire, Direction = ENineStarKiDirection.South, FamilyMember = ENineStarKiFamilyMember.MiddleDaughter, YinYang = ENineStarKiYinYang.Yin)]
        Fire
    }

    public class NineStarKiEnergy
    {
        public NineStarKiEnergy(ENineStarEnergy energy)
        {
            Energy = energy;
        }

        public ENineStarEnergy Energy { get; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.YinYangLabel)]
        public ENineStarKiYinYang YinYang => MetaData.YinYang;

        public NineStarEnumMetaDataAttribute MetaData => Energy.GetAttribute<NineStarEnumMetaDataAttribute>();

        public string FamilyMember => MetaData.GetFamilyMember();

        public string Element => MetaData.GetElement();

        public string Colour => MetaData.GetColour();

        public string Direction => MetaData.GetDirection();

    }
}