using K9.SharedLibrary.Models;

namespace K9.WebApplication.Config
{
	public class OpenAuthConfiguration : IOpenAuthConfiguration, IConfigurable
	{
		public string FacebookAppId { get; set; }
		public string FacebookAppSecret { get; set; }
	}
}