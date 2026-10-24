using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using Moq;
using System;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class MobileImageBoundaryTests
    {
        [Theory]
        [InlineData("/Images/mobile/avatars/43/0123456789abcdef0123456789abcdef.png")]
        [InlineData("/Images/mobile/avatars/42/../../web.config")]
        [InlineData("/Images/mobile/avatars/42/shell.aspx")]
        [InlineData("/Images/mobile/articles/0123456789abcdef0123456789abcdef.png")]
        [InlineData("https://example.com/image.png")]
        public void AvatarDeletionRejectsOtherOwnersAndArbitraryPaths(string path)
        {
            var service = new MediaManagementService(new Mock<INineStarKiBasePackage>().Object);
            Assert.Throws<ArgumentException>(() => service.DeleteMobileImage(path, 42));
        }

        [Fact]
        public void NonImagesAndOversizedUploadsAreRejectedBeforeDecodingOrWriting()
        {
            var service = new MediaManagementService(new Mock<INineStarKiBasePackage>().Object);
            Assert.Throws<ArgumentException>(() => service.SaveMobileImage(new byte[8], 42));
            Assert.Throws<ArgumentException>(() => service.SaveMobileImage(new byte[2 * 1024 * 1024 + 1], 42));
        }
    }
}
