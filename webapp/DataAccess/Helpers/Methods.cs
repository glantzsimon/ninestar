using System;
using System.Linq;

namespace K9.DataAccessLayer.Helpers
{
    public static class Methods
    {
        public static readonly Random RandomGenerator = new Random();
        
        public static string ToSixDigitCode(this string value)
        {
            var numericOnly = new string(value.Where(char.IsDigit).ToArray());
            return numericOnly.Substring(0, 9);
        }
    }
}
