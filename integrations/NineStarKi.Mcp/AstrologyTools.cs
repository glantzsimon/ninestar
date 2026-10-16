using System.ComponentModel;
using ModelContextProtocol.Server;

namespace NineStarKi.Mcp;

[McpServerToolType]
public sealed class AstrologyTools(AstrologyApiClient api)
{
    [McpServerTool, Description("Fetch a person's 9 Star Ki personal chart from the authoritative 9 Star Ki Astrology website. An existing site membership account number is required.")]
    public Task<string> GetPersonalChart(
        [Description("Existing nine-digit site account number")] string accountNumber,
        [Description("Birth date YYYY-MM-DD")] string dateOfBirth,
        [Description("Male, Female, or Other")] string gender,
        [Description("Known birth time HH-mm; do not invent a time")] string timeOfBirth,
        [Description("Birthplace or an agreed unknown-value placeholder accepted by the website")] string birthLocation,
        CancellationToken cancellationToken) =>
        api.GetAsync(cancellationToken, "api", "personal-chart", "get",
            AstrologyApiClient.AccountNumber(accountNumber),
            AstrologyApiClient.BirthDate(dateOfBirth),
            AstrologyApiClient.Gender(gender),
            AstrologyApiClient.BirthTime(timeOfBirth),
            AstrologyApiClient.RequiredText(birthLocation, nameof(birthLocation)));

    [McpServerTool, Description("Fetch yearly, monthly, nine-year and daily 9 Star Ki predictions from the official website. For a named year use February 5 of that year; for a named month use the 10th.")]
    public Task<string> GetPredictions(
        [Description("Existing nine-digit site account number")] string accountNumber,
        [Description("Birth date YYYY-MM-DD")] string dateOfBirth,
        [Description("Male, Female, or Other")] string gender,
        [Description("Selected prediction date YYYY-MM-DD")] string selectedDate,
        [Description("Known birth time HH-mm; do not invent a time")] string timeOfBirth,
        [Description("Birthplace")] string birthLocation,
        CancellationToken cancellationToken) =>
        api.GetAsync(cancellationToken, "api", "predictions", "get",
            AstrologyApiClient.AccountNumber(accountNumber),
            AstrologyApiClient.BirthDate(dateOfBirth),
            AstrologyApiClient.Gender(gender),
            AstrologyApiClient.SelectedDate(selectedDate),
            AstrologyApiClient.BirthTime(timeOfBirth),
            AstrologyApiClient.RequiredText(birthLocation, nameof(birthLocation)));

    [McpServerTool, Description("Fetch 9 Star Ki compatibility and relationship dynamics for two people using the official website.")]
    public Task<string> GetCompatibility(
        [Description("Existing nine-digit site account number")] string accountNumber,
        string firstPersonName, string firstPersonDateOfBirth, string firstPersonGender,
        string secondPersonName, string secondPersonDateOfBirth, string secondPersonGender,
        bool displaySexualChemistry,
        string firstPersonTimeOfBirth, string firstPersonBirthLocation,
        string secondPersonTimeOfBirth, string secondPersonBirthLocation,
        CancellationToken cancellationToken) =>
        api.GetAsync(cancellationToken, "api", "compatibility", "get",
            AstrologyApiClient.AccountNumber(accountNumber),
            AstrologyApiClient.RequiredText(firstPersonName, nameof(firstPersonName)),
            AstrologyApiClient.BirthDate(firstPersonDateOfBirth),
            AstrologyApiClient.Gender(firstPersonGender),
            AstrologyApiClient.RequiredText(secondPersonName, nameof(secondPersonName)),
            AstrologyApiClient.BirthDate(secondPersonDateOfBirth),
            AstrologyApiClient.Gender(secondPersonGender),
            displaySexualChemistry ? "true" : "false",
            AstrologyApiClient.BirthTime(firstPersonTimeOfBirth),
            AstrologyApiClient.RequiredText(firstPersonBirthLocation, nameof(firstPersonBirthLocation)),
            AstrologyApiClient.BirthTime(secondPersonTimeOfBirth),
            AstrologyApiClient.RequiredText(secondPersonBirthLocation, nameof(secondPersonBirthLocation)));

    [McpServerTool, Description("Retrieve the website's 9 Star Ki knowledge base. Requires an existing membership account number.")]
    public Task<string> GetKnowledgeBase(
        [Description("Existing nine-digit site account number")] string accountNumber,
        CancellationToken cancellationToken) =>
        api.GetAsync(cancellationToken, "api", "knowledgebase", "get",
            AstrologyApiClient.AccountNumber(accountNumber));

    [McpServerTool, Description("Generate an I Ching hexagram from the official website for interpretation in light of a user's question. No account number required.")]
    public Task<string> GetIChing(CancellationToken cancellationToken) =>
        api.GetAsync(cancellationToken, "api", "iching", "get");
}
