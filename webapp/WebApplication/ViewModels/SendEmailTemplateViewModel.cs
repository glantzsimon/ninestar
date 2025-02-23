using System.ComponentModel.DataAnnotations;
using K9.DataAccessLayer.Models;

namespace K9.WebApplication.ViewModels
{
    public class SendEmailTemplateViewModel
    {
        public EmailTemplate EmailTemplate { get; set; }
        
        [UIHint("User")]
        public int? UserId { get; set; }

        [UIHint("MailingList")]
        public int? MailingListId { get; set; }
    }
}