using K9.Base.DataAccessLayer.Attributes;

namespace K9.WebApplication.Enums
{
    public enum EHousesDisplay
    {
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.SolarHouses)]
        SolarHouse,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.LunarHouses)]
        LunarHouse,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.IsometricHouses)]
        Isometric
    }
}
