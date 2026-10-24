using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.ViewModels.MobileApi
{
    public class MobileContactRequest
    {
        [Required, StringLength(128)] public string Name { get; set; }
        [Required, EmailAddress, StringLength(200)] public string EmailAddress { get; set; }
        [StringLength(200)] public string Subject { get; set; }
        [StringLength(10000)] public string Body { get; set; }
        [Required, StringLength(10000)] public string CaptchaToken { get; set; }
    }
}
