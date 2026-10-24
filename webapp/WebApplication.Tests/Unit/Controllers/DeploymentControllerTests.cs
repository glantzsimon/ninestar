using K9.Base.WebApplication.Config;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Controllers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using Moq;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Controllers
{
    public class DeploymentControllerTests
    {
        [Fact]
        public void TargetPreflightIsReadOnlyAndUploadStaysInIntegrationVault()
        {
            using (var fixture = new Fixture())
            {
                var result = JObject.FromObject(fixture.Controller.DeploymentTarget("int").Data);
                Assert.True((bool)result["success"]);
                Assert.Equal("int", (string)result["environment"]);
                Assert.False(Directory.Exists(fixture.Integration));
                Assert.False((bool)JObject.FromObject(fixture.Controller.DeploymentTarget("prod").Data)["success"]);
                Assert.Equal(400, fixture.Response.Object.StatusCode);
                var file = new Mock<HttpPostedFileBase>();
                file.SetupGet(e => e.FileName).Returns("nested/site.css");
                file.SetupGet(e => e.ContentLength).Returns(3);
                file.Setup(e => e.SaveAs(It.IsAny<string>())).Callback<string>(path => File.WriteAllText(path, "css"));
                result = JObject.FromObject(fixture.Controller.UploadFile(file.Object, "int", "css").Data);
                Assert.True((bool)result["success"]);
                Assert.Equal("css", File.ReadAllText(Path.Combine(fixture.Integration, "css", "nested", "site.css")));
                Assert.False(Directory.Exists(Path.Combine(fixture.Root, "css")));
                fixture.Cache.Verify(e => e.SetNoStore(), Times.AtLeastOnce);
            }
        }

        [Fact]
        public void FailureResponsesHaveFailureStatusAndNoSuccessfulAcknowledgement()
        {
            using (var fixture = new Fixture())
            {
                Assert.False((bool)JObject.FromObject(fixture.Controller.UploadFile(null, "int").Data)["success"]);
                Assert.Equal(400, fixture.Response.Object.StatusCode);
                Assert.False((bool)JObject.FromObject(fixture.Controller.DeploymentTarget("typo").Data)["success"]);
                Assert.Equal(400, fixture.Response.Object.StatusCode);
                File.WriteAllText(Path.Combine(fixture.Root, "deploy.ps1"), "old script");
                Assert.False((bool)JObject.FromObject(fixture.Controller.DeploymentTarget("int").Data)["success"]);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("x")]
        [InlineData("Bearer test-key")]
        [InlineData("ApiKey incorrect")]
        public void InvalidKeysCannotStartMaintenanceOrUpload(string header)
        {
            using (var fixture = new Fixture())
            {
                fixture.Headers["Authorization"] = header;
                Assert.False((bool)JObject.FromObject(fixture.Controller.RunMaintenanceScript("int").Data)["success"]);
                Assert.Equal(401, fixture.Response.Object.StatusCode);
                Assert.False((bool)JObject.FromObject(fixture.Controller.UploadFile(null, "int").Data)["success"]);
                Assert.Equal(401, fixture.Response.Object.StatusCode);
            }
        }

        private sealed class Fixture : IDisposable
        {
            private readonly HttpContext _previous;
            public readonly string Root = Path.Combine(Path.GetTempPath(), "ninestar-deploy-" + Guid.NewGuid().ToString("N"));
            public string Integration => Path.Combine(Root, "int");
            public readonly NameValueCollection Headers = new NameValueCollection { { "Authorization", "ApiKey test-key" } };
            public readonly Mock<HttpResponseBase> Response = new Mock<HttpResponseBase>();
            public readonly Mock<HttpCachePolicyBase> Cache = new Mock<HttpCachePolicyBase>();
            public readonly ApiController Controller;
            public Fixture()
            {
                _previous = HttpContext.Current;
                HttpContext.Current = Helpers.Helpers.CreateHttpContextWithSession();
                Directory.CreateDirectory(Root);
                File.WriteAllText(Path.Combine(Root, "deploy.ps1"), "# NINESTAR_DEPLOY_V2");
                var package = new Mock<INineStarKiPackage>();
                package.SetupGet(e => e.Logger).Returns(new Mock<ILogger>().Object);
                package.SetupGet(e => e.DataSetsHelper).Returns(new Mock<IDataSetsHelper>().Object);
                package.SetupGet(e => e.Roles).Returns(new Mock<IRoles>().Object);
                package.SetupGet(e => e.Authentication).Returns(new Mock<IAuthentication>().Object);
                package.SetupGet(e => e.FileSourceHelper).Returns(new Mock<IFileSourceHelper>().Object);
                package.SetupGet(e => e.ApiConfiguration).Returns(new ApiConfiguration { ApiKey = "test-key" });
                package.SetupGet(e => e.DefaultValuesConfiguration).Returns(new DefaultValuesConfiguration
                {
                    VaultPath = Root, VaultIntPath = Integration, VaultDeployScript = "deploy.ps1", SiteIntBaseUrl = "int.example"
                });
                Controller = new ApiController(package.Object, new Mock<INineStarKiService>().Object, new Mock<IIChingService>().Object);
                var request = new Mock<HttpRequestBase>();
                request.SetupGet(e => e.Headers).Returns(Headers);
                request.SetupGet(e => e.Url).Returns(new Uri("https://int.example/api/deploy/target"));
                Response.SetupProperty(e => e.StatusCode);
                Response.SetupGet(e => e.Cache).Returns(Cache.Object);
                var context = new Mock<HttpContextBase>();
                context.SetupGet(e => e.Request).Returns(request.Object);
                context.SetupGet(e => e.Response).Returns(Response.Object);
                Controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), Controller);
            }
            public void Dispose()
            {
                HttpContext.Current = _previous;
                Directory.Delete(Root, true);
            }
        }
    }
}
