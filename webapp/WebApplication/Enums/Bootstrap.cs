
namespace K9.WebApplication.Enums
{

	public enum EButtonType
	{
		Submit,
		Button
	}

	public enum EInputSize
	{
		Default,
		Large,
		Small
	}

	public static class ExtensionMethods
	{
		public static string ToCssClass(this EInputSize size)
		{
			switch (size)
			{
				case EInputSize.Large:
					return "input-lg";

				case EInputSize.Small:
					return "input-sm";

				default:
					return string.Empty;
			}
		}
	}
}
