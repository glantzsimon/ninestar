using System.Threading.Tasks;

namespace K9.WebApplication.Services
{
    public interface IMediaService : IBaseService
    {
        string GetBaseMediaPath();
        Task CheckImageServiceHealthAsync();
        void ScheduledHealthCheck();
    }
}