using K9.Base.DataAccessLayer.Attributes;

namespace K9.DataAccessLayer.Enums
{
    public enum EEnergyType
    {
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = K9.Globalisation.Strings.Names.MainEnergy)]
        MainEnergy,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = K9.Globalisation.Strings.Names.CharacterEnergy)]
        CharacterEnergy,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = K9.Globalisation.Strings.Names.RisingEnergy)]
        RisingEnergy,
    }
}
