using K9.Base.DataAccessLayer.Attributes;

namespace K9.WebApplication.Enums
{
    public enum EHousesDisplay
    {
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.DisplayPrimaryHouses)]
        SolarHouse,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.DisplaySecondaryHouses)]
        LunarHouse,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.DisplayPrimaryAndSecondaryHouses)]
        SolarAndLunarHouses,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.DisplayIsometricHouses)]
        Isometric
    }
}
