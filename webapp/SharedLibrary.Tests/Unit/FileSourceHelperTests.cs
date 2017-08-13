using System;
using System.IO;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace K9.SharedLibrary.Tests.Unit
{
	[TestClass]
	public class FileSourceHelperTests
	{
		
		Mock<IPostedFileHelper> _postedFileHelper = new Mock<IPostedFileHelper>();

		[TestMethod]
		public void LoadFiles_ShouldThrowAnError_WhenPathDoesnotExist()
		{
			var helper = new FileSourceHelper(_postedFileHelper.Object);

			try
			{
				helper.LoadFiles(new FileSource
				{
					PathToFiles = "nonexistant/path"
				});
			}
			catch (Exception ex)
			{
				Assert.IsInstanceOfType(ex, typeof(DirectoryNotFoundException));
			}
		}

		[TestMethod]
		public void LoadFiles_ShouldThrowAnError_WhenPathDoesnotExistAndTryingToLoadFiles()
		{
			var helper = new FileSourceHelper(_postedFileHelper.Object);

			helper.LoadFiles(new FileSource
			{
				PathToFiles = "nonexistant/path"
			}, false);
		}

	}

}
