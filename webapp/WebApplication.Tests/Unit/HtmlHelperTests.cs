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
		private MemoryStream _stream;

		public string GetHtmlOutput()
		{
			var reader = new StreamReader(_stream);
			return reader.ReadToEnd();
		}

		private HtmlHelper CreateHtmlHelper(ViewDataDictionary viewDataDictionary = null)
		{
			viewDataDictionary = viewDataDictionary ?? new ViewDataDictionary(new{});
			_stream = new MemoryStream();
			var textWriter = new StreamWriter(_stream);
			var mockViewContext = new Mock<ViewContext>(
				new ControllerContext(
					new Mock<HttpContextBase>().Object,
					new RouteData(),
					new Mock<ControllerBase>().Object
				),
				new Mock<IView>().Object,
				viewDataDictionary,
				new TempDataDictionary(),
				textWriter
			);
			mockViewContext.Setup(vc => vc.Writer).Returns(textWriter);

			var mockDataContainer = new Mock<IViewDataContainer>();
			mockDataContainer.Setup(c => c.ViewData).Returns(viewDataDictionary);

			return new HtmlHelper(mockViewContext.Object, mockDataContainer.Object);
		}

		[TestMethod]
		public void EditorFor_ShouldRenderCorrectly()
		{
			var html = CreateHtmlHelper();

			html.ViewContext.Writer.WriteLine(("test"));

			Assert.AreEqual("test", GetHtmlOutput());
		}
	}
}
