using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;

namespace K9.WebApplication.ViewModels
{
    public class EmailTemplateDynamicFieldsViewModel
    {
        public string[] DynamicFields { get; }
        public ImageInfo[] ImageFields { get; }

        public EmailTemplateDynamicFieldsViewModel()
        {
            DynamicFields = new[]
            {
                nameof(User.FirstName),
                nameof(Promotion.DiscountPercent),
                nameof(Promotion.DiscountPercentText),
                nameof(Promotion.FormattedFullPrice),
                nameof(Promotion.FormattedSpecialPrice),
                nameof(Promotion.MembershipName),
                nameof(Promotion.PromoLink),
            };
        }
    }
}