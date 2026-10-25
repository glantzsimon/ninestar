using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using Moq;
using System;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class MobilePreferenceBoundaryTests
    {
        [Theory]
        [InlineData("MobileCheckout:0123456789abcdef")]
        [InlineData("mobilecheckout:0123456789abcdef")]
        [InlineData("ＭobileCheckout：0123456789abcdef")]
        [InlineData("MobileCheckout:0123456789abcdef ")]
        public void PublicPreferenceWritesCannotOverwriteOrReplayInternalCheckoutRecords(string key)
        {
            // A null repository makes any accidental persistence attempt fail;
            // the reserved key must be rejected before touching infrastructure.
            var service = new UserService(new Mock<INineStarKiBasePackage>().Object,
                null, null, null, null, null, null, null, null, null);
            var error = Assert.Throws<ArgumentException>(() => service.UpdateUserPreference(42, key, "forged checkout"));
            Assert.Equal("key", error.ParamName);
        }
    }
}
