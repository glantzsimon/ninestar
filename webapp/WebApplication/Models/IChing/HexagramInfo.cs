namespace K9.WebApplication.Models
{
    public class HexagramInfo
    {
        public string Stage { get; set; }   // Thematic category
        public string Title { get; set; }   // Traditional I Ching name
        public string Summary { get; set; } 
        public string Description { get; set; } 

        public HexagramInfo(string stage, string title, string summary, string description)
        {
            Stage = stage;
            Title = title;
            Summary = summary;
            Description = description;
        }

        public override string ToString()
        {
            return $"{Title} ({Stage})\n{Summary}\n{Description}";
        }
    }
}