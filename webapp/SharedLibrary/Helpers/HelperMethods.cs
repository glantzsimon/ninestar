using System;
using System.Collections.Generic;
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
                ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff", ".webp", ".svg"
            };
        }

        public static List<string> GetVideoFileExtensions()
        {
            return new List<string>()
            {
                ".mpeg", ".mpg", ".mov", ".mp4", ".flv", ".3gp", ".ogv", ".webm", ".avi", ".wmv", ".swf", ".mkv"
            };
        }

        public static List<string> GetAudioFileExtensions()
        {
            return new List<string>()
            {
                ".mp3", ".wav", ".aac", ".flac", ".mid", ".ac3", ".ogg", ".mka", ".m4a", ".voc", ".au", ".amr", ".ra", ".wma", ".aiff"
            };
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
