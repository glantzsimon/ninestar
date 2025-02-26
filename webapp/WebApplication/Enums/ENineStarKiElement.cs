using K9.Base.DataAccessLayer.Attributes;
using K9.Globalisation;
using K9.WebApplication.Attributes;

namespace K9.WebApplication.Enums
{
    public enum ENineStarKiElement
    {
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Water)]
        [ENineStarKiElementEnumMetaData(ResourceType = typeof(Dictionary),
            Name = Strings.Names.Water,
            Season = ESeason.Winter,
            Element = Water,
            StrongYinOrgans = EOrgan.Kidneys,
            StrongYangOrgans = EOrgan.UrinaryBladder,
            WeakYinOrgans = new EOrgan[] { EOrgan.SpleenPancreas, EOrgan.Heart },
            WeakYangOrgans = new EOrgan[] { EOrgan.Stomach, EOrgan.SmallIntestine })]
        Water,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Earth)]
        [ENineStarKiElementEnumMetaData(ResourceType = typeof(Dictionary),
            Name = Strings.Names.Earth,
            Element = Earth,
            Season = ESeason.LateSummer,
            StrongYinOrgans = EOrgan.SpleenPancreas,
            StrongYangOrgans = EOrgan.Stomach,
            WeakYinOrgans = new EOrgan[] { EOrgan.Kidneys, EOrgan.Liver },
            WeakYangOrgans = new EOrgan[] { EOrgan.UrinaryBladder, EOrgan.GallBladder })]
        Earth,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Tree)]
        [ENineStarKiElementEnumMetaData(ResourceType = typeof(Dictionary),
            Name = Strings.Names.Tree,
            Element = Tree,
            Season = ESeason.Spring,
            StrongYinOrgans = EOrgan.GallBladder,
            StrongYangOrgans = EOrgan.Liver,
            WeakYinOrgans = new EOrgan[] { EOrgan.SpleenPancreas, EOrgan.Lungs },
            WeakYangOrgans = new EOrgan[] { EOrgan.Stomach, EOrgan.LargeIntestine })]
        Tree,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Metal)]
        [ENineStarKiElementEnumMetaData(ResourceType = typeof(Dictionary),
            Name = Strings.Names.Metal,
            Element = Metal,
            Season = ESeason.Autumn,
            StrongYinOrgans = EOrgan.Lungs,
            StrongYangOrgans = EOrgan.LargeIntestine,
            WeakYinOrgans = new EOrgan[] { EOrgan.SpleenPancreas, EOrgan.Liver },
            WeakYangOrgans = new EOrgan[] { EOrgan.Stomach, EOrgan.GallBladder })]
        Metal,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Names.Fire)]
        [ENineStarKiElementEnumMetaData(ResourceType = typeof(Dictionary),
            Name = Strings.Names.Fire,
            Element = Fire,
            Season = ESeason.Summer,
            StrongYinOrgans = EOrgan.Heart,
            StrongYangOrgans = EOrgan.SmallIntestine,
            WeakYinOrgans = new EOrgan[] { EOrgan.Kidneys, EOrgan.Lungs },
            WeakYangOrgans = new EOrgan[] { EOrgan.UrinaryBladder, EOrgan.LargeIntestine })]
        Fire
    }
}
