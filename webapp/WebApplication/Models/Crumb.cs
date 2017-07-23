
namespace K9.WebApplication.Models
{
	public class Crumb
	{
		public string Label { get; set; }
		public string Action { get; set; }
		public string Controller { get; set; }
		public bool IsActive { get; set; }
	}
}