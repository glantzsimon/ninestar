using K9.DataAccessLayer.Models;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace K9.DataAccessLayer.Database.Seeds
{
    public static class MembershipOptionsSeeder
    {
        public static void Seed(DbContext context)
        {
            AddOrEditMembershipOption(context, "FreeMembership", "free_membership_description", MembershipOption.ESubscriptionType.Free, 0, true);

            AddOrEditMembershipOption(context, "WeeklyPlatinumMembership", "weekly_membership_description", MembershipOption.ESubscriptionType.WeeklyPlatinum, 9, true);

            AddOrEditMembershipOption(context, "MonthlyPlatinumMembership", "monthly_membership_description", MembershipOption.ESubscriptionType.MonthlyPlatinum, 18, true);

            AddOrEditMembershipOption(context, "YearlyPlatinumMembership", "annual_membership_description", MembershipOption.ESubscriptionType.AnnualPlatinum, 45, true);

            AddOrEditMembershipOption(context, "LifeTimePlatinumMembership", "lifetime_membership_description", MembershipOption.ESubscriptionType.LifeTimePlatinum, 111, true);

            RemoveMembershipOption(context, "MonthlyStandardMembership");

            RemoveMembershipOption(context, "YearlyStandardMembership");

            context.SaveChanges();
        }

        private static void AddOrEditMembershipOption(DbContext context, string name, string details, MembershipOption.ESubscriptionType type, double price, bool isActive = true)
        {
            var entity = context.Set<MembershipOption>().FirstOrDefault(a => a.Name == name);

            if (entity == null)
            {
                context.Set<MembershipOption>().AddOrUpdate(new MembershipOption
                {
                    Name = name,
                    SubscriptionDetails = details,
                    SubscriptionType = type,
                    Price = price,
                    IsDeleted = !isActive,
                    IsSystemStandard = true
                });
            }
            else
            {
                entity.Name = name;
                entity.SubscriptionDetails = details;
                entity.SubscriptionType = type;
                entity.Price = price;
                entity.IsDeleted = !isActive;
          
                context.Entry(entity).State = EntityState.Modified;
            }
        }

        private static void RemoveMembershipOption(DbContext context, string name)
        {
            var entity = context.Set<MembershipOption>().FirstOrDefault(a => a.Name == name);
            if (entity != null)
            {
                entity.IsDeleted = true;
                context.Entry(entity).State = EntityState.Modified;
            }
        }
    }
}
