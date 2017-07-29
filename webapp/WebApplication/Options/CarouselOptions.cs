using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;

namespace K9.WebApplication.Options
{

	public class CarouselOptions
	{

		private readonly string _pathToImages;
		private readonly string _pathToFullSizeImages;
		private List<IAssetInfo> _images;
		private List<IAssetInfo> _fullSizeImages;
		private static Guid _id = Guid.NewGuid();

		public CarouselOptions(string pathToImages, string fullSizeImageFolderName  = "full-size")
		{
			_pathToImages = pathToImages;
			_pathToFullSizeImages = Path.Combine(_pathToImages, fullSizeImageFolderName);
			LoadImages();
		}

		public string Id
		{
			get { return _id.ToString(); }
		}

		public string PathToImages
		{
			get { return _pathToImages; }
		}

		public List<IAssetInfo> Images
		{
			get { return _images; }
		}

		public List<IAssetInfo> FullSizeImages
		{
			get { return _fullSizeImages; }
		}

		public IAssetInfo GetFullSizeImage(string imageName)
		{
			return _fullSizeImages.FirstOrDefault(i => i.FileName == imageName);
		}

		private void LoadImages()
		{
			_images = ContentHelper.GetImageFiles(_pathToImages);
			_fullSizeImages = ContentHelper.GetImageFiles(_pathToFullSizeImages);
		}

	}
}