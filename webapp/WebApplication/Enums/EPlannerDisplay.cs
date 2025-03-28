using K9.Base.DataAccessLayer.Attributes;
using K9.Globalisation;

namespace K9.WebApplication.Enums
{
    public enum EPlannerDisplay
    {
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.EightyOneYearView)]
        PersonalKi,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.NineYearView)]
        GlobalKi
    }
}
