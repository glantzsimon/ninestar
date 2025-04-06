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
        private const string ElegantTone = "elegant and refined, yet succinct";

        private static string WritingStyle = $"Maintain an {ElegantTone} tone throughout and keep the writing clear, organized, and free-flowing, but without unnecessary explanation.";

        private const string CrossReferenceText =
            "Where applicable, cross-reference different influences. For example, if the yearly cycle suggests rest but the 9-year cycle suggests activity, mention both. This also applies to different influences from the same chart.";

        private const string DoNotAnnounceText =
            "Do not preface the output (e.g., 'Here is...'). Just return clean HTML output only.";

        private const string PositiveAndChallengingText =
            "Within each section, compare positive and challenging aspects where relevant, and add sub-themes if it helps organize the section better.";

        private const string OrganisationText =
            "Organize the text into one or more thematic groups, each introduced with an appropriate heading.";

        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly string _endpoint;

        public AITextMergeService(INineStarKiBasePackage my) : base(my)
        {
            _httpClient = new HttpClient();
            _apiKey = My.ApiConfiguration.OpenAIApiKey;
            _model = My.ApiConfiguration.OpenApiModel;
            _endpoint = My.ApiConfiguration.OpenApiEndpoint;

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<string> MergeTextsAsync((string theme, string[] texts)[] groups)
        {
            var prompt = GetPrompt(
                "Blend the following texts into a clear, well-organized passage using only <h5> and <p> HTML tags.",
                groups
            );
            return await ProcessRequest(prompt);
        }

        public async Task<string> MergeTextsIntoSummaryAsync((string theme, string[] texts)[] groups)
        {
            var prompt = GetPrompt(
                "Blend the following texts into a clear, well-organized summary using only <h5>, <ul>, and <li> HTML tags.",
                groups
            );
            return await ProcessRequest(prompt);
        }

        private static string GetPrompt(string intro, (string theme, string[] texts)[] groups)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{intro} {WritingStyle} {PositiveAndChallengingText} {CrossReferenceText} {DoNotAnnounceText} {OrganisationText}");

            var i = 1;
            foreach (var group in groups)
            {
                sb.AppendLine(GetGroupingText(group, i));
                i++;
            }

            return sb.ToString();
        }

        private static string GetGroupingText((string theme, string[] texts) group, int number)
        {
            var joinedText = string.Join("\n\n", group.texts);
            return $"Theme {number}: {group.theme}\n\n{joinedText}\n";
        }

        private async Task<string> ProcessRequest(string prompt)
        {
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_endpoint, content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var parsed = JObject.Parse(responseString);

            return StripMarkdownCodeFencing(parsed["choices"]?[0]?["message"]?["content"]?.ToString()?.Trim() ?? "[No response]");
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
