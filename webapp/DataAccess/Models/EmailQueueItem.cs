using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.DataAccessLayer.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(Globalisation.Dictionary), ListName = Globalisation.Strings.Names.EmailQueueItems, PluralName = Globalisation.Strings.Names.EmailQueueItems, Name = Globalisation.Strings.Names.EmailQueueItem)]
    public class EmailQueueItem : ObjectBase
    {
        [Required]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.EmailTypeLabel)]
        public EEmailType Type { get; set; }

        [Required]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.EmailTemplateLabel)]
        [UIHint("EmailTemplate")]
        [ForeignKey("EmailTemplate")]
        public int EmailTemplateId { get; set; }

        public virtual EmailTemplate EmailTemplate { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.ContactLabel)]
        [UIHint("Contact")]
        [ForeignKey("Contact")]
        public int? ContactId { get; set; }

        public virtual Contact Contact { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Names.User)]
        [UIHint("User")]
        [ForeignKey("User")]
        public int? UserId { get; set; }
        
        public virtual User User { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EmailAddressLabel)]
        public string RecipientEmailAddress => User != null ? User.EmailAddress :
            Contact != null ? Contact.EmailAddress : string.Empty;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.NameLabel)]
        [StringLength(128)]
        public string RecipientName => User != null ? User.FullName:
            Contact != null ? Contact.FullName : string.Empty;

        [Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SubjectLabel)]
        public string Subject { get; set; }

        [Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.BodyLabel)]
        public string Body { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.ScheduledOnLabel)]
        public DateTime? ScheduledOn { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SentOnLabel)]
        public DateTime? SentOn { get; set; }

        public bool IsSent => SentOn.HasValue;
    }
}
