
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using K9.SharedLibrary.Models;

namespace K9.WebApplication.Helpers
{

	public static class ContentHelper
	{

		/// <summary>
		/// Gets all file at the specified path
		/// </summary>
		/// <param name="relativePath">e.g. Images/home</param>
		/// <returns></returns>
		public static List<IAssetInfo> GetFiles(string relativePath)
		{
			return Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath.ToPathOnDisk()))
				.Select(f => new AssetInfo(f, relativePath)).ToList<IAssetInfo>();
		}

		/// <summary>
		/// Gets all image files at the specified path
		/// </summary>
		/// <param name="relativePath">e.g. Images/home</param>
		/// <returns></returns>
		public static List<IAssetInfo> GetImageFiles(string relativePath)
		{
			return GetFiles(relativePath).Where(name => name.IsImage()).ToList();
		}

		/// <summary>
		/// Gets all text files at the specified path
		/// </summary>
		/// <param name="relativePath">e.g. Images/home</param>
		/// <returns></returns>
		public static List<IAssetInfo> GetTextFiles(string relativePath)
		{
			return GetFiles(relativePath).Where(f => f.IsTextFile()).ToList();
		}

		public static List<IAssetInfo> GetFilesWithExtension(string relativePath, params string[] extensions)
		{
			return GetFiles(relativePath).Where(f => f.IsImage()).ToList();
		}

		private static string ToPathOnDisk(this string value)
		{
			return value.Replace("/", "\\");
		}

	}


}