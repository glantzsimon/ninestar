using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.ViewModels.MobileApi
{
    public class MobileMessageWriteRequest
    {
        public int? Id { get; set; }
        [Range(1, int.MaxValue)] public int RecipientId { get; set; }
        [Required, StringLength(256)] public string Subject { get; set; }
        [Required, StringLength(10000)] public string Body { get; set; }
    }
}
