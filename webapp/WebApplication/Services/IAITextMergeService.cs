using System.Threading.Tasks;

namespace K9.WebApplication.Services
{
    public interface IAITextMergeService
    {
        Task<string> MergeTextsAsync(string[] inputTexts, string tone = "elegant and refined, yet succint", string[] groups = null);
        Task<string> MergeTextsIntoSummaryAsync(string[] inputTexts, string tone = "elegant and refined, yet succint", string[] groups = null);
    }
}