using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace K9.WebApplication.ViewModels
{
    public class EmailTemplateDynamicFieldsViewModel
    {
        public string[] DynamicFields { get; }
        public ImageInfo[] ImageFields { get; }

        public EmailTemplateDynamicFieldsViewModel()
        {
            DynamicFields = new[]
            {
                nameof(User.FirstName),
                nameof(Promotion.DiscountPercent),
                nameof(Promotion.DiscountPercentText),
                nameof(Promotion.FormattedFullPrice),
                nameof(Promotion.FormattedSpecialPrice),
                nameof(Promotion.MembershipName),
                nameof(Promotion.PromoLink),
            };

            ImageFields = GetEmailTemplateImages();
        }

        private static ImageInfo[] GetEmailTemplateImages()
        {
            var virtualFolder = "~/Images/emailtemplates/custom";
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
                            Src = $"{DefaultValuesConfiguration.Instance.BaseImagesPath}/emailtemplates/custom/{fileName}"
                        };
                    })
                    .ToArray();
            }

            return Array.Empty<ImageInfo>();
        }

    }
}