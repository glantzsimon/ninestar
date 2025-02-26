using K9.WebApplication.Models;

namespace K9.WebApplication.Services
{
    public interface IChakraCodesService
    {
        ChakraCodesModel CalculateChakraCodes(ChakraCodesModel model);
        ChakraCodeForecast GetYearlyForecast(PersonModel personModel, int? offset = 0);
        ChakraCodeForecast GetMonthlyForecast(PersonModel personModel, int? offset = 0);
        ChakraCodeForecast GetDailyForecast(PersonModel personModel, int? offset = 0);
    }
}