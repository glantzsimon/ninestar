using K9.DataAccessLayer.Models;
using Xunit;

namespace K9.DataAccessLayer.Tests.Unit
{

    public class ModelTests
	{
		[Fact]
		public void MembershipOption_Upgrades()
		{
            var monthlyStandard = new MembershipOption
            {
                SubscriptionType = MembershipOption.ESubscriptionType.MonthlyStandard
            };

		    var monthlyPlatinum = new MembershipOption
		    {
		        SubscriptionType = MembershipOption.ESubscriptionType.MonthlyPlatinum
		    };

		    var annualStandard = new MembershipOption
		    {
		        SubscriptionType = MembershipOption.ESubscriptionType.AnnualStandard
		    };

		    var annualPlatinum = new MembershipOption
		    {
		        SubscriptionType = MembershipOption.ESubscriptionType.AnnualPlatinum
		    };

            Assert.True(monthlyStandard.CanUpgradeTo(annualStandard));
            Assert.True(monthlyStandard.CanUpgradeTo(monthlyPlatinum));
            Assert.True(monthlyStandard.CanUpgradeTo(annualPlatinum));
            Assert.False(monthlyStandard.CanUpgradeTo(monthlyStandard));

		    Assert.True(monthlyPlatinum.CanUpgradeTo(annualPlatinum));
		    Assert.False(monthlyPlatinum.CanUpgradeTo(monthlyStandard));
		    Assert.False(monthlyPlatinum.CanUpgradeTo(annualStandard));
		    Assert.False(monthlyPlatinum.CanUpgradeTo(monthlyPlatinum));

		    Assert.True(annualStandard.CanUpgradeTo(annualPlatinum));
		    Assert.False(annualStandard.CanUpgradeTo(monthlyStandard));
		    Assert.False(annualStandard.CanUpgradeTo(monthlyPlatinum));
		    Assert.False(annualStandard.CanUpgradeTo(annualStandard));

		    Assert.False(annualPlatinum.CanUpgradeTo(annualStandard));
		    Assert.False(annualPlatinum.CanUpgradeTo(monthlyStandard));
		    Assert.False(annualPlatinum.CanUpgradeTo(monthlyPlatinum));
		    Assert.False(annualPlatinum.CanUpgradeTo(annualPlatinum));
		}
	}
}
