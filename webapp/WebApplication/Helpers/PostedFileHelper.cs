
using System;
using System.IO;
using System.Web;
using K9.SharedLibrary.Extensions;

namespace K9.WebApplication.Helpers
{
	public class PostedFileHelper : IPostedFileHelper
	{
		/// <summary>
		/// Save the file to the path relative to the web application.
		/// </summary>
		/// <param name="postedFile"></param>
		/// <param name="saveToPath"></param>
		/// <returns></returns>
		public string SavePostedFileToRelativePath(HttpPostedFileBase postedFile, string saveToPath)
		{
			if (postedFile == null)
			{
				throw new NullReferenceException("PostedFile cannot be null");
			}
			var saveToFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, saveToPath.ToPathOnDisk(), postedFile.FileName);
			postedFile.SaveAs(saveToFilePath);
			return saveToFilePath;
		}

		public void SavePostedFileToPath(HttpPostedFileBase postedFile, string saveToFilePath)
		{
			if (postedFile == null)
			{
				throw new NullReferenceException("PostedFile cannot be null");
			}
			postedFile.SaveAs(saveToFilePath);
		}
	}
}