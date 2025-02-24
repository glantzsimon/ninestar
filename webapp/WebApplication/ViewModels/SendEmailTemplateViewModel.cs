using System.ComponentModel.DataAnnotations;
using K9.DataAccessLayer.Models;

namespace K9.WebApplication.ViewModels
{
    public class SendEmailTemplateViewModel
    {
        public EmailTemplate EmailTemplate { get; set; }
        
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.UserLabel)]
        [UIHint("User")]
        public int? UserId { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.MailingListLabel)]
        [UIHint("MailingList")]
        public int? MailingListId { get; set; }
    }
}