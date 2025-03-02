using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class IChingServiceTests
    {
        public IChingServiceTests()
        {
        }

        [Fact]
        public void IChing_Generator_Tests()
        {
            var hex1 = new Hexagram(new[] {
                ELineType.Yang, ELineType.Yang, ELineType.Yang,
                ELineType.Yang, ELineType.Yang, ELineType.Yang
            });
            Assert.Equal(1, hex1.Number);
            Assert.Equal("The Creative", hex1.Name);
            
            // Test Hexagram 2: All Yin
            var hex2 = new Hexagram(new[] {
                ELineType.Yin, ELineType.Yin, ELineType.Yin,
                ELineType.Yin, ELineType.Yin, ELineType.Yin
            });
            Assert.Equal(2, hex2.Number);
            Assert.Equal("The Receptive", hex2.Name);
            
            // Test Hexagram 3: Mixed Yin/Yang
            var hex3 = new Hexagram(new[] {
                ELineType.Yang, ELineType.Yin, ELineType.Yin,
                ELineType.Yang, ELineType.Yin, ELineType.Yang
            });
            Assert.Equal(3, hex3.Number);
            Assert.Equal("Difficulty at the Beginning", hex3.Name);
        }
        
    }
}
