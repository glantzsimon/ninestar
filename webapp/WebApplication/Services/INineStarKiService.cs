using K9.WebApplication.Models;

namespace K9.WebApplication.Services
{
    public interface INineStarKiService
    {
        NineStarKiModel Calculate(PersonModel model);
    }
}