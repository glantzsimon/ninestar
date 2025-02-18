namespace K9.WebApplication.Models
{
    public class HexagramInfo
    {
        public string Stage { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Details { get; set; }
        public string ChangingLinesInterpretation { get; set; }

        public HexagramInfo(string stage, string name, string title, string summary, string details = "")
        {
            Stage = stage;
            Name = name;
            Title = title;
            Summary = summary;
            Details = details;
        }
    }
}