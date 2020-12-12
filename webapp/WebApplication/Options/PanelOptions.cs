namespace K9.WebApplication.Options
{
    public enum EPanelImageSize
    {
        Default,
        Medium
    }

    public enum EPanelImageLayout
    {
        Cover,
        Contain
    }
    
    public class PanelOptions
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ImageSrc { get; set; }
        public EPanelImageSize ImageSize { get; set; }
        public EPanelImageLayout ImageLayout { get; set; }

        public string ImageLayoutCss => ImageLayout == EPanelImageLayout.Cover
            ? "background-size: cover"
            : "background-size: contain";
    }
}