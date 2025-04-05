using K9.Base.DataAccessLayer.Attributes;
using K9.Globalisation;

namespace K9.WebApplication.Enums
{
    public enum EScopeDisplay
    {
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.PersonalHouses)]
        PersonalKi,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.GlobalKi)]
        GlobalKi
    }
}
