using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.DataAccessLayer.Models
{
    [Name(ResourceType = typeof(Globalisation.Dictionary), ListName = Globalisation.Strings.Names.MailingLists, PluralName = Globalisation.Strings.Names.MailingLists, Name = Globalisation.Strings.Names.MailingList)]
    public class MailingList : ObjectBase
    {
        public const int AllUsersId = -1000;
        public const int PaidUsersId = -1001;
        public const int FreeUsersId = -1002;
        public const int SignedUpInLastTwoWeeksId = -1003;
        public const int BaseUsersId = -2000;

        [NotMapped]
        public List<User> Users { get; set; } = new List<User>();

        [Required]
        [Display(ResourceType = typeof(Base.Globalisation.Dictionary), Name = Base.Globalisation.Strings.Labels.DescriptionLabel)]
        [StringLength(128)]
        public string Details { get; set; }
    }
}
