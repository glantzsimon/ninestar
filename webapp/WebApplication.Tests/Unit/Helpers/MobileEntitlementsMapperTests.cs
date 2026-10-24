using K9.DataAccessLayer.Models;
using K9.WebApplication.Helpers;
using System;
using System.Globalization;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Helpers
{
    public class MobileEntitlementsMapperTests
    {
        [Theory]
        [InlineData(MembershipOption.ESubscriptionType.Free, false)]
        [InlineData(MembershipOption.ESubscriptionType.WeeklyPlatinum, true)]
        [InlineData(MembershipOption.ESubscriptionType.MonthlyPlatinum, true)]
        [InlineData(MembershipOption.ESubscriptionType.AnnualPlatinum, true)]
        [InlineData(MembershipOption.ESubscriptionType.LifeTimePlatinum, true)]
        public void ExistingMembershipRulesDetermineBothPlatforms(MembershipOption.ESubscriptionType type, bool paid)
        {
            var membership = Membership(type);
            var now = DateTime.UtcNow;
            var result = MobileEntitlementsMapper.Map(42, membership, now);
            Assert.Equal(now, result.AsOfUtc);
            Assert.Equal(paid, result.Paid);
            Assert.Equal(paid, result.Unlimited);
            Assert.Equal(paid, result.WebsiteAccess);
            Assert.Equal(paid, result.MobileAccess);
            Assert.Single(result.Sources);
            var source = result.Sources[0];
            Assert.Equal("website", source.Provider);
            Assert.Equal(type, source.SubscriptionType);
            Assert.Equal("active", source.State);
            Assert.Equal(type == MembershipOption.ESubscriptionType.Free || type == MembershipOption.ESubscriptionType.LifeTimePlatinum
                ? null : membership.EndsOn.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), source.EndsOn);
            Assert.Null(source.AutoRenewEnabled);
            Assert.Null(source.ExpiresAtUtc);
            Assert.Null(source.ProductId);
            Assert.Null(source.NextProductId);
            Assert.Null(source.NextSubscriptionType);
            Assert.Null(source.ChangeEffectiveAtUtc);
            Assert.Null(source.VerifiedAtUtc);
        }

        [Theory]
        [InlineData("missing")]
        [InlineData("expired")]
        [InlineData("future")]
        [InlineData("deactivated")]
        [InlineData("other-user")]
        [InlineData("missing-option")]
        [InlineData("invalid-option")]
        public void IneligibleOrUnownedMembershipCannotGrantAccess(string reason)
        {
            var membership = Membership(MembershipOption.ESubscriptionType.MonthlyPlatinum);
            switch (reason)
            {
                case "missing": membership = null; break;
                case "expired": membership.EndsOn = DateTime.Today.AddDays(-1); break;
                case "future": membership.StartsOn = DateTime.Today.AddDays(1); break;
                case "deactivated": membership.IsDeactivated = true; break;
                case "other-user": membership.UserId = 43; break;
                case "missing-option": membership.MembershipOption = null; break;
                case "invalid-option": membership.MembershipOption.SubscriptionType = (MembershipOption.ESubscriptionType)999; break;
            }
            var result = MobileEntitlementsMapper.Map(42, membership, DateTime.UtcNow);
            Assert.False(result.Paid);
            Assert.False(result.Unlimited);
            Assert.False(result.WebsiteAccess);
            Assert.False(result.MobileAccess);
            Assert.Empty(result.Sources);
        }

        [Fact]
        public void LegacyEndDateRemainsInclusiveAndLifetimeRemainsUnbounded()
        {
            var membership = Membership(MembershipOption.ESubscriptionType.MonthlyPlatinum);
            membership.EndsOn = DateTime.Today;
            Assert.True(MobileEntitlementsMapper.Map(42, membership, DateTime.UtcNow).Paid);
            membership.EndsOn = DateTime.Today.AddDays(-1);
            Assert.False(MobileEntitlementsMapper.Map(42, membership, DateTime.UtcNow).Paid);
            membership.MembershipOption.SubscriptionType = MembershipOption.ESubscriptionType.LifeTimePlatinum;
            var lifetime = MobileEntitlementsMapper.Map(42, membership, DateTime.UtcNow);
            Assert.True(lifetime.Paid);
            Assert.Single(lifetime.Sources);
            Assert.Null(lifetime.Sources[0].EndsOn);
            membership.IsDeactivated = true;
            Assert.False(MobileEntitlementsMapper.Map(42, membership, DateTime.UtcNow).Paid);
        }

        private static UserMembership Membership(MembershipOption.ESubscriptionType type)
        {
            return new UserMembership
            {
                UserId = 42, StartsOn = DateTime.Today.AddDays(-1), EndsOn = DateTime.Today.AddDays(2),
                IsAutoRenew = true, MembershipOption = new MembershipOption { SubscriptionType = type }
            };
        }
    }
}
