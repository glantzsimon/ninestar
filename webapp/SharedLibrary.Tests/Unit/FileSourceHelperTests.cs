using System;
using System.IO;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using Moq;
using NLog;
using Xunit;

namespace K9.SharedLibrary.Tests.Unit
{
	public class FileSourceHelperTests
	{
		
		Mock<IPostedFileHelper> _postedFileHelper = new Mock<IPostedFileHelper>();

		[Fact]
		public void LoadFiles_ShouldThrowAnError_WhenPathDoesnotExist()
		{
			var helper = new FileSourceHelper(_postedFileHelper.Object, new Mock<ILogger>().Object);

			try
			{
				helper.LoadFiles(new FileSource
				{
					PathToFiles = "nonexistant/path"
				});
			}
			catch (Exception ex)
			{
				Assert.IsType<DirectoryNotFoundException>(ex);
			}
		}

		[Fact]
		public void LoadFiles_ShouldThrowAnError_WhenPathDoesnotExistAndTryingToLoadFiles()
		{
			var helper = new FileSourceHelper(_postedFileHelper.Object, new Mock<ILogger>().Object);

			helper.LoadFiles(new FileSource
			{
				PathToFiles = "nonexistant/path"
			}, false);
		}

	}

}
