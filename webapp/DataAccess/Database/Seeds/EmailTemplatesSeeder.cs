using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using System.Data.Entity;
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
            var yearlyMembership = context.Set<MembershipOption>().FirstOrDefault(e => e.SubscriptionType == MembershipOption.ESubscriptionType.AnnualPlatinum);

            if (entity == null)
            {
                context.Set<EmailTemplate>().Add(new EmailTemplate
                {
                    Name = systemEmailTemplate.ToString(),
                    SystemEmailTemplate = systemEmailTemplate,
                    Subject = subject,
                    HtmlBody = body,
                    MembershipOptionId = yearlyMembership?.Id,
                    IsSystemStandard = true,
                });
            }
            else
            {
                entity.Name = systemEmailTemplate.ToString();
                entity.Subject = subject;
                entity.HtmlBody = body;
                entity.MembershipOptionId = yearlyMembership?.Id;

                context.Entry(entity).State = EntityState.Modified;
            }
        }

    }
}
