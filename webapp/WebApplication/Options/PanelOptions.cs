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

        private string _title2;
        public string Title2
        {
            get { return string.IsNullOrEmpty(_title2) ? Title : _title2; }
            set { _title2 = value; }
        }

        private string _title3;
        public string Title3
        {
            get { return string.IsNullOrEmpty(_title3) ? Title : _title3; }
            set { _title3 = value; }
        }

        public string Body { get; set; }
        public string Body2 { get; set; }
        public string Body3 { get; set; }

        public string Option1Text { get; set; }
        public string Option2Text { get; set; }
        public string Option3Text { get; set; }

        public string ImageSrc { get; set; }
        public EPanelImageSize ImageSize { get; set; }
        public EPanelImageLayout ImageLayout { get; set; }

        public bool IsDualView { get; set; }
        public bool IsTripleView { get; set; }

        public string Option1Id { get; set; }
        public string Option2Id { get; set; }
        public string Option3Id { get; set; }

        public int Option1Value { get; set; }
        public int Option2Value { get; set; }
        public int Option3Value { get; set; }

        public int Value { get; set; }
        
        public string SessionKey { get; set; }

        public bool Option1Checked => Value == Option1Value;
        public bool Option2Checked => Value == Option2Value;
        public bool Option3Checked => Value == Option3Value;

        public string UniqueRadioName { get; set; }
        public string PanelId => $"panel-{Id}";


        public string Option1CheckedString => Option1Checked ? "checked" : "";
        public string Option2CheckedString => Option2Checked ? "checked" : "";
        public string Option3CheckedString => Option3Checked ? "checked" : "";

        public string ImageLayoutCss => ImageLayout == EPanelImageLayout.Contain
            ? "background-size: contain"
            : "background-size: cover";

        public PanelOptions() { }

        public PanelOptions(string title, string body)
        {
            Title = title;
            Body = body;
            Init();
        }

        public static PanelOptions CreatePanelOptionsWithDualView(
            string title, string title2, 
            string body, string body2, 
            string option1Text, string option2Text,
            string sessionKey,
            int value1 = 0, int value2 = 1)
        {
            var panelOptions = new PanelOptions
            {
                Title = title,
                Title2 = title2,
                Body = body,
                Body2 = body2,
                Option1Text = option1Text,
                Option2Text = option2Text,
                Option1Value = value1,
                Option2Value = value2,
                IsDualView = true,
                SessionKey = sessionKey
            };

            panelOptions.Init();
            return panelOptions;
        }

        public static PanelOptions CreatePanelOptionsWithTripleView(
            string title, string title2, string title3, 
            string body, string body2, string body3, 
            string option1Text, string option2Text, string option3Text,
            string sessionKey,
            int value1 = 0, int value2 = 1, int value3 = 2)
        {
            var panelOptions = new PanelOptions
            {
                Title = title,
                Title2 = title2,
                Title3 = title3,
                Body = body,
                Body2 = body2,
                Body3 = body3,
                Option1Text = option1Text,
                Option2Text = option2Text,
                Option3Text = option3Text,
                Option1Value = value1,
                Option2Value = value2,
                Option3Value = value3,
                IsTripleView = true,
                SessionKey = sessionKey
            };

            panelOptions.Init();
            return panelOptions;
        }

        private void Init()
        {
            Option2Id = Guid.NewGuid().ToString();
            Option1Id = Guid.NewGuid().ToString();
            Option3Id = Guid.NewGuid().ToString();

            UniqueRadioName = Guid.NewGuid().ToString();
        }
    }
}