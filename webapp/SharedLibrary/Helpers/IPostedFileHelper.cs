using System.Web;

namespace K9.SharedLibrary.Helpers
{
	public interface IPostedFileHelper
	{
		string SavePostedFileToRelativePath(HttpPostedFileBase postedFile, string saveToPath);
		void SavePostedFileToPath(HttpPostedFileBase postedFile, string saveToFilePath);
	}
}