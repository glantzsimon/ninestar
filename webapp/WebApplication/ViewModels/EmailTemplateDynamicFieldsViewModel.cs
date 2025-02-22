using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;
using System.Collections.Generic;

namespace K9.WebApplication.ViewModels
{
    public class EmailTemplateDynamicFieldsViewModel
    {
        public EmailTemplateDynamicFieldsViewModel()
        {
            DynamicFields = new List<string>
            {
                nameof(User.FirstName),
                nameof(Promotion.DiscountPercent),
                nameof(Promotion.FormattedFullPrice),
                nameof(Promotion.FormattedSpecialPrice),
                nameof(Promotion.MembershipName),
                nameof(Promotion.PromoLink),
            };
        }

        public List<string> DynamicFields { get; }
    }
}