using K9.SharedLibrary.Helpers;
using Xunit;

namespace K9.SharedLibrary.Tests.Helpers
{
    public class HelperMethodTests
    {
        [Theory]
        [InlineData("https://www.youtube.com/watch?v=RQK5_hENKWU", "https://www.youtube.com/embed/RQK5_hENKWU")]
        public void GetEmbeddableUrl_ShouldReturnCorrectUrl(string url, string embedUrl)
        {
            var actualUrl = HelperMethods.GetEmbeddableUrl(url);
            Assert.Equal(embedUrl, actualUrl);
        }
    }
}
