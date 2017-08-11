
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
		private static Point _defaultImageSize = new Point(80, 80);

		public NewsItemsService(IPostedFileHelper postedFileHelper)
		{
			_postedFileHelper = postedFileHelper;
		}

		public string SaveNewsItemImageToDisk(HttpPostedFileBase imageFile)
		{
			var saveToPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NewsItemsImagesPath, imageFile.FileName).ToPathOnDisk();
			var originalSaveToPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NewsItemsImagesPath, string.Format("original-{0}", imageFile.FileName)).ToPathOnDisk();

			_postedFileHelper.SavePostedFileToPath(imageFile, originalSaveToPath);

			return ImageProcessor.ResizeAndSaveImage(originalSaveToPath, _defaultImageSize.X, _defaultImageSize.Y, saveToPath);
		}
	}
}