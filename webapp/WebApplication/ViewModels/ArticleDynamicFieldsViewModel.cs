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

        public ArticleDynamicFieldsViewModel(int? id = null)
        {
            GlobalImageFields = GetEmailTemplateImages();
            ArticleImageFields = id.HasValue ? GetEmailTemplateImages() : Array.Empty<ImageInfo>();
        }

        private static ImageInfo[] GetEmailTemplateImages(int? id = null)
        {
            var virtualFolder = id.HasValue ? $"~/Images/articles/{id}" : "~/Images/articles";
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
                            Src = $"{DefaultValuesConfiguration.Instance.BaseImagesPath}/articles/{fileName}"
                        };
                    })
                    .ToArray();
            }

            return Array.Empty<ImageInfo>();
        }

    }
}