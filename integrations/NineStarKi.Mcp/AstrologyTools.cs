using System.ComponentModel;
using ModelContextProtocol.Server;

namespace NineStarKi.Mcp;

[McpServerToolType]
public sealed class AstrologyTools(AstrologyApiClient api, McpMemberAccountResolver membership)
{
    [McpServerTool, Description("Fetch a person's 9 Star Ki personal chart from the official website. A member's access is verified through their connected 9 Star Ki website login.")]
    public Task<string> GetPersonalChart(
        [Description("Birth date YYYY-MM-DD")] string dateOfBirth,
        [Description("Male, Female, or Other")] string gender,
        [Description("Known birth time HH-mm; do not invent a time")] string timeOfBirth,
        [Description("Birthplace")] string birthLocation,
        [Description("Staging only: existing nine-digit site account number. Not needed after website OAuth connection.")] string? accountNumber = null,
        CancellationToken cancellationToken = default) =>
        api.GetAsync(cancellationToken, "api", "personal-chart", "get",
            membership.Resolve(accountNumber),
            AstrologyApiClient.BirthDate(dateOfBirth),
            AstrologyApiClient.Gender(gender),
            AstrologyApiClient.BirthTime(timeOfBirth),
            AstrologyApiClient.RequiredText(birthLocation, nameof(birthLocation)));

    [McpServerTool, Description("Fetch yearly, monthly, nine-year and daily 9 Star Ki predictions from the official website. Membership is resolved from the verified website login. For a named year use February 5; for a month use the 10th.")]
    public Task<string> GetPredictions(
        [Description("Birth date YYYY-MM-DD")] string dateOfBirth,
        [Description("Male, Female, or Other")] string gender,
        [Description("Selected prediction date YYYY-MM-DD")] string selectedDate,
        [Description("Known birth time HH-mm; do not invent a time")] string timeOfBirth,
        [Description("Birthplace")] string birthLocation,
        [Description("Staging only: site account number. Not needed after OAuth connection.")] string? accountNumber = null,
        CancellationToken cancellationToken = default) =>
        api.GetAsync(cancellationToken, "api", "predictions", "get",
            membership.Resolve(accountNumber),
            AstrologyApiClient.BirthDate(dateOfBirth),
            AstrologyApiClient.Gender(gender),
            AstrologyApiClient.SelectedDate(selectedDate),
            AstrologyApiClient.BirthTime(timeOfBirth),
            AstrologyApiClient.RequiredText(birthLocation, nameof(birthLocation)));

    [McpServerTool, Description("Fetch 9 Star Ki compatibility and relationship dynamics for two people. Uses the connected member's existing website subscription.")]
    public Task<string> GetCompatibility(
        string firstPersonName, string firstPersonDateOfBirth, string firstPersonGender,
        string secondPersonName, string secondPersonDateOfBirth, string secondPersonGender,
        bool displaySexualChemistry,
        string firstPersonTimeOfBirth, string firstPersonBirthLocation,
        string secondPersonTimeOfBirth, string secondPersonBirthLocation,
        [Description("Staging only: site account number. Not needed after OAuth connection.")] string? accountNumber = null,
        CancellationToken cancellationToken = default) =>
        api.GetAsync(cancellationToken, "api", "compatibility", "get",
            membership.Resolve(accountNumber),
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

    [McpServerTool, Description("Retrieve the official website's 9 Star Ki knowledge base. Requires an eligible member, identified through the connected website login.")]
    public Task<string> GetKnowledgeBase(
        [Description("Staging only: site account number. Not needed after OAuth connection.")] string? accountNumber = null,
        CancellationToken cancellationToken = default) =>
        api.GetAsync(cancellationToken, "api", "knowledgebase", "get",
            membership.Resolve(accountNumber));

    [McpServerTool, Description("Generate an I Ching hexagram from the official 9 Star Ki website for interpretation. No customer membership is required.")]
    public Task<string> GetIChing(CancellationToken cancellationToken = default) =>
        api.GetAsync(cancellationToken, "api", "iching", "get");
}
