
using System.ComponentModel.DataAnnotations;

namespace K9.SharedLibrary.Models
{
    public class LinkPreviewResult : ILinkPreviewResult
    {
        [DataType(DataType.Url)]
        public string Url { get; }
        public string Title { get; }
        [DataType(DataType.MultilineText)]
        public string Description { get; }
        [DataType(DataType.ImageUrl)]
        public string ImageUrl { get; }

        public LinkPreviewResult(string url, string title, string description, string imageUrl)
        {
            Url = url;
            Title = title;
            Description = description;
            ImageUrl = imageUrl;
        }
    }
}
