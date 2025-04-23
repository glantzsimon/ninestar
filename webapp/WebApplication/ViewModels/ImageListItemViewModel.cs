using K9.SharedLibrary.Models;

namespace K9.WebApplication.ViewModels
{
    public class ImageListItemViewModel
    {
        public IDynamicFieldsModel DynamicFieldsModel { get; set; }
        public ImageInfo ImageInfo { get; set; }
        public bool IsSelected { get; set; }
    }
}