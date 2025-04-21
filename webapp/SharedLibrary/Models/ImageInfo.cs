
using System.Drawing.Imaging;

namespace K9.SharedLibrary.Models
{
    public class ImageInfo
    {

        public int Width { get; set; }

        public int Height { get; set; }

        public ImageFormat Format { get; set; }

        public string Src { get; set; }
        
        public string LocalSrc { get; set; }

        public string FileName { get; set; }

        public string AltText { get; set; }

        public string GetHtmlPlaceHolder()
        {
            return $"{{img src=\"{Src}\" alt=\"{AltText}\" /}}";
        }

        public bool IsPortrait()
        {
            return Height > Width;
        }
    }
}
