using System;
using System.Threading.Tasks;

namespace K9.WebApplication.Services
{
    public interface IReportsService : IBaseService
    {
        Task<string> GetYearlyReport(Guid userId);
        Task<string> GetYearlyReport(int userId);
    }
}