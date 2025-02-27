using K9.SharedLibrary.Extensions;
using K9.WebApplication.Attributes;
using K9.WebApplication.Enums;
using K9.WebApplication.Extensions;

namespace K9.WebApplication.Models
{
    public class NumerologyForecast
    {
        public ENumerologyCode ChartCode { get; set; }

        public EForecastType ForecastType { get; set; }

        public int TopNumber { get; set; }

        public int BottomNumber { get; set; }

        public int? RowNumber { get; set; }
        
        public string Name => Attributes.Name;

        public string Forecast => Attributes.ResourceType.GetValueFromResource(ForecastName) ?? string.Empty;

        public string ChartCodesTitle => GetForecastTitle();

        public string ForecastPanelId => GetForecastPanelId();

        private string GetForecastPanelId()
        {
            switch (ForecastType)
            {
                case EForecastType.Daily:
                    return "daily-forecast";

                case EForecastType.Monthly:
                    return "monthly-forecast";

                default:
                    return "yearly-forecast";
            }
        }

        private int RowNumberCalculated => RowNumber ?? (TopNumber.Decrement((int)ChartCode)).ToNumerology();

        private string ForecastName => $"_{(int)ChartCode}_{RowNumberCalculated}";

        private NumerologyCodeEnumMetaDataAttribute Attributes => ChartCode.GetAttribute<NumerologyCodeEnumMetaDataAttribute>();

        private string GetForecastTitle()
        {
            return RowNumber == null
                ? $"{(int) ChartCode} - {TopNumber}/{BottomNumber}"
                : $"{(int) ChartCode} - {CalculateDailyNumbers()}";
        }

        private string CalculateDailyNumbers()
        {
            var chartCodeNumber = (int)ChartCode;
            var topNumber = (RowNumber.Value + chartCodeNumber).ToNumerology();
            var bottomNumber = (RowNumber.Value + (chartCodeNumber * 2)).ToNumerology();

            return $"{topNumber}/{bottomNumber}";
        }
    }
}