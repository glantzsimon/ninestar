using System;
using System.IO;
using System.Linq;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using NLog;

namespace K9.SharedLibrary.Helpers
{
	public class FileSourceHelper : IFileSourceHelper
	{
		private readonly IPostedFileHelper _postedFileHelper;
		private readonly ILogger _logger;

		public FileSourceHelper(IPostedFileHelper postedFileHelper, ILogger logger)
		{
			_postedFileHelper = postedFileHelper;
			_logger = logger;
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

		public void SaveFilesToDisk(FileSource fileSource, bool createDirectory = false)
		{
			if (!Directory.Exists(fileSource.PathToFiles))
			{
				if (createDirectory)
				{
					CreateDirectory(fileSource);
				}
			}
			SavePostedFiles(fileSource);
			UpdateUploadedFiles(fileSource);
		}

		private void SavePostedFiles(FileSource fileSource)
		{
			foreach (var httpPostedFileBase in fileSource.PostedFile)
			{
				_postedFileHelper.SavePostedFileToRelativePath(httpPostedFileBase, fileSource.PathToFiles);
			}
		}

		private void UpdateUploadedFiles(FileSource fileSource)
		{
			var filesToDelete = ContentHelper.GetFiles(fileSource.PathToFiles)
				.Where(f => !fileSource.UploadedFiles.Select(a => a.FileName).Contains(f.FileName)).ToList();
			filesToDelete.ForEach(f =>
			{
				try
				{
					File.Delete(f.PathOnDisk);
				}
				catch (Exception ex)
				{
					_logger.Error("UpdateUploadedFiles => could not delete file {0}. {1}", f.FileName, ex.Message);
				}
			});
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