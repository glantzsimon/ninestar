using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using System.Linq;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Models
{
    public class HexagramIsolationTests
    {
        [Fact]
        public void ChangingLinesNeverLeakIntoAnotherReadingOfTheSameHexagram()
        {
            var changing = new Hexagram(Enumerable.Repeat(ELineType.ChangingYang, 6).ToArray());
            var stable = new Hexagram(Enumerable.Repeat(ELineType.Yang, 6).ToArray());
            Assert.Equal(changing.Number, stable.Number);
            var changingInfo = changing.GetHexagramInfo();
            var stableInfo = stable.GetHexagramInfo();
            Assert.NotSame(changingInfo, stableInfo);
            Assert.Null(stableInfo.ChangingLinesInterpretation);
            changingInfo.Summary = "request-local change";
            Assert.NotEqual(changingInfo.Summary, stable.GetHexagramInfo().Summary);
        }
    }
}
