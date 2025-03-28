using K9.Base.DataAccessLayer.Attributes;
using K9.Globalisation;

namespace K9.WebApplication.Enums
{
    public enum EPlannerView
    {
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.EightyOneYearView)]
        EightyOneYear,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.NineYearView)]
        NineYear,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.YearView)]
        Year,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.MonthView)]
        Month,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.DayView)]
        Day
    }
}
