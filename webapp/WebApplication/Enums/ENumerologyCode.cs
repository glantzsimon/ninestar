using K9.Globalisation;
using K9.WebApplication.Attributes;

namespace K9.WebApplication.Enums
{
    public enum ENumerologyCode
    {
        Unspecified,
        [NumerologyCodeEnumMetaData(ResourceType = typeof(Dictionary),
            NameKey = "PioneerTitle",
            DescriptionKey = "pioneer1",
            Colour = "#833135",
            PurposeKey = "PioneerPurpose")]
        Pioneer,
        [NumerologyCodeEnumMetaData(ResourceType = typeof(Dictionary),
            NameKey = "PeacemakerTitle",
            DescriptionKey = "peacemaker",
            Colour = "#d68258",
            PurposeKey = "PeacemakerPurpose")]
        Peacemaker,
        [NumerologyCodeEnumMetaData(ResourceType = typeof(Dictionary),
            NameKey = "RevolutionaryTitle",
            DescriptionKey = "revolutionary",
            Colour = "#e3c570",
            PurposeKey = "RevolutionaryPurpose")]
        Revolutionary,
        [NumerologyCodeEnumMetaData(ResourceType = typeof(Dictionary),
            NameKey = "HealerTitle",
            DescriptionKey = "healer",
            Colour = "#496553",
            PurposeKey = "HealerPurpose")]
        Healer,
        [NumerologyCodeEnumMetaData(ResourceType = typeof(Dictionary),
            NameKey = "VisionaryTitle",
            DescriptionKey = "visionary",
            Colour = "#1e506d",
            PurposeKey = "VisionaryPurpose")]
        Visionary,
        [NumerologyCodeEnumMetaData(ResourceType = typeof(Dictionary),
            NameKey = "ManifestorTitle",
            DescriptionKey = "manifestor",
            Colour = "#4a4d69",
            PurposeKey = "ManifestorPurpose")]
        Manifestor,
        [NumerologyCodeEnumMetaData(ResourceType = typeof(Dictionary),
            NameKey = "MentorTitle",
            DescriptionKey = "mentor",
            Colour = "#f2e9e4",
            PurposeKey = "MentorPurpose")]
        Mentor,
        [NumerologyCodeEnumMetaData(ResourceType = typeof(Dictionary),
            NameKey = "MysticTitle",
            DescriptionKey = "mystic",
            Colour = "#c6a671",
            PurposeKey = "MysticPurpose")]
        Mystic,
        [NumerologyCodeEnumMetaData(ResourceType = typeof(Dictionary),
            NameKey = "ElderTitle",
            DescriptionKey = "elder",
            Colour = "#e5e5e5",
            PurposeKey = "ElderPurpose")]
        Elder
    }

}