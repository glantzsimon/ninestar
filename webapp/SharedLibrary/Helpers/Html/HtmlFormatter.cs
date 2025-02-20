using System;
using System.Linq;
using System.Net;

namespace K9.SharedLibrary.Helpers.Html
{
    public static class HtmlFormatter
    {
        public static string ConvertNewlinesToParagraphs(string input)
        {
            // Return empty if the input is null or whitespace
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Split on any type of newline (Windows, Unix, or Mac)
            var lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            // Wrap each line in <p> tags, HTML-encode the content
            var paragraphs = lines.Select(line => $"<p>{WebUtility.HtmlEncode(line)}</p>");

            // Join all paragraphs into a single string
            return string.Join("", paragraphs);
        }
    }
}