using System;
using System.IO;
using System.Web;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using Moq;
using Xunit;

namespace K9.SharedLibrary.Tests.Unit
{
	public class PostedFileHelperTests
	{

		[Fact]
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

			Assert.Equal(expectedPath, result);
			Assert.Equal(result, postedFilePath);
		}

		[Fact]
		public void SavePostedFileToRelativePath_ShouldThrowError_IfPostedFileIsNull()
		{
			var helper = new PostedFileHelper();

			try
			{
				helper.SavePostedFileToRelativePath(null, "Images/news/upload");
			}
			catch (Exception ex)
			{
			    Assert.IsType<NullReferenceException>(ex);
            }
		}

		[Fact]
		public void SavePostedFileToDisk_ShouldThrowError_IfPostedFileIsNull()
		{
			var helper = new PostedFileHelper();

			try
			{
				helper.SavePostedFileToPath(null, "Images/news/upload");
			}
			catch (Exception ex)
			{
				Assert.IsType<NullReferenceException>(ex);
			}
		}

	}
}
