using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Packages;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace K9.WebApplication.Services
{
    public class GoogleService : BaseService, IGoogleService
    {
        private readonly GoogleConfiguration _googleConfig;

        public GoogleService(INineStarKiBasePackage my, IOptions<GoogleConfiguration> googleConfig) : base(my)
        {
            _googleConfig = googleConfig.Value;
        }

        public ServiceResult Authenticate()
        {
            var result = new ServiceResult();

            try
            {
                var auth = HttpContext.Current.GetOwinContext().Authentication;
                var googleResult = auth.AuthenticateAsync("ExternalCookie").GetAwaiter().GetResult();

                if (googleResult?.Identity == null || !googleResult.Identity.IsAuthenticated)
                {
                    result.Errors.Add(new ServiceError
                    {
                        FieldName = "",
                        ErrorMessage = "User not found"
                    });

                    return result;
                }

                var emailClaim = googleResult.Identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                var nameClaim = googleResult.Identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                var firstNameClaim = googleResult.Identity.Claims.FirstOrDefault(c => c.Type == "given_name");
                var lastNameClaim = googleResult.Identity.Claims.FirstOrDefault(c => c.Type == "family_name");

                string email = emailClaim?.Value;
                string fullName = nameClaim?.Value;
                string firstName = firstNameClaim?.Value;
                string lastName = lastNameClaim?.Value;

                // Fallback if given_name/family_name are missing
                if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                {
                    if (!string.IsNullOrEmpty(fullName) && fullName.Contains(" "))
                    {
                        var parts = fullName.Split(' ');
                        firstName = parts[0] ?? "";
                        lastName = string.Join(" ", parts.Skip(1)) ?? "";
                    }
                    else
                    {
                        firstName = fullName ?? "";
                        lastName = "";
                    }
                }

                if (string.IsNullOrEmpty(email))
                {
                    result.Errors.Add(new ServiceError
                    {
                        FieldName = "",
                        ErrorMessage = "Google account did not return an email address."
                    });
                    return result;
                }

                result.Data = new User
                {
                    EmailAddress = email,
                    Username = Guid.NewGuid().ToString(), // Or email if you prefer
                    FirstName = firstName,
                    LastName = lastName
                };

                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ServiceError
                {
                    FieldName = "",
                    ErrorMessage = ex.Message
                });
            }

            return result;
        }
    }
}