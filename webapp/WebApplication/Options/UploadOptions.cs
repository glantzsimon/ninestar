using System;
using K9.SharedLibrary.Enums;

namespace K9.WebApplication.Options
{
    public class UploadOptions
    {
        public string UploadUrl { get; set; }
        public int MaxUploads { get; set; }
        public int UploadCount { get; set; }
        public string IdPrefix { get; set; } = Guid.NewGuid().ToString("N");
        public EFilesSourceFilter Filter { get; set; }
        public string Accept => GetFilterString();

        private string GetFilterString()
        {
            switch (Filter)
            {
                case EFilesSourceFilter.Images:
                    return "image/*";

                case EFilesSourceFilter.Videos:
                    return "video/*";

                case EFilesSourceFilter.Audio:
                    return "audio/*";

                default:
                    return "*";
            }
        }
    }
}