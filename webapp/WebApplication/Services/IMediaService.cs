using System.Threading.Tasks;

namespace K9.WebApplication.Services
{
    public interface IMediaService : IBaseService
    {
        Task CheckImageServiceHealthAsync();
        void ScheduledHealthCheck();
    }
}