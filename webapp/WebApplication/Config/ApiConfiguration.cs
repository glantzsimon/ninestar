namespace K9.WebApplication.Config
{
    public class ApiConfiguration
    {
        public static ApiConfiguration Instance { get; set; }

        public string ApiKey { get; set; }
        public string OpenAIApiKey { get; set; }
    }
}