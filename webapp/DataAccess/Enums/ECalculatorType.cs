using K9.Base.DataAccessLayer.Attributes;

namespace K9.DataAccessLayer.Enums
{
    public enum ECalculatorType
    {
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.Simple)]
        Simple,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.Advanced)]
        Advanced
    }
}
