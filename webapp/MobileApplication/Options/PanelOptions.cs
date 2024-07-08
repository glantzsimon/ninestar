namespace K9.WebApplication.Options
{
    public enum EPanelImageSize
    {
        Default,
        Small,
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

        public string ImageLayoutCss => ImageLayout == EPanelImageLayout.Contain
            ? "background-size: contain"
            : "background-size: cover";
    }
}