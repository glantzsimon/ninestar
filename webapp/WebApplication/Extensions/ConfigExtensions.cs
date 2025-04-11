using System.IO;
using System.Web;
using System.Web.Optimization;
using K9.WebApplication.Helpers;

namespace K9.WebApplication.Extensions
{
    public static class ConfigExtensions
    {
        public static void IncludeWithRewrite(this StyleBundle bundle, params string[] paths)
        {
            foreach (var path in paths)
            {
                bundle.Include(path, new RelativeCssRewriteUrlTransform());
            }
        }

        public static void IncludeAllCssWithRewrite(this StyleBundle bundle, string folderVirtualPath)
        {
            var physicalPath = HttpContext.Current.Server.MapPath(folderVirtualPath);
            if (Directory.Exists(physicalPath))
            {
                var cssFiles = Directory.GetFiles(physicalPath, "*.css", SearchOption.TopDirectoryOnly);
                foreach (var file in cssFiles)
                {
                    var relativePath = VirtualPathUtility.Combine(folderVirtualPath.TrimEnd('/') + "/", Path.GetFileName(file));
                    bundle.Include(relativePath, new RelativeCssRewriteUrlTransform());
                }
            }
        }
    }
}
