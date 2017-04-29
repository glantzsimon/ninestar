
using K9.WebApplication.Config;

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
		Small,
		Medium,
		Large
	}

	public enum EInputWidth
	{
		Default,
		Narrow,
		Medium,
		Wide
	}

	public static class ExtensionMethods
	{
		public static string ToCssClass(this EInputSize size)
		{
			switch (size)
			{
				case EInputSize.Small:
					return "input-sm";

				case EInputSize.Large:
					return "input-lg";

				case EInputSize.Default:
					return AppConfig.DefaultInputSize;

				default:
					return string.Empty;
			}
		}

		public static string ToCssClass(this EInputWidth size)
		{
			switch (size)
			{
				case EInputWidth.Narrow:
					return "col-xs-2";

				case EInputWidth.Medium:
					return "col-xs-3";

				case EInputWidth.Wide:
					return "col-xs-4";

				default:
					return string.Empty;
			}
		}

	}
}
