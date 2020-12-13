using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;

namespace K9.WebApplication.Services
{
    public interface INineStarKiService
    {
        NineStarKiViewModel CalculateNineStarKi(PersonModel personModel);
    }
}