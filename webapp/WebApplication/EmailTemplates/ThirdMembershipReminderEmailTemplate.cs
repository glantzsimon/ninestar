using K9.DataAccessLayer.Models;

namespace K9.WebApplication.EmailTemplates
{
    public class ThirdMembershipReminderEmailTemplate : EmailTemplate
    {
        public ThirdMembershipReminderEmailTemplate()
        {
            Subject = Globalisation.Dictionary.ThirdMembershipReminderSubject;
            HtmlBody = Globalisation.Dictionary.ThirdMembershipReminderEmail;
        }
    }
}