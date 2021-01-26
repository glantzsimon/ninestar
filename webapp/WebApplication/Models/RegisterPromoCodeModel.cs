using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.Models
{
    public class RegisterPromoCodeModel
    {
        public string Username { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.PromoCodeLabel)]
        public string PromoCode { get; set; }
    }
}