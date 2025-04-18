using System;
using K9.Base.Globalisation;
using System.ComponentModel.DataAnnotations;
using K9.Base.DataAccessLayer.Enums;
using K9.Base.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;

namespace K9.WebApplication.Models
{
    public class QuickRegisterModel
    {
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Dictionary), Name = "PasswordLabel")]
        [Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = "FieldIsRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = "PasswordMinLengthError", MinimumLength = 8)]
        public string Password { get; set; }
        
        [Display(ResourceType = typeof(Dictionary), Name = "EmailAddressLabel")]
        [EmailAddress(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = "InvalidEmailAddress")]
        [Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = "FieldIsRequired")]
        public string EmailAddress { get; set; }

        public UserAccount.RegisterModel ToRegisterModel()
        {
            var registerModel = new UserAccount.RegisterModel();
            this.MapTo(registerModel);
            registerModel.UserName = Guid.NewGuid().ToString();
            registerModel.FirstName = Guid.NewGuid().ToString();
            registerModel.LastName = Guid.NewGuid().ToString();
            registerModel.ConfirmPassword = Password;
            registerModel.BirthDate = DateTime.Now.AddYears(36);
            registerModel.Gender = EGender.Other;

            return registerModel;
        }
    }
}