using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Authentication;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [DefaultPermissions(Role = RoleNames.Administrators)]
    public class MailingListContact : ObjectBase
    {
        [UIHint("MailingList")]
        [Required]
        [ForeignKey("MailingList")]
        public int MailingListId { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.ContactLabel)]
        [UIHint("Contact")]
        [Required]
        [ForeignKey("Contact")]
        public int ContactId { get; set; }

        public virtual MailingList MailingList { get; set; }

        public virtual Contact Contact { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Globalisation.Strings.Labels.ContactLabel)]
        [LinkedColumn(LinkedTableName = "Contact", LinkedColumnName = "FullName")]
        public string ContactName { get; set; }
        
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Names.MailingList)]
        [LinkedColumn(LinkedTableName = "MailingList", LinkedColumnName = "Name")]
        public string MailingListName { get; set; }
        
    }
}
