using K9.Base.DataAccessLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.ViewModels
{
    public class RegisterViewModel
    {
        public UserAccount.RegisterModel RegisterModel { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = K9.Globalisation.Strings.Labels.PromoCodeLabel)]
        public string PromoCode { get; set; }
    }
}