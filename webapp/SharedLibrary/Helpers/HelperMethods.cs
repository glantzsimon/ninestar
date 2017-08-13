using System.Collections.Generic;

namespace K9.SharedLibrary.Helpers
{
	public static class HelperMethods
	{

		public static List<string> GetImageFileExtensions()
		{
			return new List<string>()
			{
				".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff"
			};
		}

		public static List<string> GetVideoFileExtensions()
		{
			return new List<string>()
			{
				".mpeg", ".mpg", ".mov", ".mp4"
			};
		}

	}
}
