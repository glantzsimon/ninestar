using K9.WebApplication.Controllers;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
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
