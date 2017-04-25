using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace K9.SharedLibrary.Helpers
{
	public class HtmlTestHelper : HtmlHelper
	{

		public string ResponseText { get; set; }

		public static HtmlTestHelper GetTestHtmlHelper()
		{
			var request = new Mock<HttpRequestBase>();
			var viewContext = new Mock<ViewContext>();
			var httpContext = new Mock<HttpContextBase>();
			var httpResponse = new Mock<HttpResponseBase>();
			
			httpContext
				.Setup(c => c.Request)
				.Returns(request.Object);

			viewContext
				.SetupGet(_ => _.HttpContext)
				.Returns(httpContext.Object);

			viewContext
				.Setup(v => v.HttpContext.Response)
				.Returns(httpResponse.Object);

			var helper = new HtmlTestHelper(viewContext.Object, new Mock<IViewDataContainer>().Object);

			httpResponse.Setup(r => r.Write(It.IsAny<string>()))
				.Callback((string s) => helper.ResponseText += s);

			return helper;
		}

		public HtmlTestHelper(ViewContext viewContext, IViewDataContainer viewDataContainer) : base(viewContext, viewDataContainer) 
		{
		}

		public HtmlTestHelper(ViewContext viewContext, IViewDataContainer viewDataContainer, RouteCollection routeCollection) : base(viewContext, viewDataContainer, routeCollection)
		{
		}
	}
}
