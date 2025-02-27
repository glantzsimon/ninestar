using K9.Globalisation;

namespace K9.WebApplication.ViewModels
{
    public class PricingViewModel
    {
        public string Name { get; set; }
        public string FormattedPrice { get; set; }
        public string ActionText { get; set; } = Dictionary.BuyNow;
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public object RouteValues { get; set; }
    }
}