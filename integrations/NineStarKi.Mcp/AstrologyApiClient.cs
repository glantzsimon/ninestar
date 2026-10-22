using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace NineStarKi.Mcp;

/// <summary>
/// Wraps the existing HTTPS API without changing its routes or calculations.
/// This adapter does NOT itself confer site membership privileges.
/// </summary>
public sealed class AstrologyApiClient(HttpClient client)
{
    public async Task<string> GetAsync(CancellationToken cancellationToken, params string[] segments)
    {
        var relativePath = string.Join("/", segments.Select(Uri.EscapeDataString));

        try
        {
            using var response = await client.GetAsync(relativePath,
                HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    statusCode = (int)response.StatusCode,
                    error = "The 9 Star Ki service could not complete this request."
                });

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            using var document = JsonDocument.Parse(content);
            if (document.RootElement.ValueKind != JsonValueKind.Object ||
                !document.RootElement.TryGetProperty("success", out _))
                throw new JsonException("Unexpected upstream response");

            // IMPORTANT: the legacy MVC API often reports 401/404/422 INSIDE
            // JSON with HTTP 200. Pass the original JSON through unchanged.
            return content;
        }
        catch (HttpRequestException)
        {
            return JsonSerializer.Serialize(new { success = false, error = "The 9 Star Ki service is unavailable." });
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return JsonSerializer.Serialize(new { success = false, error = "The 9 Star Ki service timed out." });
        }
        catch (JsonException)
        {
            return JsonSerializer.Serialize(new { success = false, error = "The 9 Star Ki service returned an invalid response." });
        }
    }

    public static string AccountNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !Regex.IsMatch(value, @"\A[0-9]{9}\z"))
            throw new ArgumentException("An existing nine-digit account number is required.");
        return value;
    }

    public static string BirthDate(string value)
    {
        if (!DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out _))
            throw new ArgumentException("Birth dates must use YYYY-MM-DD.");
        return value;
    }

    public static string SelectedDate(string value) => BirthDate(value);

    public static string BirthTime(string value)
    {
        if (string.IsNullOrEmpty(value) ||
            !Regex.IsMatch(value, @"\A(?:[01][0-9]|2[0-3])-[0-5][0-9]\z"))
            throw new ArgumentException("Birth time must use HH-mm, for example 07-00.");
        return value;
    }

    public static string Gender(string value)
    {
        if (value is not ("Male" or "Female" or "Other"))
            throw new ArgumentException("Gender must be Male, Female, or Other.");
        return value;
    }

    public static string RequiredText(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 200)
            throw new ArgumentException(fieldName + " is required (maximum 200 characters).");
        return value.Trim();
    }
}
