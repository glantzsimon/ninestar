using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;

namespace K9.WebApplication.EmailTemplates
{
    public class ThirdMembershipReminderEmailTemplate : EmailTemplate
    {
        public ThirdMembershipReminderEmailTemplate()
        {
            SystemEmailTemplate = ESystemEmailTemplate.ThirdMembershipReminder;
            Subject = Globalisation.Dictionary.ThirdMembershipReminderSubject;
            HtmlBody = Globalisation.Dictionary.ThirdMembershipReminderEmail;
        }
    }
}