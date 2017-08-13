
using System.Collections.Generic;
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
		public List<UploadedFile> UploadedFiles { get; set; }

		public FileSource()
		{
			UploadedFiles = new List<UploadedFile>();
		}

		public List<string> GetAcceptedFileExtensions()
		{
			switch (Filter)
			{
				case EFilesSourceFilter.Images:
					return HelperMethods.GetImageFileExtensions();

				case EFilesSourceFilter.Videos:
					return HelperMethods.GetVideoFileExtensions();

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
