using System.Text.RegularExpressions;

namespace K9.SharedLibrary.Helpers
{
    public static class SqlValidator
    {
        public static bool IsSafeSqlQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return false;

            query = query.Trim();

            // Ensure query starts with "SELECT" (case-insensitive)
            if (!Regex.IsMatch(query, @"^\s*SELECT\s", RegexOptions.IgnoreCase))
                return false;

            // Block dangerous SQL keywords
            string[] forbiddenKeywords = { "INSERT", "UPDATE", "DELETE", "DROP", "ALTER", "TRUNCATE", "EXEC", "MERGE" };
            foreach (var keyword in forbiddenKeywords)
            {
                if (Regex.IsMatch(query, $@"\b{keyword}\b", RegexOptions.IgnoreCase))
                    return false;
            }

            return true;
        }
    }

}
