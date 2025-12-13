using System;
using System.Threading.Tasks;

namespace K9.WebApplication.Services
{
    public interface IAIService
    {
        Task<string> MergeTextsAsync((string theme, string[] texts)[] groups, string extraPrompt = null);
        Task<string> MergeTextsIntoSummaryAsync((string theme, string[] texts)[] groups, string extraPrompt = null);
        Task<string> GetYearlyReport(DateTime dateOfBirth, int year);
    }
}