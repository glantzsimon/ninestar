using K9.WebApplication.Controllers;
using K9.WebApplication.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K9.WebApplication.Tests.Unit
{
	[TestClass]
	public class ExtensionTests
	{
		
		[TestMethod]
		public void GetControllerName_ShouldReturnNameOfController_ForUseInRouting()
		{
			Assert.AreEqual("Messages", typeof(MessagesController).GetControllerName());
		}

	}

}
