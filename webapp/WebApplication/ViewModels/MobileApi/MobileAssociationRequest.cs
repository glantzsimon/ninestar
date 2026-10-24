using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.ViewModels.MobileApi
{
    public class MobileAssociationRequest
    {
        [Range(1, int.MaxValue)] public int OwnerId { get; set; }
        [Required, MaxLength(1000)] public int[] SelectedIds { get; set; }
    }
}
