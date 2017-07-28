using System;
using System.Collections.Generic;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;

namespace K9.WebApplication.Options
{

	public class CarouselOptions
	{

		private readonly string _pathToImages;
		private List<IAssetInfo> _images;
		private static Guid _id = Guid.NewGuid();

		public CarouselOptions(string pathToImages)
		{
			_pathToImages = pathToImages;
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

		private void LoadImages()
		{
			_images = ContentHelper.GetImageFiles(PathToImages);
		}

	}
}