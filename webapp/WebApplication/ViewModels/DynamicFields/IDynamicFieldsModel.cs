using K9.WebApplication.Enums;
using System;

namespace K9.WebApplication.ViewModels
{
    public interface IDynamicFieldsModel
    {
        int? EntityId { get; set; }
        ImageListItemViewModel[] GlobalImageFields { get; }
        ImageListItemViewModel[] EntityImageFields { get; }
        string EntityName { get; }
        Type EntityType { get; }
        string EntityPluralName { get; }
        string FolderName { get; }

        string Label { get; set; }
        EDynamicFieldsMode Mode { get; set; }
    }
}