using System;
using System.ComponentModel.DataAnnotations.Schema;
using K9.SharedLibrary.Enums;

namespace K9.SharedLibrary.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class FileSourceInfo : NotMappedAttribute
	{
		private readonly string _pathToFiles;
		
		public string PathToFiles
		{
			get { return _pathToFiles; }
		}

		public EFilesSourceFilter Filter { get; set; }

		public FileSourceInfo(string pathToFiles)
		{
			_pathToFiles = pathToFiles;
		}
	}
}
