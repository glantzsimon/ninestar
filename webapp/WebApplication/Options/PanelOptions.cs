using System;
using System.ComponentModel.DataAnnotations;
using K9.Globalisation;
using K9.WebApplication.Enums;
using K9.WebApplication.Helpers;

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

        public string FullTextOptionId { get; }
        public string SummaryOptionId { get; }
        public string UniqueRadioName { get; }

        public string PanelId => $"panel-{Id}";
        public string SummaryChecked => PanelView == EPanelView.SummaryView ? "checked" : "";
        public string FullTextChecked => PanelView == EPanelView.FullTextView ? "checked" : "";

        [UIHint("PanelView")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PanelViewLabel)]
        public EPanelView PanelView { get; set; }

        public string ImageLayoutCss => ImageLayout == EPanelImageLayout.Contain
            ? "background-size: contain"
            : "background-size: cover";

        public PanelOptions()
        {
            FullTextOptionId = Guid.NewGuid().ToString();
            SummaryOptionId = Guid.NewGuid().ToString();
            UniqueRadioName = Guid.NewGuid().ToString();
        }
    }
}