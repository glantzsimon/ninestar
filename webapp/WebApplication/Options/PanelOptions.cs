using System.ComponentModel.DataAnnotations;
using K9.Globalisation;
using K9.WebApplication.Enums;

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
        public string Summary { get; set; }
        public string ImageSrc { get; set; }
        public EPanelImageSize ImageSize { get; set; }
        public EPanelImageLayout ImageLayout { get; set; }
        public bool IsDualView { get; set; }

        public string PanelId => $"panel-{Id}";

        [UIHint("PanelView")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PanelViewLabel)]
        public EPanelView PanelView { get; set; }

        public string ImageLayoutCss => ImageLayout == EPanelImageLayout.Contain
            ? "background-size: contain"
            : "background-size: cover";
    }
}