
using System;
using System.Drawing;
using System.IO;
using System.Web;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Extensions;
using K9.WebApplication.Helpers;

namespace K9.WebApplication.Services
{
	public class NewsItemsService : INewsItemsService
	{
		private readonly IPostedFileHelper _postedFileHelper;
		private const string NewsItemsImagesPath = "Images/news/upload";
		private static Point _defaultImageSize = new Point(440, 300);

		public NewsItemsService(IPostedFileHelper postedFileHelper)
		{
			_postedFileHelper = postedFileHelper;
		}

		public string SaveNewsItemImageToDisk(HttpPostedFileBase imageFile)
		{
			var fileName = imageFile.FileName;
			var saveToPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NewsItemsImagesPath, fileName).ToPathOnDisk();
			var originalSaveToPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NewsItemsImagesPath, string.Format("original-{0}", fileName)).ToPathOnDisk();

			_postedFileHelper.SavePostedFileToPath(imageFile, originalSaveToPath);

			ImageProcessor.ResizeAndSaveImage(originalSaveToPath, _defaultImageSize.X, _defaultImageSize.Y, saveToPath);
			return string.Format("{0}/{1}", NewsItemsImagesPath, fileName);
		}
	}
}