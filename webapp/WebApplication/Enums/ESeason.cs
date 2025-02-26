using K9.Base.DataAccessLayer.Attributes;

namespace K9.WebApplication.Enums
{
    public enum ESeason
    {
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.Spring)]
        Spring = 1,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.Summer)]
        Summer,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.LateSummer)]
        LateSummer,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.Autumn)]
        Autumn,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.Winter)]
        Winter
    }
}
