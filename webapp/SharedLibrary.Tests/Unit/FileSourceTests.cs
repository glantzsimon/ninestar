using K9.DataAccess.Models;
using K9.SharedLibrary.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K9.SharedLibrary.Tests.Unit
{
	[TestClass]
	public class FileSourceTests
	{
		[TestMethod]
		public void FileSource_GetPathTest()
		{
			var newsItem = new NewsItem();
			
			Assert.AreEqual("Images/news/upload", newsItem.ImageFileSource.PathToFiles);
			Assert.AreEqual(EFilesSourceFilter.Images, newsItem.ImageFileSource.Filter);
		}

	}
}
