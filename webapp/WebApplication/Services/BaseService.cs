using K9.WebApplication.Packages;

namespace K9.WebApplication.Services
{
    public class BaseService : IBaseService
    {
        public INineStarKiBasePackage My { get; }

        public BaseService(INineStarKiBasePackage package)
        {
            My = package;
        }
    }
}