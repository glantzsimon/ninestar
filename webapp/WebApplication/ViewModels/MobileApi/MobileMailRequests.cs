using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.ViewModels.MobileApi
{
    public class MobilePromotionEmailRequest
    {
        [Range(1, int.MaxValue)] public int PromotionId { get; set; }
        [Required, StringLength(128)] public string Name { get; set; }
        [Required, EmailAddress, StringLength(255)] public string EmailAddress { get; set; }
    }

    public class MobileUnsubscribeRequest
    {
        [Required, StringLength(128)] public string ExternalId { get; set; }
    }
}
