using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using Xunit;

namespace K9.SharedLibrary.Tests.Helpers
{
    public class TestModel
    {
        public int UserId { get; set; }
        public int PractitionerUserId { get; set; }
        public string Name { get; set; }
        public int GuestUserId { get; set; }
    }

    public class ExtensionsTests
    {
        [Fact]
        public void GetUserIdProperties_Test()
        {
            Assert.Equal(3, typeof(TestModel).GetUserIdProperties().Count);
        }
    }
}
