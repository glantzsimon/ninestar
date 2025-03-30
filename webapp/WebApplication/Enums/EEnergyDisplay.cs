using K9.Base.DataAccessLayer.Attributes;
using K9.Globalisation;

namespace K9.WebApplication.Enums
{
    public enum EEnergyDisplay
    {
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.Graphical)]
        Graphical,
        [EnumDescription(ResourceType = typeof(Dictionary), Name = Strings.Labels.MagicSquare)]
        MagicSquare
    }
}
