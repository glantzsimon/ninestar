using System;
using System.IO;

namespace K9.SharedLibrary.Extensions
{
	public static class IoExtensions
	{

		public static string GetFileNameWithoutExtension(this FileInfo fileInfo)
		{
			var fileName = fileInfo.Name;
			return fileName.Substring(0, fileName.LastIndexOf(".", StringComparison.Ordinal));
		}

		public static string ToPathOnDisk(this string value)
		{
			return value.Replace("/", "\\");
		}
	}
}
