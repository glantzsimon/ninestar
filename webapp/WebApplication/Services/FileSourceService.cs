
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;

namespace K9.WebApplication.Services
{
	public class FileSourceService : IFileSourceService
	{
		private readonly IPostedFileHelper _postedFileHelper;
		
		public FileSourceService(IPostedFileHelper postedFileHelper)
		{
			_postedFileHelper = postedFileHelper;
		}

		public void SaveFilesToDisk(FileSource fileSource)
		{
			foreach (var httpPostedFileBase in fileSource.PostedFile)
			{
				_postedFileHelper.SavePostedFileToRelativePath(httpPostedFileBase, fileSource.PathToFiles);
			}
		}
	}
}