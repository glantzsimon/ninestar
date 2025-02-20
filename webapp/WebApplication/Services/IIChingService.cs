using K9.WebApplication.Models;

namespace K9.WebApplication.Services
{
    public interface IIChingService : IBaseService
    {
        Hexagram GenerateHexagram();
    }
}