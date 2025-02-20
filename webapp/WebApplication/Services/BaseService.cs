using K9.WebApplication.Packages;

namespace K9.WebApplication.Services
{
    public class BaseService : IBaseService
    {
        public INineStarKiBasePackage Package { get; }

        public BaseService(INineStarKiBasePackage package)
        {
            Package = package;
        }
    }
}