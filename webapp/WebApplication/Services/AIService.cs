using K9.Base.WebApplication.Extensions;
using K9.Globalisation;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Packages;
using K9.WebApplication.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceStack.Text;
using StackExchange.Profiling.Internal;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using K9.WebApplication.Models;

namespace K9.WebApplication.Services
{
    public class AIService : BaseService, IAIService
    {
        private const string ElegantTone = "elegant and refined, yet succinct,";

        private static string WritingStyle = $"Maintain an {ElegantTone} tone throughout, and keep the writing clear, organized, and free-flowing—but without unnecessary explanation.";

        private const string CrossReferenceText =
            "Where applicable, cross-reference different contradictory influences within the same section, if they arise. For example, within a section there could be an expansive, light influence of 9 fire but also the energy of darkness and stillness of water. In such cases, both influences are felt. This may be felt as fluctuating between the two, experiencing contradictory feelings simultaneously, or as the two opposites blending together to soften each other's influence. When addressing this effect, always be as specific as possible, mentioning the influences and their effects with appropriate names and language, as opposed to general statements which may be less clear.";

        private const string DoNotAnnounceText =
            "Do not preface the output (e.g., 'Here is...'). Just return clean HTML output only.";

        private const string PositiveAndChallengingText =
            "Within each section, compare the positive and challenging aspects where relevant, and add sub-themes if it helps organize the section better.";

        private const string OrganisationText =
            "Organize the text into one or more thematic groups, each introduced with an appropriate heading.";

        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly string _endpoint;

        public AIService(INineStarKiBasePackage my) : base(my)
        {
            _httpClient = new HttpClient();
            _apiKey = My.ApiConfiguration.OpenAIApiKey;
            _model = My.ApiConfiguration.OpenApiModel;
            _endpoint = My.ApiConfiguration.OpenApiEndpoint;

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<string> GetYearlyReport(YearlyReportViewModel model)
        {
            return await ProcessRequest(GetYearlyReportPrompt(model));
        }

        public async Task<string> MergeTextsAsync((string theme, string[] texts)[] groups, string extraPrompt = null)
        {
            var prompt = GetPrompt(
                $"Blend the following texts into a clear, well-organized passage using only <h5> and <p> HTML tags. {extraPrompt}",
                groups
            );
            return await ProcessRequest(prompt);
        }

        public async Task<string> MergeTextsIntoSummaryAsync((string theme, string[] texts)[] groups, string extraPrompt = null)
        {
            var prompt = GetPrompt(
                $"Blend the following texts into a clear, well-organized summary using only <h5>, <ul>, and <li> HTML tags. {extraPrompt}",
                groups
            );
            return await ProcessRequest(prompt);
        }

        public string GetYearlyReportPrompt(YearlyReportViewModel model)
        {
            var nineStarKiModel = model.NineStarKiModel;
            var personModel = nineStarKiModel.PersonModel;
            var year = nineStarKiModel.SelectedDate.Value.Year;

            return TemplateParser.Parse(Dictionary.yearly_report, new
            {
                Year = year,
                YearPlusNine = year + 9,

                FullName = personModel.Name,
                personModel.Gender,
                DateOfBirth = personModel.DateOfBirth.ToLocalDateString(),
                MainEnergy = nineStarKiModel.MainEnergy.EnergyNameNumberAndElement,
                CharacterEnergy = nineStarKiModel.CharacterEnergy.EnergyNameNumberAndElement,
                SurfaceEnergy = nineStarKiModel.SurfaceEnergy.EnergyNameNumberAndElement,
                GenerationalKi = nineStarKiModel.PersonalChartEnergies.Generation.EnergyNameNumberAndElement,
                DayStar = nineStarKiModel.PersonalChartEnergies.Day.EnergyNameNumberAndElement,
                PersonalData = GetPersonalData(personModel, nineStarKiModel).ToJson(),

                model.YearlyPlannerModel.PeriodStarsOn,
                model.YearlyPlannerModel.PeriodEndsOn,
                YearlyEnergy = model.NineStarKiModel.PersonalHousesOccupiedEnergies.Year.EnergyNameNumberAndElement,
                YearlyTheme = model.NineStarKiModel.PersonalHousesOccupiedEnergies.Year.CycleDescription,
                YearlyDirectionsData = model.NineStarKiModel.GetDrectionsChartViewModel().ToJson(),

                PlannerDataJson = model.YearlyPlannerModel.ToJson()
            });
        }

        private static object GetPersonalData(PersonModel personModel, NineStarKiModel nineStarKiModel)
        {
            return new
            {
                personModel,
                GenerationEnergy = new NineStarKiEnergySummary(nineStarKiModel.PersonalChartEnergies.Generation),
                CoreEnergy = new NineStarKiEnergySummary(nineStarKiModel.MainEnergy),
                CharacterEnergy = new NineStarKiEnergySummary(nineStarKiModel.CharacterEnergy),
                SocialExpressionEnergy = new NineStarKiEnergySummary(nineStarKiModel.SurfaceEnergy),
                DayStarEnergy = new NineStarKiEnergySummary(nineStarKiModel.PersonalChartEnergies.Day),

                nineStarKiModel.Summary,
                nineStarKiModel.Overview,
                IntellectualQualities = nineStarKiModel.MainEnergy.IntellectualQualitiesSummary,
                InterpersonalQualities = nineStarKiModel.MainEnergy.InterpersonalQualitiesSummary,
                EmotionalLandscape = nineStarKiModel.MainEnergy.EmotionalLandscapeSummary,
                Spirituality = nineStarKiModel.MainEnergy.SpiritualitySummary,
                Health = nineStarKiModel.MainEnergy.HealthSummary,
                nineStarKiModel.MainEnergy.Illnesses,
                Career = nineStarKiModel.MainEnergy.CareerSummary,
                Finances = nineStarKiModel.MainEnergy.FinancesSummary,
                nineStarKiModel.MainEnergy.Occupations,
                nineStarKiModel.MainEnergyRelationshipsSummary,
                nineStarKiModel.StressResponseDetails,
                nineStarKiModel.StressResponseFromNatalHouseDetails,
                nineStarKiModel.AdultChildRelationsihpDescription,

            };
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
