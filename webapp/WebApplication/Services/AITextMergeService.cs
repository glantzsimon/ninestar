using K9.WebApplication.Packages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace K9.WebApplication.Services
{
    public class AITextMergeService : BaseService, IAITextMergeService
    {
        public const string ElegantTone = "elegant and refined, yet succint";

        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly string _endpoint;

        public AITextMergeService(INineStarKiBasePackage my)
            : base(my)
        {
            _httpClient = new HttpClient();
            _apiKey = My.ApiConfiguration.OpenAIApiKey;
            _model = My.ApiConfiguration.OpenApiModel;
            _endpoint = My.ApiConfiguration.OpenApiEndpoint;

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            //_httpClient.DefaultRequestHeaders.Add("HTTP-Referer", $"https://{My.DefaultValuesConfiguration.SiteBaseUrl}");
        }

        public async Task<string> MergeTextsAsync(string[] inputTexts, string tone = ElegantTone, string[] groups = null)
        {
            var joinedText = string.Join("\n\n", inputTexts);
            var groupsList = string.Join(",", groups);
            var groupingText = groups != null && groups.Any() ? $"the following themed sections: {groupsList}" : "themed sections";

            var prompt = $"Blend the following texts into a clear, well-organized passage using only <h5> and <p> HTML tags. Group related ideas into {groupingText}, and maintain a {tone} tone throughout. Avoid prefacing the response (e.g., 'Here is...') — just return the final result as clean HTML:\n\n{joinedText}";

            return await ProcessRequest(prompt);
        }

        public async Task<string> MergeTextsIntoSummaryAsync(string[] inputTexts, string tone = ElegantTone)
        {
            var joinedText = string.Join("\n\n", inputTexts);
            var prompt = $"Merge the following texts into a single, well-structured summary using only 'h5', 'ul', and 'li' HTML tags. Group related points into thematic sections. Within each section, compare positive and challenging aspects where relevant. The tone should be {tone} — clear, organized, and free-flowing, but without unnecessary explanation. Do not preface the output (e.g. 'Here is...'). Just return the clean HTML output only:\n\n{joinedText}";

            return await ProcessRequest(prompt);
        }

        private async Task<string> ProcessRequest(string prompt)
        {
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_endpoint, content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var parsed = JObject.Parse(responseString);

            return StripMarkdownCodeFencing(parsed["choices"]?[0]?["message"]?["content"]?.ToString()?.Trim() ??
                                            "[No response]");
        }

        private static string StripMarkdownCodeFencing(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var lines = input.Trim().Split('\n').ToList();

            if (lines.Count >= 2)
            {
                if (lines[0].Trim().StartsWith("```"))
                    lines.RemoveAt(0);

                if (lines[lines.Count - 1].Trim().StartsWith("```"))
                    lines.RemoveAt(lines.Count - 1);
            }

            return string.Join("\n", lines).Trim();
        }

    }
}