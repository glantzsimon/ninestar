using K9.Base.DataAccessLayer.Attributes;

namespace K9.WebApplication.Enums
{
    public enum EOrgan
    {
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.SpleenPancreas)]
        SpleenPancreas = 1,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.Lungs)]
        Lungs,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.Kidneys)]
        Kidneys,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.Liver)]
        Liver,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.Heart)]
        Heart,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.Stomach)]
        Stomach,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.LargeIntestine)]
        LargeIntestine,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.UrinaryBladder)]
        UrinaryBladder,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.Gallbladder)]
        GallBladder,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.SmallIntestine)]
        SmallIntestine
    }
}
