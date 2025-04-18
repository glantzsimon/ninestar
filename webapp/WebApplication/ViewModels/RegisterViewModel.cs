using K9.Base.DataAccessLayer.Models;
using System.ComponentModel.DataAnnotations;
using K9.DataAccessLayer.Models;

namespace K9.WebApplication.ViewModels
{
    public class RegisterViewModel
    {
        public UserAccount.RegisterModel RegisterModel { get; set; }

        public UserInfo UserInfo { get; set; }
        
        [Display(ResourceType = typeof(Globalisation.Dictionary),
            Name = Globalisation.Strings.Names.AllowMarketingEmails)]
        public bool AllowMarketingEmails { get; set; }
        
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.PromotionLabel)]
        public string PromoCode { get; set; }
    }
}