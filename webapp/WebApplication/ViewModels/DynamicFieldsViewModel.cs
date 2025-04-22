using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace K9.WebApplication.ViewModels
{
    public abstract class DynamicFieldsViewModel<T> : IDynamicFieldsModel
        where T : class, IObjectBase 
    {
        public int? EntityId { get; set; }
        public ImageInfo[] GlobalImageFields { get; }
        public ImageInfo[] EntityImageFields { get; }
        public string EntityName { get; }
        public Type EntityType { get; }
        public string EntityPluralName { get; }
        public string FolderName { get; }

        public DynamicFieldsViewModel(int? id = null, string entityName = "", string entityPluralName = "")
        {
            EntityId = id;
            EntityName = string.IsNullOrEmpty(entityName) ? typeof(T).Name : entityName;
            EntityType = typeof(T);
            EntityPluralName = string.IsNullOrEmpty(entityPluralName) ? EntityName.Pluralise() : entityPluralName;
            FolderName = EntityPluralName.ToLower();
            GlobalImageFields = GetImages();
            EntityImageFields = EntityId.HasValue && EntityId.Value > 0 ? GetImages(EntityId.Value) : Array.Empty<ImageInfo>();
        }

        private ImageInfo[] GetImages(int? id = null)
        {
            var virtualFolder = id.HasValue
                ? $"~/Images/{FolderName}/{id.Value}"
                : $"~/Images/{FolderName}";

            var physicalPath = HostingEnvironment.MapPath(virtualFolder);

            if (string.IsNullOrWhiteSpace(physicalPath) || !Directory.Exists(physicalPath))
            {
                return Array.Empty<ImageInfo>();
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

                    return new ImageInfo
                    {
                        FileName = fileName,
                        AltText = Path.GetFileNameWithoutExtension(fileName),
                        Src = $"{DefaultValuesConfiguration.Instance.BaseImagesPath}{relative.Replace("/Images", "")}", // Storj path
                        RelativePath = relative // used for Server.MapPath
                    };
                })
                .ToArray();
        }
    }
}