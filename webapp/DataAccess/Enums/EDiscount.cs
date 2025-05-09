using K9.DataAccessLaye.Attributes;
using K9.Globalisation;

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
        [Discount(DiscountPercent = 50, ResourceType = typeof(Dictionary), Name = Strings.Names.ThirdDiscount)]
        ThirdDiscount,
        [Discount(DiscountPercent = 75, ResourceType = typeof(Dictionary), Name = Strings.Names.FourthDiscount)]
        FourthDiscount
    }


}
