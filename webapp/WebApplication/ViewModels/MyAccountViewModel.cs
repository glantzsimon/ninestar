using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.ViewModels
{
    public class MyAccountViewModel
    {
        public User User { get; set; }

        public DataAccessLayer.Models.UserMembership Membership { get; set; }

        public List<UserConsultation> Consultations { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.RedeemPromoCode)]
        public string PromoCode { get; set; }
    }
}