using System.Linq;
using K9.SharedLibrary.Helpers;
using Xunit;

namespace K9.SharedLibrary.Tests.Helpers
{
    public class LinkPreviewerTests
    {
        [Theory]
        [InlineData(true, "<img src=\"https://blah.com/images/oops.jpg\" />", "https://blah.com/images/oops.jpg")]
        [InlineData(true, "<img src=\"https://blah.com/images/oops.jpg\" /><img src=\"https://blah.com/images/oops2.jpg\" />", "https://blah.com/images/oops2.jpg")]
        [InlineData(false, "<img src=\"https://blah.com/images/oops.jpg\" /><img src=\"https://blah.com/images/oops2.jpg\" />", "https://blah.com/images/oops3.jpg")]
        [InlineData(true, "<div style=\"background-image: url('https://blessingsoftheforest.org/Images/shared/blessings_loader.gif');\"></div>", "https://blessingsoftheforest.org/Images/shared/blessings_loader.gif")]
        public void GetOtherImages_ReturnsSrcOfImg(bool result, string html, string src)
        {
            var actualSrces = LinkPreviewer.GetOtherImages(html).ToList();
            Assert.Equal(result, actualSrces.Contains(src));
        }
    }
}
