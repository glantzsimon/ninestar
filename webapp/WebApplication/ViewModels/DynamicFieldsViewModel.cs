using K9.Globalisation;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Enums;
using K9.WebApplication.Services;
using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace K9.WebApplication.ViewModels
{
    public abstract class DynamicFieldsViewModel<T> : IDynamicFieldsModel
        where T : class, IObjectBase
    {
        public ImageListItemViewModel[] GlobalImageFields { get; }
        public ImageListItemViewModel[] EntityImageFields { get; }
        public Type EntityType { get; }
        public string SelectedImageUrl { get; }

        public int? EntityId { get; set; }
        public string EntityName { get; set; }
        public string EntityPluralName { get; set; }
        public string FolderName { get; set; }
        public string Label { get; set; }
        public int MaxUploads { get; set; }
        public EDynamicFieldsMode Mode { get; set; }

        public DynamicFieldsViewModel(int? id = null, string selectedImageUrl = "", string entityName = "", string entityPluralName = "", string label = "", EDynamicFieldsMode mode = EDynamicFieldsMode.Advanced)
        {
            EntityId = id;
            EntityName = string.IsNullOrEmpty(entityName) ? typeof(T).Name : entityName;
            EntityType = typeof(T);
            EntityPluralName = string.IsNullOrEmpty(entityPluralName) ? EntityName.Pluralise() : entityPluralName;
            Label = string.IsNullOrEmpty(label) ? Dictionary.Images : label;
            SelectedImageUrl = selectedImageUrl;
            Mode = mode;
            FolderName = EntityPluralName.ToLower();
            GlobalImageFields = GetImages();
            EntityImageFields = EntityId.HasValue && EntityId.Value > 0 ? GetImages(EntityId.Value) : Array.Empty<ImageListItemViewModel>();
        }

        private ImageListItemViewModel[] GetImages(int? id = null)
        {
            var virtualFolder = id.HasValue
                ? $"~/Images/{FolderName}/{id.Value}"
                : $"~/Images/{FolderName}";

            var physicalPath = HostingEnvironment.MapPath(virtualFolder);

            if (string.IsNullOrWhiteSpace(physicalPath) || !Directory.Exists(physicalPath))
            {
                return Array.Empty<ImageListItemViewModel>();
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            return Directory.GetFiles(physicalPath)
                .Where(f => allowedExtensions.Contains(Path.GetExtension(f).ToLower()))
                .Select(f =>
                {
                    var fileName = Path.GetFileName(f);
                    var relative = id.HasValue
                        ? $"/Images/{FolderName}/{id.Value}/{fileName}"
                        : $"/Images/{FolderName}/{fileName}";

                    var imageInfo = new ImageInfo
                    {
                        FileName = fileName,
                        AltText = Path.GetFileNameWithoutExtension(fileName),
                        Src = $"{MediaService.BaseImagesPath}{relative.Replace("/Images", "")}",
                        RelativePath = relative // used for Server.MapPath
                    };

                    return new ImageListItemViewModel
                    {
                        ImageInfo = imageInfo,
                        DynamicFieldsModel = this,
                        IsSelected = string.Equals(
                            SelectedImageUrl?.Trim().ToLowerInvariant(),
                            imageInfo.Src?.Trim().ToLowerInvariant(),
                            StringComparison.OrdinalIgnoreCase
                        )
                    };
                })
                .ToArray();
        }
    }
}