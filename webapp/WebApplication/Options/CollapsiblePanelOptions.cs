namespace K9.WebApplication.Options
{
    public class CollapsiblePanelOptions
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Footer { get; set; }
        public bool Expaded { get; set; }
        public string ExpadedCssClass => Expaded ? "in" : "";
        public string CaretCssClass => Expaded ? "fa-caret-up" : "fa-caret-down";
    }
}