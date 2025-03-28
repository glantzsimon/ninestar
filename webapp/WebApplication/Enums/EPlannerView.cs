using K9.Base.DataAccessLayer.Attributes;
using K9.Globalisation;

namespace K9.WebApplication.Enums
{
    public enum EPlannerView
    {
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.EightyOneYearPeriod)]
        EightyOneYear,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.NineYearPeriod)]
        NineYear,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.Year)]
        Year,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.Month)]
        Month,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.Day)]
        Day
    }
}
