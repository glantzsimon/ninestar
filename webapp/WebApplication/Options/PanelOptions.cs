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
        public string Body2 { get; set; }
        public string ImageSrc { get; set; }
        public EPanelImageSize ImageSize { get; set; }
        public EPanelImageLayout ImageLayout { get; set; }
        public bool IsDualView { get; set; }
        public bool IsGlobalSwitch { get; set; }
        
        public string Option1Id { get; }
        public string Option2Id { get; }
        public string UniqueRadioName { get; }

        public string PanelId => $"panel-{Id}";
        public string SummaryChecked => PanelView == EPanelView.SummaryView ? "checked" : "";
        public string FullTextChecked => PanelView == EPanelView.FullTextView ? "checked" : "";
        public string GlobalViewChecked => PanelCycleView == EPanelCycleView.GlobalView ? "checked" : "";
        public string PersonalViewChecked => PanelCycleView == EPanelCycleView.PersonalView ? "checked" : "";

        [UIHint("PanelView")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PanelViewLabel)]
        public EPanelView PanelView { get; set; }

        [UIHint("PanelCycleView")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PanelCycleViewLabel)]
        public EPanelCycleView PanelCycleView { get; set; }

        public string ImageLayoutCss => ImageLayout == EPanelImageLayout.Contain
            ? "background-size: contain"
            : "background-size: cover";

        public PanelOptions()
        {
            Option2Id = Guid.NewGuid().ToString();
            Option1Id = Guid.NewGuid().ToString();
            UniqueRadioName = Guid.NewGuid().ToString();
        }
    }
}