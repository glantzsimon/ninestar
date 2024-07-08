using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.ViewModels
{
    public class AssignPromoCodeViewModel
    {
        [Display(Name = "Promo Code")]
        public string PromoCode { get; set; }
        public int UserId { get; set; }
    }
}