using K9.Base.DataAccessLayer.Attributes;

namespace K9.WebApplication.Enums
{
    public enum ESection
    {
        Unspecified,
        [EnumDescription(CultureCode = "p")]
        Profile,
        [EnumDescription(CultureCode = "c")]
        Compatibility,
        [EnumDescription(CultureCode = "pr")]
        Predictions,
        [EnumDescription(CultureCode = "b")]
        Biorhythms,
        [EnumDescription(CultureCode = "k")]
        KnowledgeBase,
        [EnumDescription(CultureCode = "n")]
        Numerology
    }
}
