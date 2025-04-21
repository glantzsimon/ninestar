using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace K9.WebApplication.ViewModels
{
    public class ArticleDynamicFieldsViewModel
    {
        public int? ArticleId { get; set; }
        public ImageInfo[] GlobalImageFields { get; }
        public ImageInfo[] ArticleImageFields { get; }

        public ArticleDynamicFieldsViewModel(int? articleId = null)
        {
            ArticleId = articleId;
            GlobalImageFields = GetEmailTemplateImages();
            ArticleImageFields = ArticleId.HasValue && ArticleId.Value > 0 ? GetEmailTemplateImages(ArticleId.Value) : Array.Empty<ImageInfo>();
        }

        private static ImageInfo[] GetEmailTemplateImages(int? articleId = null)
        {
            var virtualFolder = articleId.HasValue ? $"~/Images/articles/{articleId.Value}" : "~/Images/articles";
            var physicalPath = HostingEnvironment.MapPath(virtualFolder);

            if (Directory.Exists(physicalPath))
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

                return Directory.GetFiles(physicalPath)
                    .Where(f => allowedExtensions.Contains(Path.GetExtension(f).ToLower()))
                    .Select(f =>
                    {
                        var fileName = Path.GetFileName(f);
                        return new ImageInfo
                        {
                            FileName = fileName,
                            AltText = Path.GetFileNameWithoutExtension(fileName),
                            Src = articleId.HasValue 
                                ? $"{DefaultValuesConfiguration.Instance.BaseImagesPath}/articles/{articleId.Value}/{fileName}"
                                : $"{DefaultValuesConfiguration.Instance.BaseImagesPath}/articles/{fileName}",
                            LocalSrc = articleId.HasValue 
                                ? $"{DefaultValuesConfiguration.Instance.LocalBaseImagesPath}/articles/{articleId.Value}/{fileName}"
                                : $"{DefaultValuesConfiguration.Instance.LocalBaseImagesPath}/articles/{fileName}",
                        };
                    })
                    .ToArray();
            }

            return Array.Empty<ImageInfo>();
        }

    }
}