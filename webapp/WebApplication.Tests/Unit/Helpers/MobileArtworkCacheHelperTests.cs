using K9.WebApplication.Helpers;
using Moq;
using System.Web;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Helpers
{
    public class MobileArtworkCacheHelperTests
    {
        [Theory]
        [InlineData("GET", "~/api/mobile/v1/metadata/artwork", "", true)]
        [InlineData("GET", "~/api/mobile/v1/metadata/artwork/", "", true)]
        [InlineData("GET", "~/API/MOBILE/V1/METADATA/ARTWORK", "", true)]
        [InlineData("POST", "~/api/mobile/v1/metadata/artwork", "", false)]
        [InlineData("HEAD", "~/api/mobile/v1/metadata/artwork", "", false)]
        [InlineData("GET", "~/api/mobile/v1/metadata/artwork/private", "", false)]
        [InlineData("GET", "~/api/mobile/v1/metadata/artwork", "/private", false)]
        [InlineData("GET", "~/api/mobile/v1/metadata/artwork-other", "", false)]
        [InlineData("GET", "~/api/mobile/v1/metadata/options", "", false)]
        [InlineData("GET", "~/api/mobile/v1/account/me", "", false)]
        [InlineData("GET", "~/api/mobile/v1/membership/entitlements", "", false)]
        [InlineData("GET", "~/api/mobile/v1/media/images", "", false)]
        [InlineData("GET", "~/personal-chart", "", false)]
        [InlineData("GET", null, "", false)]
        public void OnlyExactPublicArtworkGetSkipsDebugAndReleaseDefaults(
            string method, string path, string pathInfo, bool exempt)
        {
            foreach (var defaultPolicy in new[] { HttpCacheability.NoCache, HttpCacheability.ServerAndPrivate })
            {
                var request = new Mock<HttpRequestBase>();
                request.SetupGet(e => e.HttpMethod).Returns(method);
                request.SetupGet(e => e.AppRelativeCurrentExecutionFilePath).Returns(path);
                request.SetupGet(e => e.PathInfo).Returns(pathInfo);
                var cache = new Mock<HttpCachePolicyBase>(MockBehavior.Strict);
                if (!exempt) cache.Setup(e => e.SetCacheability(defaultPolicy));
                var response = new Mock<HttpResponseBase>();
                response.SetupGet(e => e.Cache).Returns(cache.Object);
                var context = new Mock<HttpContextBase>();
                context.SetupGet(e => e.Request).Returns(request.Object);
                context.SetupGet(e => e.Response).Returns(response.Object);

                MobileArtworkCacheHelper.ApplyRequestDefault(context.Object, defaultPolicy);

                cache.Verify(e => e.SetCacheability(defaultPolicy), exempt ? Times.Never() : Times.Once());
                cache.Verify(e => e.SetCacheability(HttpCacheability.Public), Times.Never());
                cache.Verify(e => e.SetNoStore(), Times.Never());
            }
        }
    }
}
