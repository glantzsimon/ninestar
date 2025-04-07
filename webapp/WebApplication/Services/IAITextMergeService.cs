using System.Threading.Tasks;

namespace K9.WebApplication.Services
{
    public interface IAITextMergeService
    {
        Task<string> MergeTextsAsync((string theme, string[] texts)[] groups, string extraPrompt = null);
        Task<string> MergeTextsIntoSummaryAsync((string theme, string[] texts)[] groups, string extraPrompt = null);
    }
}