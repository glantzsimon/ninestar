using K9.Base.DataAccessLayer.Attributes;

namespace K9.WebApplication.Enums
{
    public enum ECalculationMethod
    {
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.TraditionalNineStarKi)]
        Traditional = 2,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.InvertYinYearsForPersonalChart)]
        InvertYinYearsForPersonalChart = 1,
        [EnumDescription(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.InvertYinYearsForPersonalChartAndCycles)]
        InvertYinYearsForPersonalChartAndCycles = 0
    }
}
