using System.Web;
using System.Web.Optimization;

namespace K9.WebApplication.Helpers
{
    public class RelativeCssRewriteUrlTransform : IItemTransform
    {
        public string Process(string includedVirtualPath, string input)
        {
            var basePath = VirtualPathUtility.ToAbsolute("~");
            var rewrite = new CssRewriteUrlTransform();
            var rewritten = rewrite.Process(includedVirtualPath, input);

            // Prepend virtual path if missing
            return rewritten.Replace("url(/Content", $"url({basePath.TrimEnd('/')}/Content");
        }
    }
}