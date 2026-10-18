using System;
using System.Text;

namespace K9.DataAccessLayer.Helpers
{
    public static class Methods
    {
        public static readonly Random RandomGenerator = new Random();
        
        public static string ToSixDigitCode(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("source");
            }

            // Avoid LINQ method-group analysis that crashes the VS2017 editor.
            var digits = new StringBuilder();
            foreach (char character in value)
            {
                if (char.IsDigit(character))
                {
                    digits.Append(character);
                }
            }

            return digits.ToString().Substring(0, 9);
        }
    }
}
