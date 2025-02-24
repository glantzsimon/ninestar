using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.Models
{
    public class MailingListSendResultItem
    {
        [Display(Name = "UserId")]
        public int UserId { get; set; }
        
        [Display(Name = "Recipient")]
        public string RecipientName { get; set; }
        
        public string EmailAddress { get; set; }
     
        [Display(Name = "Success")]
        public bool IsSuccess { get; set; }
     
        [Display(Name = "Error")]
        public string ErrorMessage { get; set; }
    }
}