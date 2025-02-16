using System;
using System.Linq;

namespace K9.DataAccessLayer.Helpers
{
    public static class Methods
    {
        public static readonly Random RandomGenerator = new Random();

        public static bool IsValidEmail(this string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        public static string ToSixDigitCode(this string value)
        {
            var numericOnly = new string(value.Where(char.IsDigit).ToArray());
            return numericOnly.Substring(0, 9);
        }
    }
}
