using K9.DataAccessLaye.Attributes;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using System.Collections.Generic;
using System.Linq;

namespace K9.DataAccessLayer.Enums
{
    public enum EDiscount
    {
        [Discount(DiscountPercent = 0, ResourceType = typeof(Dictionary), Name = Strings.Names.None)]
        None,
        [Discount(DiscountPercent = 15, ResourceType = typeof(Dictionary), Name = Strings.Names.FirstDiscount)]
        FirstDiscount,
        [Discount(DiscountPercent = 30, ResourceType = typeof(Dictionary), Name = Strings.Names.SecondDiscount)]
        SecondDiscount,
        [Discount(DiscountPercent = 70, ResourceType = typeof(Dictionary), Name = Strings.Names.ThirdDiscount)]
        ThirdDiscount
    }

    public static class EDiscountExtensions
    {
        public static List<ListItem> GetDiscountListItems()
        {
            var values = new List<EDiscount>
            {
                EDiscount.None,
                EDiscount.FirstDiscount,
                EDiscount.SecondDiscount,
                EDiscount.ThirdDiscount
            };
            return new List<ListItem>(values.Select(e =>
            {
                var discountAttribute = e.GetAttribute<DiscountAttribute>();
                return new ListItem((int)e, discountAttribute.Description, discountAttribute.DiscountPercent.ToString());
            }));
        }
    }

}
