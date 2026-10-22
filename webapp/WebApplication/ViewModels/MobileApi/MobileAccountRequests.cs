using System;
using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.ViewModels.MobileApi
{
    public class MobileLoginRequest
    {
        [Required, StringLength(200)] public string Username { get; set; }
        [Required, StringLength(1000)] public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class MobileAccountUpdateRequest
    {
        [Required, StringLength(100)] public string FirstName { get; set; }
        [Required, StringLength(100)] public string LastName { get; set; }
        [Required, EmailAddress, StringLength(200)] public string EmailAddress { get; set; }
        [StringLength(100)] public string PhoneNumber { get; set; }
        public bool AllowMarketingEmails { get; set; }
        [Required] public MobileCalculationRequest Settings { get; set; }
    }

    public class MobileActivationRequest
    {
        public Guid ActivationId { get; set; }
        [Range(1, 9)] public int Digit1 { get; set; }
        [Range(1, 9)] public int Digit2 { get; set; }
        [Range(1, 9)] public int Digit3 { get; set; }
        [Range(1, 9)] public int Digit4 { get; set; }
        [Range(1, 9)] public int Digit5 { get; set; }
        [Range(1, 9)] public int Digit6 { get; set; }
    }

    public class MobileDeleteAccountRequest
    {
        public bool Confirm { get; set; }
        [Required, StringLength(1000)] public string Password { get; set; }
    }
}
