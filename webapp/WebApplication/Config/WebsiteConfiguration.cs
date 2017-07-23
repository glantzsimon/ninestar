
namespace K9.WebApplication.Config
{
	public class WebsiteConfiguration
	{
		public string CompanyLogoUrl { get; set; }
		public string CompanyName { get; set; }

		public static WebsiteConfiguration Instance { get; set; }
	}

}