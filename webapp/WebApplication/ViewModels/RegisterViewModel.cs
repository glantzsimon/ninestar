using K9.Base.DataAccessLayer.Models;

namespace K9.WebApplication.ViewModels
{
    public class RegisterViewModel
    {
        public UserAccount.RegisterModel RegisterModel { get; set; }
        public string PromoCode { get; set; }
    }
}