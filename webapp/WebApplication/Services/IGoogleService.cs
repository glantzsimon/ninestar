using K9.Base.WebApplication.Models;

namespace K9.WebApplication.Services
{
    public interface IGoogleService : IBaseService
    {
        ServiceResult Authenticate(string googleIdToken);
    }
}