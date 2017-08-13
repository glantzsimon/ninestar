using System;
using System.IO;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;

namespace K9.SharedLibrary.Helpers
{
	public class FileSourceHelper : IFileSourceHelper
	{
		private readonly IPostedFileHelper _postedFileHelper;

		public FileSourceHelper(IPostedFileHelper postedFileHelper)
		{
			_postedFileHelper = postedFileHelper;
		}

		public void SaveFilesToDisk(FileSource fileSource, bool createDirectory = false)
		{
			if (!Directory.Exists(fileSource.PathToFiles))
			{
				if (createDirectory)
				{
					CreateDirectory(fileSource);
				}
			}
			foreach (var httpPostedFileBase in fileSource.PostedFile)
			{
				_postedFileHelper.SavePostedFileToRelativePath(httpPostedFileBase, fileSource.PathToFiles);
			}
		}

		public void LoadFiles(FileSource fileSource, bool throwErrorIfDirectoryNotFound = true)
		{
			var pathOnDisk = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileSource.PathToFiles.ToPathOnDisk());
			if (Directory.Exists(pathOnDisk))
			{
				try
				{
					fileSource.UploadedFiles = ContentHelper.GetFiles(fileSource.PathToFiles);
				}
				catch (Exception ex)
				{
					throw new Exception("An error occurred whilst trying to load the files.", ex);
				}	
			}
			else if (throwErrorIfDirectoryNotFound)
			{
				throw new DirectoryNotFoundException(string.Format("The directory {0} does not exist.", pathOnDisk));
			}
		}

		private void CreateDirectory(FileSource fileSource)
		{
			var pathOnDisk = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileSource.PathToFiles.ToPathOnDisk());
			if (!Directory.Exists(pathOnDisk))
			{
				Directory.CreateDirectory(pathOnDisk);
			}
		}

	}
}