
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using K9.SharedLibrary.Enums;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;

namespace K9.SharedLibrary.Models
{
	public class FileSource
	{
		public EFilesSourceFilter Filter { get; set; }
		public string PathToFiles { get; set; }
		public List<HttpPostedFileBase> PostedFile { get; set; }
		public List<IAssetInfo> UploadedFiles { get; set; }

		public List<string> GetAcceptedFileExtensions()
		{
			switch (Filter)
			{
				case EFilesSourceFilter.Images:
					return new List<string>()
					{
						".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff"
					};

				case EFilesSourceFilter.Videos:
					return new List<string>()
					{
						".mov", ".mp4"
					};

				default:
					return new List<string>();
			}
		}

		public string GetAcceptedFileExtensionsList()
		{
			return GetAcceptedFileExtensions().ToDelimitedString();
		}

		public void LoadFiles()
		{
			var pathOnDisk = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PathToFiles.ToPathOnDisk());
			if (Directory.Exists(pathOnDisk))
			{
				try
				{
					Directory.CreateDirectory(pathOnDisk);
					UploadedFiles = ContentHelper.GetFiles(PathToFiles);
				}
				catch (Exception ex)
				{
					throw new Exception("An error occurred whilst trying to load the files.", ex);
				}
			}
		}
	}
}
