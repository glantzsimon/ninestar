using K9.Base.DataAccessLayer.Attributes;

namespace K9.DataAccessLayer.Enums
{
    public enum EGender
    {
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = K9.Globalisation.Strings.Names.Female)]
        Female,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = K9.Globalisation.Strings.Names.Male)]
        Male,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = K9.Globalisation.Strings.Names.TransFemale)]
        TransFemale,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = K9.Globalisation.Strings.Names.TransMale)]
        TransMale,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = K9.Globalisation.Strings.Names.Hermaphrodite)]
        Hermaphrodite,
        [EnumDescription(ResourceType = typeof(K9.Globalisation.Dictionary), Name = K9.Globalisation.Strings.Names.Other)]
        Other
    }
}
