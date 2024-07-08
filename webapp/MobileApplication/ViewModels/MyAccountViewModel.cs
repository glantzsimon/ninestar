using System.ComponentModel.DataAnnotations;
using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;

namespace K9.WebApplication.ViewModels
{
    public class MyAccountViewModel
    {
        public User User { get; set; }
        public UserMembership Membership { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.RedeemPromoCode)]
        public string PromoCode { get; set; }
    }
}