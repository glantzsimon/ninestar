
using System.Collections.Generic;
using System.Web;
using K9.SharedLibrary.Enums;
using K9.SharedLibrary.Extensions;

namespace K9.SharedLibrary.Models
{
	public class FileSource
	{
		public EFilesSourceFilter Filter { get; set; }
		public string PathToFiles { get; set; }
		public List<HttpPostedFileBase> PostedFile { get; set; }
		public List<UploadedFile> UploadedFiles { get; set; }

		public int UploadedFileCount
		{
			get { return UploadedFiles == null ? 0 : UploadedFiles.Count; }
		}

		public FileSource()
		{
			UploadedFiles = new List<UploadedFile>();
		}

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
		
	}
}
