using K9.WebApplication.Models;
using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.ViewModels
{
    public class QuickRegisterViewModel
    {
        public QuickRegisterModel RegisterModel { get; set; }
        
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.PromotionLabel)]
        public string PromoCode { get; set; }
    }
}