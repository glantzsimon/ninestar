using System.Collections.Generic;

namespace K9.WebApplication.ViewModels.MobileApi
{
    public class MobileArtworkResponse
    {
        public string Version { get; set; }
        public Dictionary<string, MobileArtwork> Assets { get; set; }
    }

    public class MobileArtwork
    {
        public string Group { get; set; }
        public Dictionary<string, MobileArtworkVariant> Variants { get; set; }
    }

    public class MobileArtworkVariant
    {
        public string Url { get; set; }
        public string Revision { get; set; }
        public string ContentType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
