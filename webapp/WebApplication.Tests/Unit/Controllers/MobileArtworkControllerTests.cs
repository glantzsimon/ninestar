using K9.WebApplication.Controllers;
using K9.WebApplication.Helpers;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Controllers
{
    public class MobileArtworkControllerTests
    {
        [Fact]
        public void AnonymousGetReturnsPublicCatalogueAndConditionalGetReturns304()
        {
            var fixture = new Fixture();
            var result = Assert.IsType<ContentResult>(fixture.Controller.Artwork());
            Assert.Equal("application/json", result.ContentType);
            var data = JObject.Parse(result.Content);
            Assert.True((bool)data["success"]);
            Assert.NotNull(data["data"]["assets"]["ninestar.energies.water"]);
            fixture.Cache.Verify(e => e.SetCacheability(HttpCacheability.Public), Times.Once);
            fixture.Cache.Verify(e => e.SetMaxAge(TimeSpan.FromMinutes(5)), Times.Once);
            fixture.Cache.Verify(e => e.SetNoStore(), Times.Never);
            fixture.Headers["If-None-Match"] = "W/\"" + (string)data["data"]["version"] + "\"";
            Assert.Equal(304, Assert.IsType<HttpStatusCodeResult>(fixture.Controller.Artwork()).StatusCode);
            fixture.Controller.ImageBase = "https://fallback.example/ninestar/Images";
            Assert.IsType<ContentResult>(fixture.Controller.Artwork());
        }

        [Fact]
        public void MissingBaseReturnsUncacheableJson503()
        {
            var fixture = new Fixture();
            fixture.Controller.ImageBase = null;
            var result = Assert.IsType<ContentResult>(fixture.Controller.Artwork());
            fixture.Response.VerifySet(e => e.StatusCode = 503);
            fixture.Cache.Verify(e => e.SetNoStore(), Times.Once);
            fixture.Cache.Verify(e => e.SetCacheability(HttpCacheability.Public), Times.Never);
            Assert.Equal("artwork_unavailable", (string)JObject.Parse(result.Content)["error"]["code"]);
        }

        [Theory]
        [InlineData(HttpCacheability.NoCache)]
        [InlineData(HttpCacheability.ServerAndPrivate)]
        public void RealCachePolicyStaysPublicThroughRequestDefaultActionAndConditionalGet(HttpCacheability defaultPolicy)
        {
            // Real ASP.NET cache policy, no IIS/Application_Start/database. Mocked
            // cache setters alone cannot detect its more-restrictive-policy rule.
            using (var output = new StringWriter())
            {
                var response = new HttpResponse(output);
                var request = new Mock<HttpRequestBase>();
                var headers = new NameValueCollection();
                request.SetupGet(e => e.HttpMethod).Returns("GET");
                request.SetupGet(e => e.AppRelativeCurrentExecutionFilePath).Returns("~/api/mobile/v1/metadata/artwork");
                request.SetupGet(e => e.PathInfo).Returns(string.Empty);
                request.SetupGet(e => e.Headers).Returns(headers);
                var context = new Mock<HttpContextBase>();
                context.SetupGet(e => e.Request).Returns(request.Object);
                context.SetupGet(e => e.Response).Returns(new HttpResponseWrapper(response));
                var controller = new TestController();
                controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

                MobileArtworkCacheHelper.ApplyRequestDefault(context.Object, defaultPolicy);
                var result = Assert.IsType<ContentResult>(controller.Artwork());
                Assert.Equal(HttpCacheability.Public, response.Cache.GetCacheability());

                Assert.Equal(TimeSpan.FromMinutes(5), response.Cache.GetMaxAge());
                Assert.False(response.Cache.GetNoStore());
                var version = (string)JObject.Parse(result.Content)["data"]["version"];
                Assert.Equal("W/\"" + version + "\"", response.Cache.GetETag());
                // A conditional GET is a new request/response. ASP.NET rejects
                // setting the ETag twice on the same HttpCachePolicy instance.
                response = new HttpResponse(output);
                context.SetupGet(e => e.Response).Returns(new HttpResponseWrapper(response));
                headers["If-None-Match"] = "W/\"" + version + "\"";
                MobileArtworkCacheHelper.ApplyRequestDefault(context.Object, defaultPolicy);
                Assert.Equal(304, Assert.IsType<HttpStatusCodeResult>(controller.Artwork()).StatusCode);
                Assert.Equal(HttpCacheability.Public, response.Cache.GetCacheability());
            }
        }

        [Theory]
        [InlineData(HttpCacheability.NoCache)]
        [InlineData(HttpCacheability.ServerAndPrivate)]
        public void RealCachePolicyReproducesWhyRestrictiveBeginRequestDefaultCannotBeRelaxed(HttpCacheability defaultPolicy)
        {
            using (var output = new StringWriter())
            {
                var response = new HttpResponse(output);
                response.Cache.SetCacheability(defaultPolicy);
                response.Cache.SetCacheability(HttpCacheability.Public);
                Assert.Equal(defaultPolicy, response.Cache.GetCacheability());
            }
        }

        private sealed class Fixture
        {
            public readonly TestController Controller = new TestController();
            public readonly NameValueCollection Headers = new NameValueCollection();
            public readonly Mock<HttpCachePolicyBase> Cache = new Mock<HttpCachePolicyBase>();
            public readonly Mock<HttpResponseBase> Response = new Mock<HttpResponseBase>();
            public Fixture()
            {
                var request = new Mock<HttpRequestBase>();
                request.SetupGet(e => e.Headers).Returns(Headers);
                Response.SetupGet(e => e.Cache).Returns(Cache.Object);
                var context = new Mock<HttpContextBase>();
                context.SetupGet(e => e.Request).Returns(request.Object);
                context.SetupGet(e => e.Response).Returns(Response.Object);
                Controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), Controller);
            }
        }

        private sealed class TestController : MobileArtworkController
        {
            public string ImageBase = "https://media.example/Images";
            protected override string GetBaseImagesPath() => ImageBase;
        }
    }
}
