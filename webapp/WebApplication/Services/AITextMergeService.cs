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

        public const string CrossReferenceText =
            "Where applicable, cross-reference the different thematic sections. For example, if in one section it discusses a period of rest and renewal for the 9-year cycle, but in the yearly cycle the energy is more active, then one would mention this, as both influences are relevant at the same time. Similarly, there can be different and seemingly opposing influences coming from different parts of the nine star ki personal chart, so again, where appropriate, it may be useful to cross-reference. Avoid prefacing the response (e.g., 'Here is...') — just return the final result as clean HTML";

        public const string DoNotAnnounceText =
            "Do not preface the output (e.g. 'Here is...'). Just return the clean HTML output only";

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

        public async Task<string> MergeTextsAsync(string[] inputTexts, string[] themes = null)
        {
            var joinedText = string.Join("\n\n", inputTexts);
            var groupingText = GetGroupingText(themes);

            var prompt = $"Blend the following texts into a clear, well-organized passage using only <h5> and <p> HTML tags. Group related ideas into {groupingText}, and maintain a {ElegantTone} tone throughout. {CrossReferenceText}. {DoNotAnnounceText}. :\n\n{joinedText}";

            return await ProcessRequest(prompt);
        }

        public async Task<string> MergeTextsIntoSummaryAsync(string[] inputTexts, string[] themes = null)
        {
            var joinedText = string.Join("\n\n", inputTexts);
            var groupingText = GetGroupingText(themes);

            var prompt = $"Merge the following texts into a single, well-structured summary using only 'h5', 'ul', and 'li' HTML tags. Group related ideas into {groupingText}. Within each section, compare positive and challenging aspects where relevant. The tone should be {ElegantTone} — clear, organized, and free-flowing, but without unnecessary explanation. {CrossReferenceText}. {DoNotAnnounceText}:\n\n{joinedText}";

            return await ProcessRequest(prompt);
        }

        private static string GetGroupingText(string[] groups)
        {
            var groupsList = string.Join(",", groups);
            var groupingText = groups != null && groups.Any()
                ? $"the following themed sections: {groupsList}"
                : "themed sections";
            return groupingText;
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