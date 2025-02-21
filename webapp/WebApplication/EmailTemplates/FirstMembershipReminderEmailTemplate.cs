using K9.DataAccessLayer.Models;

namespace K9.WebApplication.EmailTemplates
{
    public class FirstMembershipReminderEmailTemplate : EmailTemplate
    {
        public FirstMembershipReminderEmailTemplate()
        {
            Subject = Globalisation.Dictionary.FirstMembershipReminderSubject;
            HtmlBody = Globalisation.Dictionary.FirstMembershipReminderEmail;
        }
    }
}