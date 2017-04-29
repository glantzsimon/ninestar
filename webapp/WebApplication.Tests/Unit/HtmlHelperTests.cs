using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Http.Routing;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace K9.WebApplication.Tests.Unit
{
	[TestClass]
	public class HtmlHelperTests
	{
		private Mock<HtmlHelper> _html;
		private Mock<HttpContextBase> _httpContext;
		private Mock<ControllerBase> _controller;
		private RouteData _routeData;
		private Mock<IView> _view;
		private ViewDataDictionary _viewData;
		private ControllerContext _controllerContext;
		private Mock<TextWriter> _textWriter;
		private TempDataDictionary _tempData;
		private ViewContext _viewContext;

		public HtmlHelperTests()
		{
			_routeData = new RouteData();
			_httpContext = new Mock<HttpContextBase>();
			_controller = new Mock<ControllerBase>();
			_view = new Mock<IView>();
			_viewData = new ViewDataDictionary();
			_controllerContext = new ControllerContext(_httpContext.Object, _routeData, _controller.Object);
			_tempData = new TempDataDictionary();
			_textWriter =  new Mock<TextWriter>(MockBehavior.Strict);
			
			_viewContext = new ViewContext(_controllerContext, _view.Object, _viewData, _tempData, _textWriter.Object);

			_html.Setup(h => h.ViewContext)
				.Returns(new ViewContext(_controllerContext, _view.Object, _viewData, _tempData, _textWriter.Object));

			_textWriter.Setup(t => t.WriteLine(It.IsAny<string>()))
				.Callback(x => )
				
		}

		[TestMethod]
		public void EditorFor_ShouldRenderCorrectly()
		{
		}
	}
}
