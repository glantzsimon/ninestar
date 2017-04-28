
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using K9.SharedLibrary.Models;

namespace K9.SharedLibrary.Helpers
{
	public static class ImageProcessor
	{

		#region Variables

		private const string InvalidCoordinatesSpecified = "Invalid coordinates specified.";
		private const string CutoutAreaIsOutsideOfTheImageArea = "Cutout area is outside of the image area.";

		#endregion


		#region Enumerators

		public enum Format 
		{
			Jpeg = 1,
			Png = 2,
			Gif = 3,
			Bmp = 4
		}

		#endregion


		#region Functions

		public static Format GetImageFormat(string imagePath)
		{
			FileInfo fileInfo = new FileInfo(imagePath);
			switch (fileInfo.Extension.ToLower())
			{
				case ".jpg":
				case ".jpeg":
					return Format.Jpeg;

				case ".png":
					return Format.Png;

				case ".gif":
					return Format.Gif;

				case ".bmp":
					return Format.Bmp;

				default:
					throw new Exception("Format not supported.");
			}
		}

		public static ImageInfo GetImageInfo(string imagePath)
		{
			Bitmap bitmap = new Bitmap(imagePath);
			ImageInfo imageInfo = new ImageInfo()
			{
				Width = bitmap.Width,
				Height = bitmap.Height,
				Format = bitmap.RawFormat,
				Src = imagePath
			};

			bitmap.Dispose();
			return imageInfo;
		}

		public static string SaveImageToWebFormat(string imagePath, string destinationPath)
		{
			Bitmap bitmap = new Bitmap(imagePath);
			string path = SaveImageToWebFormat(bitmap, imagePath, destinationPath);
			bitmap.Dispose();
			return path;
		}

		private static string SaveImageToWebFormat(Bitmap image, string imagePath, string destinationPath)
		{
			FileInfo fileInfo = new FileInfo(imagePath);
			string savedImageFileName;

			switch (image.RawFormat.ToString().ToLower())
			{
				case "bmp":
				case "emf":
				case "exif":
				case "memorybmp":
				case "tiff":
				case "wmf":
					savedImageFileName = Path.Combine(destinationPath, string.Format("{0}.png", Path.GetFileNameWithoutExtension(imagePath)));
					image.Save(savedImageFileName, ImageFormat.Png);
					break;

				default:
					savedImageFileName = Path.Combine(destinationPath, fileInfo.Name);
					image.Save(savedImageFileName);
					break;
			}

			return savedImageFileName;
		}

		public static Image CutOut(Image image, Rectangle rectangle)
		{
			return CutOut(image, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		public static Image CutOut(Image image, int x, int y, int width, int height)
		{
			Bitmap bitmap = new Bitmap(width, height);
			Bitmap original = new Bitmap(image);

			if (x < 0 || y < 0)
			{
				
				throw new Exception(InvalidCoordinatesSpecified);
			}

			if ((width + x) > original.Width || (height + y) > original.Height)
			{
				throw new Exception(CutoutAreaIsOutsideOfTheImageArea);
			}

			for (int xx = x; xx < (x + width); xx++)
			{
				for (int yy = y; yy < (y + height); yy++)
				{
					Color color = original.GetPixel(xx, yy);
					bitmap.SetPixel(xx - x, yy - y, color);
				}
			}

			original.Dispose();
			return bitmap;
		}

		public static Bitmap CopyToArea(Image sourceImage, Bitmap targetBitmap, Rectangle area)
		{
			return CopyToArea(sourceImage, targetBitmap, area, 255);
		}

		public static Bitmap CopyToArea(Image sourceImage, Bitmap targetBitmap, Rectangle area, int opacity)
		{
			Bitmap original = new Bitmap(sourceImage);
			if (area.X < 0 || area.Y < 0)
			{
				throw new Exception(InvalidCoordinatesSpecified);
			}

			if ((area.Width + area.X) > original.Width || (area.Height + area.Y) > original.Height)
			{
				throw new Exception(CutoutAreaIsOutsideOfTheImageArea);
			}

			for (int xx = area.X; xx < (area.X + area.Width); xx++)
			{
				for (int yy = area.Y; yy < (area.Y + area.Height); yy++)
				{
					Color color = Color.FromArgb(opacity, original.GetPixel(xx, yy));
					targetBitmap.SetPixel(xx - area.X, yy - area.Y, color);
				}
			}

			original.Dispose();
			return targetBitmap;
		}

		public static Bitmap CropImage(string imagePath, int width, int height)
		{
			Image image = Image.FromFile(imagePath);
			return CropImage(image, width, height);
		}

		public static Bitmap CropImage(Image image, int width, int height)
		{
			Bitmap bitmap = new Bitmap(image);
			double scale = 0F;

			if (bitmap.Width != width)
			{
				scale = width / (double)bitmap.Width;

				if ((bitmap.Height * scale) < height)
				{
					scale = height / (double)bitmap.Height;
				}
			}

			if (scale == 0)
			{
				scale = 1F;
			}

			Bitmap resizedBitmap = new Bitmap(Convert.ToInt32(bitmap.Width * scale), Convert.ToInt32(bitmap.Height * scale));
			Graphics graphics = Graphics.FromImage(resizedBitmap);

			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.DrawImage(bitmap, 0, 0, resizedBitmap.Width + 1, resizedBitmap.Height + 1);

			Bitmap croppedBitmap = new Bitmap(width, height);

			var startX = Convert.ToInt32(bitmap.Width * scale) - width;
			if (startX < 0)
			{
				startX = 0;
			}
			else
			{
				startX = (int)Math.Floor(startX / 2.0);
			}

			var startY = Convert.ToInt32(bitmap.Height * scale) - height;
			if (startY < 0)
			{
				startY = 0;
			}
			else
			{
				startY = (int)Math.Floor(startY / 2.0);
			}

			for (var x = startX; x <= (width - 1) + startX; x++)
			{
				for (var y = startY; y <= (height - 1) + startY; y++)
				{
					var color = resizedBitmap.GetPixel(x, y);

					croppedBitmap.SetPixel(x - startX, y - startY, color);
				}
			}

			bitmap.Dispose();
			resizedBitmap.Dispose();
			graphics.Dispose();

			return croppedBitmap;
		}

		public static string CropAndSaveImage(string imagePath, int width, int height, string destinationPath)
		{
			Bitmap croppedBitmap = CropImage(imagePath, width, height);
			string path = SaveImageToWebFormat(croppedBitmap, imagePath, destinationPath);
			croppedBitmap.Dispose();
			return path;
		}

		public static string ResizeAndSaveImage(string imagePath, int targetWidth, int targetHeight, string destinationPath)
		{
			Bitmap bitmap = new Bitmap(imagePath);
			Bitmap resizedBitmap = new Bitmap(targetWidth, targetHeight);
			Graphics graphics = Graphics.FromImage(resizedBitmap);

			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.DrawImage(bitmap, 0, 0, resizedBitmap.Width + 1, resizedBitmap.Height + 1);

			bitmap.Dispose();
			graphics.Dispose();

			string path = SaveImageToWebFormat(resizedBitmap, imagePath, destinationPath);
			resizedBitmap.Dispose();
			return path;
		}

		public static Image ResizeImage(Image imageToResize, Size targetSize)
		{
			var sourceWidth = imageToResize.Width;
			var sourceHeight = imageToResize.Height;

			var nPercentW = (targetSize.Width / (float)sourceWidth);
			var nPercentH = (targetSize.Height / (float)sourceHeight);

			var nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;

			var destWidth = (int)(sourceWidth * nPercent);
			var destHeight = (int)(sourceHeight * nPercent);

			Bitmap b = new Bitmap(destWidth, destHeight);
			Graphics g = Graphics.FromImage(b);
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;

			g.DrawImage(imageToResize, 0, 0, destWidth, destHeight);
			g.Dispose();

			return b;
		}

		public static Bitmap ResizeImageEx(Bitmap bitmap, Size targetSize)
		{
			double scale = 0F;

			var height = targetSize.Height;
			var width = targetSize.Width;

			if (bitmap.Width != width)
			{
				scale = width / (double)bitmap.Width;

				if ((bitmap.Height * scale) < height)
				{
					scale = height / (double)bitmap.Height;
				}
			}

			if (scale == 0)
			{
				scale = 1F;
			}

			Bitmap resizedBitmap = new Bitmap(Convert.ToInt32(bitmap.Width * scale), Convert.ToInt32(bitmap.Height * scale));
			Graphics graphics = Graphics.FromImage(resizedBitmap);

			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.DrawImage(bitmap, 0, 0, resizedBitmap.Width + 1, resizedBitmap.Height + 1);

			Bitmap croppedBitmap = new Bitmap(width, height);

			var startX = Convert.ToInt32(bitmap.Width * scale) - width;
			if (startX < 0)
			{
				startX = 0;
			}
			else
			{
				startX = (int)Math.Floor(startX / 2.0);
			}

			var startY = Convert.ToInt32(bitmap.Height * scale) - height;
			if (startY < 0)
			{
				startY = 0;
			}
			else
			{
				startY = (int)Math.Floor(startY / 2.0);
			}

			for (var x = startX; x <= (width - 1) + startX; x++)
			{
				for (var y = startY; y <= (height - 1) + startY; y++)
				{
					var color = resizedBitmap.GetPixel(x, y);
					croppedBitmap.SetPixel(x - startX, y - startY, color);
				}
			}

			bitmap.Dispose();
			graphics.Dispose();

			return croppedBitmap;
		}

		/// <summary>
		/// Creates a mask for use with office command bar buttons
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public static Bitmap CreateOfficeButtonMask(Image image)
		{
			var bmp = new Bitmap(image);
			var topLeftColor = bmp.GetPixel(0, 0);

			for (int i = 0; i < bmp.Width; i++)
			{
				for (var j = 0; j < bmp.Height; j++)
				{
					bmp.SetPixel(i, j, bmp.GetPixel(i, j) == topLeftColor ? Color.White : Color.Black);
				}
			}

			return bmp;
		}

		/// <summary>
		/// Removes alpha transparency and blends any alpha transparency over the background color
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public static Bitmap BlendToBackground(Image image, Color backgroundColor)
		{
			Bitmap bmp = new Bitmap(image);

			for (int x = 0; x < bmp.Width; x++)
			{
				for (var y = 0; y < bmp.Height; y++)
				{
					if (bmp.GetPixel(x, y).A < 255)
					{
						var originalColor = bmp.GetPixel(x, y);
						Color blendedColor;

						if (originalColor.A == 0)
						{
							blendedColor = backgroundColor;

						}
						else
						{
							byte red = Convert.ToByte(Convert.ToInt32(originalColor.R) + ((Convert.ToInt32(backgroundColor.R) - Convert.ToInt32(originalColor.R)) * ((255 - Convert.ToInt32(originalColor.A)) / 255)));
							byte green = Convert.ToByte(Convert.ToInt32(originalColor.R) + ((Convert.ToInt32(backgroundColor.R) - Convert.ToInt32(originalColor.R)) * ((255 - Convert.ToInt32(originalColor.A)) / 255)));
							byte blue = Convert.ToByte(Convert.ToInt32(originalColor.B) + ((Convert.ToInt32(backgroundColor.B) - Convert.ToInt32(originalColor.B)) * ((255 - Convert.ToInt32(originalColor.A)) / 255)));

							blendedColor = Color.FromArgb(red, green, blue);
						}

						bmp.SetPixel(x, y, blendedColor);
					}
				}
			}

			return bmp;
		}

		public static Bitmap CombineImage(Bitmap bitmap, Bitmap background)
		{
			if (bitmap.Width > background.Width)
			{
				throw new Exception("The bitmap is wider than the background image");
			}
			if (bitmap.Height > background.Height)
			{
				throw new Exception("The bitmap is taller than the background image");
			}

			Bitmap newBitmap = background.Clone(new Rectangle(0, 0, background.Width, background.Height), PixelFormat.Format32bppArgb);
			Graphics graphics = Graphics.FromImage(newBitmap);
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			var startX = Convert.ToInt32((background.Width - bitmap.Width) / 2);
			var startY = Convert.ToInt32((background.Height - bitmap.Height) / 2);

			graphics.DrawImage(background, new Rectangle(0, 0, background.Width, background.Height));
			graphics.DrawImage(bitmap, new Rectangle(startX, startY, bitmap.Width, bitmap.Height));

			graphics.Dispose();

			return newBitmap;
		}

		public static Bitmap AddMirror(Bitmap original)
		{
			return AddMirror(original, 20);
		}

		public static Bitmap AddMirror(Bitmap original, int height)
		{
			var mirror = CreateMirror(original, height);
			var newBitmap = new Bitmap(original.Width, original.Height + height, PixelFormat.Format32bppArgb);
			Graphics graphics = Graphics.FromImage(newBitmap);
			graphics.SmoothingMode = SmoothingMode.HighQuality;

			graphics.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height));
			graphics.DrawImage(mirror, new Point(0, original.Height));

			graphics.Dispose();
			return newBitmap;
		}

		public static Bitmap CreateMirror(Bitmap original)
		{
			return CreateMirror(original, 20);
		}

		public static Bitmap CreateMirror(Bitmap original, int height)
		{
			var bitmap = new Bitmap(original.Width, height, PixelFormat.Format32bppArgb);
			var flippedImage = (Bitmap)original.Clone();
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.SmoothingMode = SmoothingMode.HighQuality;

			// Flip image
			flippedImage.RotateFlip(RotateFlipType.RotateNoneFlipY);

			// Apply fade
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < flippedImage.Width; x++)
				{
					var originalColor = flippedImage.GetPixel(x, y);
					var opacity = Convert.ToInt32((originalColor.A * 0.8) * ((height / (float)(y + 1)) / height));
					if (originalColor.A != 0)
					{
						var color = Color.FromArgb(opacity, originalColor);
						using (var pen = new Pen(color))
						{
							graphics.DrawRectangle(pen, new Rectangle(new Point(x, y), new Size(1, 1)));
						}
					}
				}
			}

			graphics.Dispose();
			flippedImage.Dispose();
			return bitmap;
		}

		public static Image CreateRoundCorners(Image startImage, int cornerRadius, Color backgroundColor)
		{
			cornerRadius *= 2;
			Bitmap roundedImage = new Bitmap(startImage.Width, startImage.Height);
			Graphics g = Graphics.FromImage(roundedImage);
			g.Clear(backgroundColor);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			Brush brush = new TextureBrush(startImage);
			GraphicsPath gp = new GraphicsPath();
			gp.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90);
			gp.AddArc(0 + roundedImage.Width - cornerRadius, 0, cornerRadius, cornerRadius, 270, 90);
			gp.AddArc(0 + roundedImage.Width - cornerRadius, 0 + roundedImage.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
			gp.AddArc(0, 0 + roundedImage.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);
			g.FillPath(brush, gp);
			return roundedImage;
		}

		#endregion

	}
}
