using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Globalisation;
using System.ComponentModel.DataAnnotations;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(Dictionary), ListName = Strings.Names.SystemSettings, PluralName = Strings.Names.SystemSettings, Name = Strings.Names.SystemSettings)]
    public class SystemSetting : ObjectBase
    {
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.IsSendMembershipUpgradeRemindersLabel)]
        public bool IsSendMembershipUpgradeReminders { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.IsPausedEmailJobQueueLabel)]
        public bool IsPausedEmailJobQueue { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.IsEnabledAlchemyLabel)]
        public bool IsEnabledAlchemy { get; set; }
    }
}
