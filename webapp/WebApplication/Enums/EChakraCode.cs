using K9.Globalisation;
using K9.WebApplication.Attributes;

namespace K9.WebApplication.Enums
{
    public enum EChakraCode
    {
        Unspecified,
        [ChakraCodeEnumMetaData(ResourceType = typeof(Dictionary),
            NameKey = "PioneerTitle",
            DescriptionKey = "pioneer1",
            Colour = "#833135",
            PurposeKey = "PioneerPurpose")]
        Pioneer,
        [ChakraCodeEnumMetaData(ResourceType = typeof(Dictionary),
            NameKey = "PeacemakerTitle",
            DescriptionKey = "peacemaker",
            Colour = "#d68258",
            PurposeKey = "PeacemakerPurpose")]
        Peacemaker,
        [ChakraCodeEnumMetaData(ResourceType = typeof(Dictionary),
            NameKey = "WarriorTitle",
            DescriptionKey = "warrior",
            Colour = "#e3c570",
            PurposeKey = "WarriorPurpose")]
        Warrior,
        [ChakraCodeEnumMetaData(ResourceType = typeof(Dictionary),
            NameKey = "HealerTitle",
            DescriptionKey = "healer",
            Colour = "#496553",
            PurposeKey = "HealerPurpose")]
        Healer,
        [ChakraCodeEnumMetaData(ResourceType = typeof(Dictionary),
            NameKey = "AkashicTitle",
            DescriptionKey = "akashic",
            Colour = "#1e506d",
            PurposeKey = "AkashicPurpose")]
        Akashic,
        [ChakraCodeEnumMetaData(ResourceType = typeof(Dictionary),
            NameKey = "ManifestorTitle",
            DescriptionKey = "manifestor",
            Colour = "#4a4d69",
            PurposeKey = "ManifestorPurpose")]
        Manifestor,
        [ChakraCodeEnumMetaData(ResourceType = typeof(Dictionary),
            NameKey = "RoyalTitle",
            DescriptionKey = "royal",
            Colour = "#f2e9e4",
            PurposeKey = "RoyalPurpose")]
        Royal,
        [ChakraCodeEnumMetaData(ResourceType = typeof(Dictionary),
            NameKey = "MysticTitle",
            DescriptionKey = "mystic",
            Colour = "#c6a671",
            PurposeKey = "MysticPurpose")]
        Mystic,
        [ChakraCodeEnumMetaData(ResourceType = typeof(Dictionary),
            NameKey = "ElderTitle",
            DescriptionKey = "elder",
            Colour = "#e5e5e5",
            PurposeKey = "ElderPurpose")]
        Elder
    }

}