using System.Threading.Tasks;

namespace K9.WebApplication.Services
{
    public interface IAITextMergeService
    {
        Task<string> MergeTextsAsync(string[] inputTexts, string[] themes = null);
        Task<string> MergeTextsIntoSummaryAsync(string[] inputTexts, string[] themes = null);
    }
}