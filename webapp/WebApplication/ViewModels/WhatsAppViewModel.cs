using K9.Globalisation;

namespace K9.WebApplication.ViewModels
{
    public class WhatsAppViewModel
    {
        public string Title { get; set; } = Dictionary.DoYouHaveAQuestion;
        public string Message { get; set; } = Dictionary.WhatsAppNineStarQueryText;
        public string ButtonText { get; set; } = Dictionary.MessageUs;
    }
}