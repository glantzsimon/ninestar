using K9.WebApplication.Packages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace K9.WebApplication.Services
{
    public class AITextMergeService : BaseService, IAITextMergeService
    {
        public const string ElegantTone = "elegant and refined, yet succint";
        
        private readonly HttpClient _httpClient;
        
        public AITextMergeService(INineStarKiBasePackage my)
            : base(my)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {My.ApiConfiguration.OpenRouterApiKey}");
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", $"https://{My.DefaultValuesConfiguration.SiteBaseUrl}");
        }

        public async Task<string> MergeTextsAsync(string[] inputTexts, string tone = ElegantTone)
        {
            var joinedText = string.Join("\n\n", inputTexts);
            var prompt = $"Please blend the following texts into a well-structured and organised text. Use h5 and p tags (html) and keep the {tone}. Maintain clarity and logical flow:\n\n{joinedText}";

            var requestBody = new
            {
                model = My.ApiConfiguration.OpenRouterModel,
                messages = new[]
                {
                    new {
                        role = "user",
                        content = prompt
                    }
                }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(My.ApiConfiguration.OpenRouterEndpoint, content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var parsed = JObject.Parse(responseString);

            return parsed["choices"]?[0]?["message"]?["content"]?.ToString()?.Trim() ?? "[No response]";
        }

        public async Task<string> MergeTextsIntoSummaryAsync(string[] inputTexts, string tone = ElegantTone)
        {
            var joinedText = string.Join("\n\n", inputTexts);
            var prompt = $"Please blend the following texts together and output a single, well-structured summary using 'ul' and 'li' and h5 tags where appropriate. Try to group things together logically into 'Gifts' and 'Challenges' and 'Best Practices'. The tone should be {tone}e. Maintain clarity and logical flow, even where contradictory information seems to exist. No need to start by declaring what you are doing. Just stick to the result - so that it's presentable. :\n\n{joinedText}";

            var requestBody = new
            {
                model = My.ApiConfiguration.OpenRouterModel,
                messages = new[]
                {
                    new {
                        role = "user",
                        content = prompt
                    }
                }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(My.ApiConfiguration.OpenRouterEndpoint, content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var parsed = JObject.Parse(responseString);

            return parsed["choices"]?[0]?["message"]?["content"]?.ToString()?.Trim() ?? "[No response]";
        }
    }
}