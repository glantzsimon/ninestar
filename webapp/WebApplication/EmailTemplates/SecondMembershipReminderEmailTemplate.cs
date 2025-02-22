using K9.DataAccessLayer.Models;

namespace K9.WebApplication.EmailTemplates
{
    public class SecondMembershipReminderEmailTemplate : EmailTemplate
    {
        public SecondMembershipReminderEmailTemplate()
        {
            Id = -2;
            Subject = Globalisation.Dictionary.SecondMembershipReminderSubject;
            HtmlBody = Globalisation.Dictionary.SecondMembershipReminderEmail;
        }
    }
}