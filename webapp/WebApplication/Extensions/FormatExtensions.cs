
using K9.Globalisation;

namespace K9.WebApplication.Extensions
{
	public static class FormatExtensions
	{

		public static string ToYesNo(this bool value)
		{
			return value ? Dictionary.Yes : Dictionary.No;
		}

	}
}