using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;

namespace K9.WebApplication.Services
{
    public class RecaptchaService : IRecaptchaService
    {
        public INineStarKiPackage Package { get; }

        private readonly RecaptchaConfiguration _config;

        public RecaptchaService(INineStarKiPackage packge, IOptions<RecaptchaConfiguration> config)
        {
            Package = packge;
            _config = config.Value;
        }
        
        public bool Validate(string encodedResponse)
        {
            if (string.IsNullOrEmpty(encodedResponse)) return false;

            var client = new System.Net.WebClient();
            var secret = _config.RecaptchaSecreteKey;

            if (string.IsNullOrEmpty(secret)) return false;

            var googleReply = client.DownloadString(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={encodedResponse}");

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            var reCaptcha = serializer.Deserialize<RecaptchaResult>(googleReply);

            return reCaptcha.Success;
        }
    }
}