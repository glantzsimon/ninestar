using System.Collections.Generic;

namespace K9.WebApplication.Models
{
    public class RecaptchaResult
    {
        public const string ResponseFormVariable = "g-Recaptcha-Response";

        public bool Success { get; set; }
        public List<string> ErrorCodes { get; set; }
    }
}