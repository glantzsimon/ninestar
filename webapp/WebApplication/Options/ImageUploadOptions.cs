using System;

namespace K9.WebApplication.Options
{
    public class ImageUploadOptions
    {
        public string UploadUrl { get; set; }
        public string IdPrefix { get; set; } = Guid.NewGuid().ToString("N");
    }
}