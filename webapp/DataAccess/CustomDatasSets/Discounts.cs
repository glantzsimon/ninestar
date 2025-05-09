using K9.DataAccessLaye.Attributes;
using K9.DataAccessLayer.Enums;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace K9.DataAccessLayer.Database
{
    public class Discounts : ICustomDataSet<EDiscount>
    {
        public string Key => nameof(EDiscount);

        public List<ListItem> GetData(DbContext db)
        {
            var values = new List<EDiscount>
            {
                EDiscount.None,
                EDiscount.FirstDiscount,
                EDiscount.SecondDiscount,
                EDiscount.ThirdDiscount,
                EDiscount.FourthDiscount
            };
            return new List<ListItem>(values.Select(e =>
            {
                var discountAttribute = e.GetAttribute<DiscountAttribute>();
                return new ListItem((int)e, discountAttribute.Description, discountAttribute.DiscountPercent.ToString());
            }));
        }
    }
}
