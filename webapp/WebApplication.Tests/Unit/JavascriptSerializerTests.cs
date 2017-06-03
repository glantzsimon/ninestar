using System.Collections.Generic;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K9.WebApplication.Tests.Unit
{
	[TestClass]
	public class JavascriptSerializerTests
	{

		[TestMethod]
		public void OutputOfJavascriptSerializer_ShouldBeArrayOfString()
		{
			var strings = new List<string>
			{
				"one",
				"two",
				"three"
			};

			var jsArray = new JavaScriptSerializer().Serialize(strings.ToArray());

			Assert.AreEqual("[\"one\",\"two\",\"three\"]", jsArray);
		}

	}

}
