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
            AddOrEditMembershipOption(context, "FreeMembership", "free_membership_description", MembershipOption.ESubscriptionType.Free, 0, 0, 0);

            //AddMembershipOption(context, "MonthlyStandardMembership", "standard_monthly_membership_description", MembershipOption.ESubscriptionType.MonthlyStandard, 12, 50, 20);

            //AddMembershipOption(context, "YearlyStandardMembership", "standard_annual_membership_description", MembershipOption.ESubscriptionType.AnnualStandard, 79, 50, 20);

            AddOrEditMembershipOption(context, "WeeklyPlatinumMembership", "weekly_membership_description", MembershipOption.ESubscriptionType.WeeklyPlatinum, 14, MembershipOption.Unlimited, MembershipOption.Unlimited);

            AddOrEditMembershipOption(context, "MonthlyPlatinumMembership", "monthly_membership_description", MembershipOption.ESubscriptionType.MonthlyPlatinum, 27, MembershipOption.Unlimited, MembershipOption.Unlimited);

            AddOrEditMembershipOption(context, "YearlyPlatinumMembership", "annual_membership_description", MembershipOption.ESubscriptionType.AnnualPlatinum, 72, MembershipOption.Unlimited, MembershipOption.Unlimited);

            AddOrEditMembershipOption(context, "LifeTimePlatinumMembership", "lifetime_membership_description", MembershipOption.ESubscriptionType.LifeTimePlatinum, 144, MembershipOption.Unlimited, MembershipOption.Unlimited);

            RemoveMembershipOption(context, "MonthlyStandardMembership");

            RemoveMembershipOption(context, "YearlyStandardMembership");

            context.SaveChanges();
        }

        private static void AddOrEditMembershipOption(DbContext context, string name, string details, MembershipOption.ESubscriptionType type, double price, int numberOfReadings, int numberOfCompatibility)
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
                    NumberOfProfileReadings = numberOfReadings,
                    NumberOfCompatibilityReadings = numberOfCompatibility,
                    IsSystemStandard = true
                });
            }
            else
            {
                entity.Name = name;
                entity.SubscriptionDetails = details;
                entity.SubscriptionType = type;
                entity.Price = price;
                entity.NumberOfProfileReadings = numberOfReadings;
                entity.NumberOfCompatibilityReadings = numberOfCompatibility;

                context.Set<MembershipOption>().AddOrUpdate(entity);
            }
        }

        private static void RemoveMembershipOption(DbContext context, string name)
        {
            var entity = context.Set<MembershipOption>().FirstOrDefault(a => a.Name == name);
            if (entity != null)
            {
                entity.IsDeleted = true;
                context.Set<MembershipOption>().AddOrUpdate(entity);
            }
        }
    }
}
