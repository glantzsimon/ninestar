using K9.DataAccess.Models;
using K9.SharedLibrary.Enums;
using Xunit;

namespace K9.SharedLibrary.Tests.Unit
{
	public class FileSourceTests
	{
		[Fact]
		public void FileSource_GetPathTest()
		{
			var newsItem = new NewsItem();
			
			Assert.Equal("Images/news/upload/NewsItem/0", newsItem.ImageFileSource.PathToFiles);
			Assert.Equal(EFilesSourceFilter.Images, newsItem.ImageFileSource.Filter);

			newsItem.Id = 2;
			Assert.Equal("Images/news/upload/NewsItem/2", newsItem.ImageFileSource.PathToFiles);
		}

	}
}
