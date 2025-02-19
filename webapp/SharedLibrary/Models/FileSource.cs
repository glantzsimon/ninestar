
using System.Collections.Generic;
using System.Linq;
using System.Web;
using K9.SharedLibrary.Enums;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;

namespace K9.SharedLibrary.Models
{
    public class FileSource
    {
        public EFilesSourceFilter Filter { get; set; }
        public string PathToFiles { get; set; }
        public List<HttpPostedFileBase> PostedFile { get; set; }
        public List<UploadedFile> UploadedFiles { get; set; }
        public List<UploadedFile> AudioFiles => UploadedFiles?.Where(_ => _.AssetInfo.IsAudio()).ToList();
        public List<UploadedFile> VideoFiles => UploadedFiles?.Where(_ => _.AssetInfo.IsVideo()).ToList();
        public List<UploadedFile> ImageFiles => UploadedFiles?.Where(_ => _.AssetInfo.IsImage()).ToList();
        public List<UploadedFile> Documents => UploadedFiles?.Where(_ => _.AssetInfo.IsDocument()).ToList();
        public bool IsReadOnly { get; set; }

        public FileSource()
        {
            UploadedFiles = new List<UploadedFile>();
        }

        public List<string> GetAcceptedFileExtensions()
        {
            switch (Filter)
            {
                case EFilesSourceFilter.Images:
                    return HelperMethods.GetImageFileExtensions();

                case EFilesSourceFilter.Videos:
                    return HelperMethods.GetVideoFileExtensions();

                case EFilesSourceFilter.Audio:
                    return HelperMethods.GetAudioFileExtensions();

                default:
                    return new List<string>(new[] { "*" });
            }
        }

        public string GetAcceptedFileExtensionsList()
        {
            return GetAcceptedFileExtensions().ToDelimitedString();
        }

        public bool IsFileExtensionValid(string extension)
        {
            var acceptedFileExtensions = GetAcceptedFileExtensions();   
            return acceptedFileExtensions.Contains(extension) || acceptedFileExtensions.Contains("*");
        }
        
    }
}
