
using System;
using System.Collections.Generic;
using System.IO;
using K9.SharedLibrary.Helpers;

namespace K9.SharedLibrary.Models
{
	public class AssetInfo : IAssetInfo
	{
		private static readonly List<string> ImageFileExtensions = new List<string> { ".png", ".jpg", ".jpeg", ".gif" };
		private readonly string _pathOnDisk;
		private readonly string _baseWebPath;
		private readonly FileInfo _fileInfo;
		private readonly ImageInfo _imageInfo;

		public AssetInfo(string pathOnDisk, string baseWebPath)
		{
			_pathOnDisk = pathOnDisk;
			_baseWebPath = baseWebPath.EndsWith("/") ? baseWebPath.Remove(_baseWebPath.Length - 1) : baseWebPath;
			_fileInfo = new FileInfo(_pathOnDisk);
			_imageInfo = ImageProcessor.GetImageInfo(_pathOnDisk);
		}

		public string PathOnDisk
		{
			get
			{
				return _pathOnDisk;
			}
		}

		public string FileName
		{
			get
			{
				return _fileInfo.Name;
			}
		}

		public string Src
		{
			get
			{
				return String.Format("{0}/{1}", _baseWebPath, FileName);
			}
		}

		public FileInfo FileInfo
		{
			get { return _fileInfo; }
		}

		public ImageInfo ImageInfo
		{
			get { return _imageInfo; }
		}

		public string Extension
		{
			get { return _fileInfo.Extension; }
		}

		public bool IsImage()
		{
			return ImageFileExtensions.Contains(_fileInfo.Extension.ToLower());
		}

		public bool IsTextFile()
		{
			return _fileInfo.Extension.ToLower() == ".txt";
		}

		public string GetNameWithoutExtensions()
		{
			return FileName.Substring(0, FileName.LastIndexOf(".", StringComparison.Ordinal));
		}

	}
}
