using K9.WebApplication.Models;

namespace K9.WebApplication.Services
{
    public interface INumerologyService
    {
        NumerologyModel Calculate(NumerologyModel model);
        NumerologyForecast GetYearlyForecast(PersonModel personModel, int? offset = 0);
        NumerologyForecast GetMonthlyForecast(PersonModel personModel, int? offset = 0);
        NumerologyForecast GetDailyForecast(PersonModel personModel, int? offset = 0);
    }
}