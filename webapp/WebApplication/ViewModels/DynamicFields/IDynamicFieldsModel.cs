using K9.WebApplication.Enums;
using System;

namespace K9.WebApplication.ViewModels
{
    public interface IDynamicFieldsModel
    {
        ImageListItemViewModel[] GlobalImageFields { get; }
        ImageListItemViewModel[] EntityImageFields { get; }
        Type EntityType { get; }
        string SelectedImageUrl { get; }

        int? EntityId { get; set; }
        string EntityName { get; set; }
        string EntityPluralName { get; set; }
        string FolderName { get; set; }
        string Label { get; set; }
        EDynamicFieldsMode Mode { get; set; }
    }
}