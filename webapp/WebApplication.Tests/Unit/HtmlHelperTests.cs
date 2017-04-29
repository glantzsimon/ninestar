using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using K9.DataAccess.Models;
using K9.WebApplication.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace K9.WebApplication.Tests.Unit
{
	[TestClass]
	public class HtmlHelperTests
	{
		private MemoryStream _stream;
		private StreamWriter _streamWriter;

		public string GetOutputFromStream()
		{
			_streamWriter.Flush();
			var result = Encoding.UTF8.GetString(_stream.ToArray());

			_stream.Close();
			_streamWriter.Close();

			return result;
		}

		private HtmlHelper<TModel> CreateHtmlHelper<TModel>(ViewDataDictionary viewDataDictionary = null) where TModel : class
		{
			viewDataDictionary = viewDataDictionary ?? new ViewDataDictionary(new { });
			_stream = new MemoryStream();
			_streamWriter = new StreamWriter(_stream);
			var mockViewContext = new Mock<ViewContext>(
				new ControllerContext(
					new Mock<HttpContextBase>().Object,
					new RouteData(),
					new Mock<ControllerBase>().Object
				),
				new Mock<IView>().Object,
				viewDataDictionary,
				new TempDataDictionary(),
				_streamWriter
			);
			mockViewContext.Setup(vc => vc.Writer).Returns(_streamWriter);
			var mockDataContainer = new Mock<IViewDataContainer>();
			mockDataContainer.Setup(c => c.ViewData).Returns(viewDataDictionary);
			return new HtmlHelper<TModel>(mockViewContext.Object, mockDataContainer.Object);
		}

		[TestMethod]
		public void HtmlHelper_ShouldWriteToStream()
		{
			var html = CreateHtmlHelper<ObjectBase>();

			html.ViewContext.Writer.Write("test");

			Assert.AreEqual("test", GetOutputFromStream());
		}

		[TestMethod]
		public void HtmlHelper_BootstrapEditoFor_ShouldRenderCorrectly()
		{
			var html = CreateHtmlHelper<UserAccount.LoginModel>();
			var model = new UserAccount.LoginModel();

			html.BootstrapEditorFor(m => model.UserName);

			Assert.AreEqual("<input>", GetOutputFromStream());
		}
	}
}
