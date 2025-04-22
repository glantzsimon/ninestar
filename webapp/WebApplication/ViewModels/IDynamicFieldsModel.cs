using K9.SharedLibrary.Models;
using System;

namespace K9.WebApplication.ViewModels
{
    public interface IDynamicFieldsModel
    {
        int? EntityId { get; set; }
        ImageInfo[] GlobalImageFields { get; }
        ImageInfo[] EntityImageFields { get; }
        string EntityName { get; }
        Type EntityType { get; }
        string EntityPluralName { get; }
        string FolderName { get; }
    }
}