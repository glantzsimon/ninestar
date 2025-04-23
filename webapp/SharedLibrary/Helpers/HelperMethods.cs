using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace K9.SharedLibrary.Helpers
{
    public static class HelperMethods
    {
        private const string YoutubeEmbedUrl = "https://www.youtube.com/embed";

        public static List<string> GetImageFileExtensions()
        {
            return new List<string>()
            {
                ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff", ".webp", ".svg",
                ".ico",     // Windows icon files
                ".heic",    // High Efficiency Image Format (used by iPhones)
                ".jfif",    // JPEG File Interchange Format
                ".avif"     // Modern image format (AV1 Image File Format)
            };

        }

        public static List<string> GetVideoFileExtensions()
        {
            return new List<string>()
            {
                ".mpeg", ".mpg", ".mov", ".mp4", ".flv", ".3gp", ".ogv", ".webm", ".avi", ".wmv", ".swf", ".mkv",
                ".m4v",     // Apple’s video container
                ".mts",     // AVCHD file (used in HD video cameras)
                ".ts",      // Transport Stream (often from broadcast systems)
                ".vob",     // DVD Video Object
                ".f4v",     // Flash video format
                ".m2ts"     // Blu-ray video format
            };

        }

        public static List<string> GetAudioFileExtensions()
        {
            return new List<string>()
            {
                ".mp3", ".wav", ".aac", ".flac", ".mid", ".ac3", ".ogg", ".mka", ".m4a", ".voc", ".au", ".amr", ".ra", ".wma", ".aiff",
                ".opus",    // High-efficiency codec (used in WhatsApp, Discord)
                ".aif",     // Alternative extension for AIFF
                ".mp2",     // MPEG-1 Audio Layer II
                ".snd",     // NeXT/Sun audio file
                ".oga"      // Ogg Audio container
            };
        }

        public static List<string> GetDocumentFileExtensions()
        {
            return new List<string>()
            {
                ".pdf",         // Adobe PDF
                ".doc", ".docx", // Microsoft Word
                ".xls", ".xlsx", // Microsoft Excel
                ".ppt", ".pptx", // Microsoft PowerPoint
                ".txt",         // Plain text
                ".rtf",         // Rich Text Format
                ".odt",         // OpenDocument Text
                ".ods",         // OpenDocument Spreadsheet
                ".odp",         // OpenDocument Presentation
                ".csv",         // Comma-separated values
                ".tsv",         // Tab-separated values
                ".md"           // Markdown
            };
        }

        private static readonly List<string> _allSupportedFileExtensions = GetImageFileExtensions()
            .Concat(GetVideoFileExtensions())
            .Concat(GetAudioFileExtensions())
            .Concat(GetDocumentFileExtensions())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        public static List<string> GetAllSupportedFileExtensions() => _allSupportedFileExtensions;
        
        public static bool IsSupportedFileType(this string fileName)
        {
            return GetAllSupportedFileExtensions().Contains(new FileInfo(fileName).Extension.ToLower());
        }

        public static bool IsImage(this string fileName)
        {
            return GetImageFileExtensions().Contains(new FileInfo(fileName).Extension.ToLower());
        }

        public static bool IsVideo(this string fileName)
        {
            return GetVideoFileExtensions().Contains(new FileInfo(fileName).Extension.ToLower());
        }

        public static bool IsAudio(this string fileName)
        {
            return GetAudioFileExtensions().Contains(new FileInfo(fileName).Extension.ToLower());
        }

        public static bool IsDocument(this string fileName)
        {
            return GetDocumentFileExtensions().Contains(new FileInfo(fileName).Extension.ToLower());
        }

        public static string GetEmbeddableUrl(string url)
        {
            if (url.ToLower().Contains("youtube.com/watch?v="))
            {
                return url.Replace("watch?v=", "embed/");
            }
            return url;
        }

        public static IEnumerable<DateTime> Until(this DateTime from, DateTime until)
        {
            for (var day = from.Date; day.Date <= until.Date; day = day.AddDays(1))
                yield return day;
        }

        public static string Slugify(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            var slug = value.ToLowerInvariant().Trim();
            slug = Regex.Replace(slug, @"[^\w\s-]", "");  // Remove non-word chars
            slug = Regex.Replace(slug, @"\s+", "-");      // Replace spaces with dashes
            slug = Regex.Replace(slug, "-+", "-");        // Collapse multiple dashes
            return slug;
        }
    }
}
