using System.Diagnostics;
using Xunit.Abstractions;

namespace K9.WebApplication.Tests.Unit.Helpers
{
    public class TestOutputTraceListener : TraceListener
    {
        private readonly ITestOutputHelper _output;
        public TestOutputTraceListener(ITestOutputHelper output)
        {
            _output = output;
        }

        public override void Write(string message)
        {
            _output.WriteLine(message);
        }

        public override void WriteLine(string message)
        {
            _output.WriteLine(message);
        }
    }
}
