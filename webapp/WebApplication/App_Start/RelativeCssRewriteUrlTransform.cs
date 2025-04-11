using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Optimization;

namespace K9.WebApplication.Helpers
{
    public class RelativeCssRewriteUrlTransform : IItemTransform
    {
        public string Process(string includedVirtualPath, string input)
        {
            var rewrite = new CssRewriteUrlTransform();
            var appPath = VirtualPathUtility.ToAbsolute("~").TrimEnd('/');

            return Regex.Replace(input, @"url\((['""]?)([^)'""]+)\1\)", match =>
            {
                var quote = match.Groups[1].Value;
                var url = match.Groups[2].Value;

                // Skip fully qualified external URLs
                if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                    url.StartsWith("//"))
                {
                    return match.Value; // Don't touch it
                }

                // Let the standard rewrite handle the path
                var rewritten = rewrite.Process(includedVirtualPath, match.Value);

                // If the rewritten URL starts with /Content (i.e., app-relative), prepend app base path
                return Regex.Replace(rewritten, @"url\((['""]?)/Content", $"url($1{appPath}/Content", RegexOptions.IgnoreCase);
            }, RegexOptions.IgnoreCase);
        }
    }
}