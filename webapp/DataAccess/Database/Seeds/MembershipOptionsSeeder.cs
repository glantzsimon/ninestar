﻿using K9.DataAccessLayer.Models;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace K9.DataAccessLayer.Database.Seeds
{
    public static class MembershipOptionsSeeder
    {
        public static void Seed(DbContext context)
        {
            AddMembershipOption(context, "MonthlyStandardMembership", "standard_membership_description", MembershipOption.ESubscriptionType.MonthlyStandard, 7, 50, 20);
            AddMembershipOption(context, "YearlyStandardMembership", "standard_membership_description", MembershipOption.ESubscriptionType.AnnualStandard, 57, 50, 20);
            AddMembershipOption(context, "MonthlyPlatinumMembership", "platinum_membership_description", MembershipOption.ESubscriptionType.MonthlyPlatinum, 17, MembershipOption.Unlimited, MembershipOption.Unlimited);
            AddMembershipOption(context, "YearlyPlatninumMembership", "platinum_membership_description", MembershipOption.ESubscriptionType.AnnualPlatinum, 97, MembershipOption.Unlimited, MembershipOption.Unlimited);

            context.SaveChanges();
        }

        private static void AddMembershipOption(DbContext context, string name, string details, MembershipOption.ESubscriptionType type, double price, int maxNumberOfReadings, int maxNumberOfCompatibility)
        {
            if (!context.Set<MembershipOption>().Any(a => a.Name == name))
            {
                context.Set<MembershipOption>().AddOrUpdate(new MembershipOption
                {
                    Name = name,
                    SubscriptionDetails = details,
                    SubscriptionType = type,
                    Price = price,
                    MaxNumberOfProfileReadings = maxNumberOfReadings,
                    MaxNumberOfCompatibilityReadings = maxNumberOfCompatibility,
                    IsSystemStandard = true
                });
            }
        }
    }
}