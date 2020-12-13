using K9.WebApplication.Models;

namespace K9.WebApplication.Services
{
    public class NineStarKiService : INineStarKiService
    {
        public NineStarKiModel CalculateNineStarKi(PersonModel personModel)
        {
            return new NineStarKiModel(personModel);
        }
    }
}