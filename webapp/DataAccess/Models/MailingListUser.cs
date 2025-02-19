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
    public class MailingListUser : ObjectBase
    {
        [UIHint("MailingList")]
        [Required]
        [ForeignKey("MailingList")]
        public int MailingListId { get; set; }

        [UIHint("User")]
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public virtual MailingList MailingList { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.UserNameLabel)]
        [LinkedColumn(LinkedTableName = "User", LinkedColumnName = "Username")]
        public string UserName { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Names.MailingList)]
        [LinkedColumn(LinkedTableName = "MailingList", LinkedColumnName = "Name")]
        public string MailingListName { get; set; }
        
    }
}
