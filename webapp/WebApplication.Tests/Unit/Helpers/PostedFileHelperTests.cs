using System;
using System.IO;
using System.Web;
using K9.WebApplication.Extensions;
using K9.WebApplication.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace K9.WebApplication.Tests.Unit.Helpers
{
	[TestClass]
	public class PostedFileHelperTests
	{

		[TestMethod]
		public void SavePostedFileToRelativePath_ShouldSaveFileToDisk_AndReturnFileName()
		{
			var helper = new PostedFileHelper();
			var postedFile = new Mock<HttpPostedFileBase>();
			var postedFilePath = string.Empty;
			var fileName = "test.txt";
			var imagesNewsUploadPath = "Images/news/upload";

			postedFile.Setup(_ => _.FileName).Returns(fileName);
			postedFile.Setup(_ => _.SaveAs(It.IsAny<string>()))
				.Callback<string>((f) => postedFilePath = f);

			var expectedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagesNewsUploadPath, fileName).ToPathOnDisk();
			var result = helper.SavePostedFileToRelativePath(postedFile.Object, imagesNewsUploadPath);

			Assert.AreEqual(expectedPath, result);
			Assert.AreEqual(result, postedFilePath);
		}

		[TestMethod]
		public void SavePostedFileToRelativePath_ShouldThrowError_IfPostedFileIsNull()
		{
			var helper = new PostedFileHelper();

			try
			{
				helper.SavePostedFileToRelativePath(null, "Images/news/upload");
			}
			catch (Exception ex)
			{
				Assert.IsInstanceOfType(ex, typeof(NullReferenceException));
			}
		}

		[TestMethod]
		public void SavePostedFileToDisk_ShouldThrowError_IfPostedFileIsNull()
		{
			var helper = new PostedFileHelper();

			try
			{
				helper.SavePostedFileToPath(null, "Images/news/upload");
			}
			catch (Exception ex)
			{
				Assert.IsInstanceOfType(ex, typeof(NullReferenceException));
			}
		}

	}
}
