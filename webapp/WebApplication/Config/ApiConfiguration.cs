namespace K9.WebApplication.Config
{
    public class ApiConfiguration
    {
        public static ApiConfiguration Instance { get; set; }

        public string ApiKey { get; set; }
        public string OpenAIApiKey { get; set; }
        public string NineStarKiAstrologerGptUrl { get; set; }
        public string GroqApiKey { get; set; }
        public string GroqEndpoint { get; set; }
        public string GroqModel { get; set; }
        public string OpenRouterApiKey { get; set; }
        public string OpenRouterModel { get; set; }
        public string OpenRouterEndpoint { get; set; }
    }
}