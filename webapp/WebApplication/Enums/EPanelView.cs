using K9.Base.DataAccessLayer.Attributes;

namespace K9.WebApplication.Enums
{
    public enum EPanelView
    {
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.FullTextView)]
        FullTextView,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.SummaryView)]
        SummaryView
    }
}
