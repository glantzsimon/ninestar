using K9.WebApplication.ViewModels;
using System;

namespace K9.WebApplication.Services
{
    public interface IReportsService : IBaseService
    {
        YearlyReportViewModel GetYearlyReport(Guid? userId);
        YearlyReportViewModel GetYearlyReport(int userId);
    }
}