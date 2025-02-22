using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;

namespace K9.WebApplication.EmailTemplates
{
    public class FirstMembershipReminderEmailTemplate : EmailTemplate
    {
        public FirstMembershipReminderEmailTemplate()
        {
            SystemEmailTemplate = ESystemEmailTemplate.FirstMembershipReminder;
            Subject = Globalisation.Dictionary.FirstMembershipReminderSubject;
            HtmlBody = Globalisation.Dictionary.FirstMembershipReminderEmail;
        }
    }
}