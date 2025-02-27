using K9.SharedLibrary.Extensions;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using System.Collections.Generic;

namespace K9.WebApplication.ViewModels
{
    public class NumerologyForecastViewModel
    {
        public NumerologyForecast Forecast { get; set; }

        public List<NumerologyPlannerModel> PlannerItems { get; set; }

        public string GetTitle(NumerologyPlannerModel model)
        {
            switch (Forecast.ForecastType)
            {
                case EForecastType.Yearly:
                    return model.EndDate.Year.ToString();

                case EForecastType.Monthly:
                    return model.Month.ToString("MMM");

                case EForecastType.Daily:
                    return model.StartDate.ToString("MMM d");

                default:
                    return "";
            }
        }

        public string GetFooter(NumerologyPlannerModel model)
        {
            switch (Forecast.ForecastType)
            {
                case EForecastType.Yearly:
                case EForecastType.Monthly:
                    return $"{model.StartDate.ToShortDateFormatString()} - {model.EndDate.ToShortDateFormatString()}";
                
                default:
                    return "";
            }
        }

        public string RazorId
        {
            get
            {
                switch (Forecast.ForecastType)
                {
                    case EForecastType.Yearly:
                        return "yearly-forecast";

                    case EForecastType.Monthly:
                        return "monthly-forecast";

                    case EForecastType.Daily:
                        return "daily-forecast";

                    default:
                        return "";
                }
            }
        }
    }
}