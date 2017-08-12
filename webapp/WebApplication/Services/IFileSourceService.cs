
using K9.SharedLibrary.Models;

namespace K9.WebApplication.Services
{
	public interface IFileSourceService
	{
		void SaveFilesToDisk(FileSource fileSource);
	}
}