
using System;

namespace K9.SharedLibrary.Exceptions
{
	public class FileSourceFilePathNotSpecifiedException : ApplicationException
	{
		public FileSourceFilePathNotSpecifiedException() : base("Path to files was not specified.") {} 
	}
}
