using K9.WebApplication.Controllers;
using K9.WebApplication.Packages;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Controllers
{
    public class MobileAccessBoundaryTests
    {
        public static IEnumerable<object[]> ProtectedActions()
        {
            var protectedControllers = new[]
            {
                typeof(MobileAdminArticlesController), typeof(MobileAdminContentController),
                typeof(MobileAdminDataController), typeof(MobileAdminMailController),
                typeof(MobileAdminOperationsController), typeof(MobileAdminPromotionsController),
                typeof(MobileAdminUsersController), typeof(MobileMediaController), typeof(MobileMessagesController),
                typeof(MobileReadingsController), typeof(MobileBiorhythmsController),
                typeof(MobileCompatibilityController), typeof(MobileNumerologyController),
                typeof(MobilePredictionsController), typeof(MobileCalendarController),
                typeof(MobileMembershipController), typeof(MobilePaymentsController), typeof(MobileReportsController),
                typeof(MobileConsultationsController), typeof(MobilePersonalChartController), typeof(MobileBlogController)
            };
            foreach (var type in protectedControllers)
                foreach (var action in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(e => e.GetCustomAttribute<RouteAttribute>() != null))
                {
                    if (type == typeof(MobilePersonalChartController) && action.Name == "Preview") continue;
                    if (type == typeof(MobileBlogController) && new[] { "Index", "Article", "Tags" }.Contains(action.Name)) continue;
                    yield return new object[] { type, action.Name };
                }
        }

        [Theory, MemberData(nameof(ProtectedActions))]
        public async Task AnonymousRequestsCannotReachProtectedDependencies(Type controllerType, string actionName)
        {
            // No service property may be accessed before authentication. A strict
            // package and null secondary dependencies expose any such regression.
            var package = new Mock<INineStarKiPackage>(MockBehavior.Strict);
            var constructor = controllerType.GetConstructors().Single();
            var arguments = constructor.GetParameters().Select(e => e.ParameterType == typeof(INineStarKiPackage)
                ? (object)package.Object : e.ParameterType == typeof(IOptions<StripeConfiguration>)
                    ? new Mock<IOptions<StripeConfiguration>>().Object : null).ToArray();
            var controller = (Controller)constructor.Invoke(arguments);
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(e => e.IsAuthenticated).Returns(false);
            var response = new Mock<HttpResponseBase>();
            response.SetupProperty(e => e.StatusCode);
            response.SetupGet(e => e.Cache).Returns(new Mock<HttpCachePolicyBase>().Object);
            var context = new Mock<HttpContextBase>();
            context.SetupGet(e => e.Request).Returns(request.Object);
            context.SetupGet(e => e.Response).Returns(response.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            var action = controllerType.GetMethod(actionName);
            var inputs = action.GetParameters().Select(e => e.HasDefaultValue ? e.DefaultValue :
                e.ParameterType.IsValueType ? Activator.CreateInstance(e.ParameterType) : null).ToArray();
            var output = action.Invoke(controller, inputs);
            var pending = output as Task<ActionResult>;
            var result = Assert.IsType<ContentResult>(pending == null ? output : await pending);
            Assert.True(response.Object.StatusCode == 401 || response.Object.StatusCode == 403);
            var json = JObject.Parse(result.Content);
            Assert.False((bool)json["success"]);
            Assert.Equal(JTokenType.Null, json["data"].Type);
        }
    }
}
