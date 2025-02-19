using K9.SharedLibrary.Helpers;
using System;
using System.IO;
using System.Linq;
using K9.SharedLibrary.Extensions;

namespace K9.SharedLibrary.Models
{
    public class AssetInfo : IAssetInfo
    {
        private readonly string _baseWebPath;

        public AssetInfo(string pathOnDisk, string baseWebPath)
        {
            PathOnDisk = pathOnDisk;
            _baseWebPath = baseWebPath.EndsWith("/") ? baseWebPath.Remove(_baseWebPath.Length - 1) : baseWebPath;
            FileInfo = new FileInfo(PathOnDisk);
            ImageInfo = IsImage() ? ImageProcessor.GetImageInfo(PathOnDisk, false) : null;
        }

        public string PathOnDisk { get; }

        public string FileName => FileInfo.Name;

        public string FileNameWithoutExtension => GetNameWithoutExtension();

        public string ShortFileName => FileInfo.GetShortFileName();

        public string Src => $"{_baseWebPath}/{FileName}";

        public FileInfo FileInfo { get; }

        public ImageInfo ImageInfo { get; }

        public string FontAweSomeFileTypeClass => GetFontAweSomeFileTypeClass();

        public string Extension => FileInfo.Extension;

        public string ExtensionNoDot => Extension.Split('.').LastOrDefault();

        public bool IsImage()
        {
            return HelperMethods.GetImageFileExtensions().Contains(FileInfo.Extension.ToLower());
        }

        public bool IsVideo()
        {
            return HelperMethods.GetVideoFileExtensions().Contains(FileInfo.Extension.ToLower());
        }

        public bool IsAudio()
        {
            return HelperMethods.GetAudioFileExtensions().Contains(FileInfo.Extension.ToLower());
        }

        public bool IsDocument()
        {
            return !IsVideo() && !IsAudio() && !IsImage();
        }

        public bool IsTextFile()
        {
            return FileInfo.Extension.ToLower() == ".txt";
        }

        private string GetNameWithoutExtension()
        {
            return FileName.Substring(0, FileName.LastIndexOf(".", StringComparison.Ordinal));
        }

        private string GetFontAweSomeFileTypeClass()
        {
            switch (Extension.ToLower())
            {
                case ".cs":
                case ".vb":
                case ".js":
                case ".css":
                case ".cshtml":
                case ".xml":
                case ".html":
                case ".htm":
                    return "fa fa-file-code-o";

                case ".xlsx":
                case ".xlsm":
                case ".xls":
                    return "fa fa-file-excel-o";

                case ".pdf":
                    return "fa fa-file-pdf-o";

                case ".ppsx":
                case ".ppt":
                case ".pptm":
                case ".pptx":
                    return "fa fa-file-powerpoint-o";

                case ".txt":
                    return "fa fa-file-text-o";

                case ".doc":
                case ".docx":
                    return "fa fa-file-word-o";

                case ".zip":
                    return "fa fa-file-zip-o";

                default:
                    return "fa fa-file-o";
            }
        }

    }
}
