using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace K9.WebApplication.Helpers
{
    public static class MobileCheckoutProtection
    {
        private const string Purpose = "NineStarKi.MobileCheckout.v1";

        public static string Protect(string json, int userId, Guid requestId) => Convert.ToBase64String(
            MachineKey.Protect(Encoding.UTF8.GetBytes(json), Purpose, userId.ToString(CultureInfo.InvariantCulture), requestId.ToString("N")));

        public static string Unprotect(string value, int userId, Guid requestId)
        {
            try
            {
                var bytes = MachineKey.Unprotect(Convert.FromBase64String(value), Purpose,
                    userId.ToString(CultureInfo.InvariantCulture), requestId.ToString("N"));
                return bytes == null ? null : Encoding.UTF8.GetString(bytes);
            }
            catch (FormatException) { return null; }
            catch (CryptographicException) { return null; }
            catch (ArgumentException) { return null; }
        }
    }
}
