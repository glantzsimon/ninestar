using K9.WebApplication.Config;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class MediaServiceTests
    {
        [Fact]
        public void InitialisationUsesTheBuildEnvironmentBeforeAnyHttpRequest()
        {
            var configuration = CreateConfiguration();
#if Integration
            const string expected = "https://int.example.com/application";
#elif DEBUG && !PRODUCTION
            const string expected = "https://localhost:44370/ninestar";
#else
            const string expected = "https://www.example.com";
#endif
            var previousImages = MediaService.BaseImagesPath;
            var previousVideos = MediaService.BaseVideosPath;
            try
            {
                MediaService.InitialiseMediaPaths(configuration);
                AssertMediaPaths(configuration, expected);
            }
            finally
            {
                MediaService.BaseImagesPath = previousImages;
                MediaService.BaseVideosPath = previousVideos;
            }
        }

        [Theory]
        [InlineData("https://localhost:44370/ninestar/", "https://localhost:44370/ninestar")]
        [InlineData("int.example.com/application", "https://int.example.com/application")]
        public async Task DisabledHostingSkipsHostedSettingsAndKeepsEveryConsumerConsistent(string application, string expected)
        {
            var configuration = CreateConfiguration();
            configuration.LocalMediaBasePath = application;
            configuration.SiteIntBaseUrl = application;
            configuration.LiveMediaBasePath = application;
            // Null hosted settings make an accidental probe fail immediately.
            configuration.HostedImagesBasePath = null;
            configuration.HostedVideosBasePath = null;
            var package = new Mock<INineStarKiBasePackage>();
            package.SetupGet(value => value.DefaultValuesConfiguration).Returns(configuration);
            var service = new MediaService(package.Object);
            await service.CheckImageServiceHealthAsync();
            var previousImages = MediaService.BaseImagesPath;
            var previousVideos = MediaService.BaseVideosPath;
            try
            {
                service.ScheduledHealthCheck();
                AssertMediaPaths(configuration, expected);
            }
            finally
            {
                MediaService.BaseImagesPath = previousImages;
                MediaService.BaseVideosPath = previousVideos;
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("ftp://example.com")]
        [InlineData("https://example.com?wrong=path")]
        [InlineData("https://user:password@example.com")]
        public void InvalidMediaConfigurationFailsClearly(string application)
        {
            var configuration = CreateConfiguration();
            configuration.LocalMediaBasePath = application;
            configuration.SiteIntBaseUrl = application;
            configuration.LiveMediaBasePath = application;
            Assert.Throws<InvalidOperationException>(() => MediaService.InitialiseMediaPaths(configuration));
        }

        private static void AssertMediaPaths(DefaultValuesConfiguration configuration, string expected)
        {
            Assert.Equal(expected + "/Images", MediaService.BaseImagesPath);
            Assert.Equal(expected + "/Videos", MediaService.BaseVideosPath);
            Assert.Equal(MediaService.BaseImagesPath, configuration.BaseImagesPath);
            Assert.Equal(MediaService.BaseVideosPath, configuration.BaseVideosPath);
            Assert.Equal(MediaService.BaseImagesPath, configuration.BaseEmailTemplateImagesPath);
            Assert.Equal(MediaService.BaseVideosPath, configuration.BaseBaseEmailTemplateVideosPath);
        }

        private static DefaultValuesConfiguration CreateConfiguration()
        {
            return new DefaultValuesConfiguration
            {
                UseHostedMedia = false,
                LocalMediaBasePath = "https://localhost:44370/ninestar/",
                SiteIntBaseUrl = "int.example.com/application/",
                LiveMediaBasePath = "https://www.example.com/"
            };
        }
    }
}
