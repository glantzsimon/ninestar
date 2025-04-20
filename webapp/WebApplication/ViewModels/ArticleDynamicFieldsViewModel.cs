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
        public ImageInfo[] ImageFields { get; }

        public ArticleDynamicFieldsViewModel()
        {
            ImageFields = GetEmailTemplateImages();
        }

        private static ImageInfo[] GetEmailTemplateImages()
        {
            var virtualFolder = "~/Images/articles";
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