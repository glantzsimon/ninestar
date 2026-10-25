using K9.WebApplication.Helpers;
using Newtonsoft.Json.Linq;
using Moq;
using System.Web.Mvc;
using System;
using System.Web;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Controllers
{
    public class MobileApiErrorTests
    {
        [Theory]
        [InlineData("~/api/mobile/v1", true)]
        [InlineData("~/api/mobile/v1/missing", true)]
        [InlineData("~/API/MOBILE/V1/account/me", true)]
        [InlineData("~/api/mobile/v10/account/me", false)]
        [InlineData("~/personal-chart/my-free-chart", false)]
        [InlineData("~/api/get-personal-chart", false)]
        [InlineData(null, false)]
        public void PipelineHandlingIsConfinedToTheMobileNamespace(string path, bool expected)
        {
            Assert.Equal(expected, MobileApiErrorHelper.IsMobilePath(path));
        }

        [Fact]
        public void WebsiteErrorFilterDoesNotReplaceMobileFailuresWithHtml()
        {
            var http = new Mock<HttpContextBase>();
            http.SetupGet(e => e.Request.AppRelativeCurrentExecutionFilePath)
                .Returns("~/api/mobile/v1/metadata/options");
            var context = new ExceptionContext
            {
                HttpContext = http.Object,
                Exception = new Exception("private diagnostic content")
            };
            new K9.WebApplication.Services.CustomExceptionFilter().OnException(context);
            Assert.False(context.ExceptionHandled);
            Assert.False(context.Result is ViewResult);
        }

        [Fact]
        public void ErrorEnvelopesDoNotIncludeExceptionDetails()
        {
            var error = new HttpException(404, "private diagnostic content");
            var status = MobileApiErrorHelper.Status(error);
            var json = JObject.Parse(MobileApiErrorHelper.Json(status));
            Assert.Equal(404, status);
            Assert.Equal("endpoint_not_found", (string)json["error"]["code"]);
            Assert.Equal(JTokenType.Null, json["data"].Type);
            Assert.DoesNotContain("private diagnostic", json.ToString());
            Assert.Equal(500, MobileApiErrorHelper.Status(new Exception("private diagnostic content")));
        }
    }
}
