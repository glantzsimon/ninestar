using K9.DataAccessLayer.Models;
using Xunit;

namespace K9.DataAccessLayer.Tests.Unit
{

    public class ModelTests
	{
        [Fact]
		public void MembershipOption_Upgrades()
		{
		    var free = new MembershipOption
		    {
		        SubscriptionType = MembershipOption.ESubscriptionType.Free
		    };

		    var weeklyPlatinum = new MembershipOption
		    {
		        SubscriptionType = MembershipOption.ESubscriptionType.WeeklyPlatinum
		    };

		    var monthlyPlatinum = new MembershipOption
		    {
		        SubscriptionType = MembershipOption.ESubscriptionType.MonthlyPlatinum
		    };

		    var yearlyPlatinum = new MembershipOption
		    {
		        SubscriptionType = MembershipOption.ESubscriptionType.AnnualPlatinum
		    };

		    var lifetimePlatinum = new MembershipOption
		    {
		        SubscriptionType = MembershipOption.ESubscriptionType.LifeTimePlatinum
		    };

            Assert.True(free.CanUpgradeTo(weeklyPlatinum));
            Assert.True(free.CanUpgradeTo(monthlyPlatinum));
            Assert.True(free.CanUpgradeTo(yearlyPlatinum));
            Assert.True(free.CanUpgradeTo(lifetimePlatinum));
            
		    Assert.True(weeklyPlatinum.CanUpgradeTo(monthlyPlatinum));
		    Assert.True(weeklyPlatinum.CanUpgradeTo(yearlyPlatinum));
		    Assert.True(weeklyPlatinum.CanUpgradeTo(lifetimePlatinum));

            Assert.True(monthlyPlatinum.CanUpgradeTo(yearlyPlatinum));
            Assert.True(monthlyPlatinum.CanUpgradeTo(lifetimePlatinum));

		    Assert.True(yearlyPlatinum.CanUpgradeTo(lifetimePlatinum));

		    Assert.False(free.CanUpgradeTo(free));
		    
		    Assert.False(weeklyPlatinum.CanUpgradeTo(free));
		    Assert.False(weeklyPlatinum.CanUpgradeTo(weeklyPlatinum));

		    Assert.False(monthlyPlatinum.CanUpgradeTo(free));
		    Assert.False(monthlyPlatinum.CanUpgradeTo(weeklyPlatinum));
		    Assert.False(monthlyPlatinum.CanUpgradeTo(monthlyPlatinum));

		    Assert.False(yearlyPlatinum.CanUpgradeTo(free));
		    Assert.False(yearlyPlatinum.CanUpgradeTo(weeklyPlatinum));
		    Assert.False(yearlyPlatinum.CanUpgradeTo(monthlyPlatinum));
		    Assert.False(yearlyPlatinum.CanUpgradeTo(yearlyPlatinum));

		    Assert.False(lifetimePlatinum.CanUpgradeTo(free));
		    Assert.False(lifetimePlatinum.CanUpgradeTo(weeklyPlatinum));
		    Assert.False(lifetimePlatinum.CanUpgradeTo(monthlyPlatinum));
		    Assert.False(lifetimePlatinum.CanUpgradeTo(yearlyPlatinum));
		    Assert.False(lifetimePlatinum.CanUpgradeTo(lifetimePlatinum));
		}
	}
}
