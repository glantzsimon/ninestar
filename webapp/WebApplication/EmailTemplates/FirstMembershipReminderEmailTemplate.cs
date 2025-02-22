using K9.DataAccessLayer.Models;

namespace K9.WebApplication.EmailTemplates
{
    public class FirstMembershipReminderEmailTemplate : EmailTemplate
    {
        public FirstMembershipReminderEmailTemplate()
        {
            Id = -1;
            Subject = Globalisation.Dictionary.FirstMembershipReminderSubject;
            HtmlBody = Globalisation.Dictionary.FirstMembershipReminderEmail;
        }
    }
}