using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;

namespace K9.WebApplication.EmailTemplates
{
    public class SecondMembershipReminderEmailTemplate : EmailTemplate
    {
        public SecondMembershipReminderEmailTemplate()
        {
            SystemEmailTemplate = ESystemEmailTemplate.SecondMembershipReminder;
            Subject = Globalisation.Dictionary.SecondMembershipReminderSubject;
            HtmlBody = Globalisation.Dictionary.SecondMembershipReminderEmail;
        }
    }
}