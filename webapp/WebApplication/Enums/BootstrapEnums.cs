﻿

namespace K9.WebApplication.Enums
{

	public enum EButtonType
	{
		Submit,
		Create,
		Delete,
		Edit,
		Button
	}

	public enum EInputSize
	{
		Medium,
		Large,
		Small
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