
using K9.SharedLibrary.Models;

namespace K9.SharedLibrary.Helpers
{
	public interface IFileSourceHelper
	{
		void SaveFilesToDisk(FileSource fileSource, bool createDirectory = false);
	    void DeleteFilesFromDisk(FileSource fileSource);
        void LoadFiles(FileSource fileSource, bool throwErrorIfDirectoryNotFound = true);
	}
}