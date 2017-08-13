using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;

namespace K9.SharedLibrary.Helpers
{

	public static class ContentHelper
	{

		/// <summary>
		/// Gets all file at the specified path
		/// </summary>
		/// <param name="relativePath">e.g. Images/home</param>
		/// <returns></returns>
		public static List<AssetInfo> GetFiles(string relativePath)
		{
			var pathToFiles = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath.ToPathOnDisk());
			return Directory.GetFiles(pathToFiles)
				.Select(f => new AssetInfo(f, relativePath)).ToList();
		}

		/// <summary>
		/// Gets all image files at the specified path
		/// </summary>
		/// <param name="relativePath">e.g. Images/home</param>
		/// <returns></returns>
		public static List<AssetInfo> GetImageFiles(string relativePath)
		{
			return GetFiles(relativePath).Where(name => name.IsImage()).ToList();
		}

		/// <summary>
		/// Gets all text files at the specified path
		/// </summary>
		/// <param name="relativePath">e.g. Images/home</param>
		/// <returns></returns>
		public static List<AssetInfo> GetTextFiles(string relativePath)
		{
			return GetFiles(relativePath).Where(f => f.IsTextFile()).ToList();
		}

		public static List<AssetInfo> GetFilesWithExtension(string relativePath, params string[] extensions)
		{
			return GetFiles(relativePath).Where(f => f.IsImage()).ToList();
		}

	}


}