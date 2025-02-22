using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace K9.DataAccessLayer.Database.Seeds
{
    public static class EmailTemplatesSeeder
    {
        public static void Seed(DbContext context)
        {
            AddOrEditEmailTemplate(context, Globalisation.Dictionary.FirstMembershipReminderSubject, Globalisation.Dictionary.FirstMembershipReminderEmail, ESystemEmailTemplate.FirstMembershipReminder);

            AddOrEditEmailTemplate(context, Globalisation.Dictionary.SecondMembershipReminderSubject, Globalisation.Dictionary.SecondMembershipReminderEmail, ESystemEmailTemplate.SecondMembershipReminder);

            AddOrEditEmailTemplate(context, Globalisation.Dictionary.ThirdMembershipReminderSubject, Globalisation.Dictionary.ThirdMembershipReminderEmail, ESystemEmailTemplate.ThirdMembershipReminder);
         
            context.SaveChanges();
        }

        private static void AddOrEditEmailTemplate(DbContext context, string subject, string body, ESystemEmailTemplate systemEmailTemplate)
        {
            var entity = context.Set<EmailTemplate>().FirstOrDefault(e => e.SystemEmailTemplate == systemEmailTemplate);
           
            if (entity == null)
            {
                context.Set<EmailTemplate>().AddOrUpdate(new EmailTemplate
                {
                    Name = systemEmailTemplate.ToString(),
                    SystemEmailTemplate = systemEmailTemplate,
                    Subject = subject,
                    HtmlBody = body,
                    IsSystemStandard = true
                });
            }
            else
            {
                entity.Name = systemEmailTemplate.ToString();
                entity.Subject = subject;
                entity.HtmlBody = body;
                context.Set<EmailTemplate>().AddOrUpdate(entity);
            }
        }
    }
}
