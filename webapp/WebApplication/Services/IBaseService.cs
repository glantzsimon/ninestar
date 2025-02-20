using K9.WebApplication.Packages;

namespace K9.WebApplication.Services
{
    public interface IBaseService
    {
        INineStarKiPackage Package { get; }
    }
}