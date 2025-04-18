using Google.Apis.Auth;
using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Packages;
using System;
using K9.SharedLibrary.Extensions;

namespace K9.WebApplication.Services
{
    public class GoogleService : BaseService, IGoogleService
    {
        private readonly GoogleConfiguration _googleConfig;

        public GoogleService(INineStarKiBasePackage my, IOptions<GoogleConfiguration> googleConfig) : base(my)
        {
            _googleConfig = googleConfig.Value;
        }

        public ServiceResult Authenticate(string googleIdToken)
        {
            var result = new ServiceResult();

            try
            {
                var payload = GoogleJsonWebSignature.ValidateAsync(googleIdToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleConfig.ClientId } // This should match the one in your JS frontend
                }).GetAwaiter().GetResult();

                if (payload == null)
                {
                    result.Errors.Add(new ServiceError
                    {
                        ErrorMessage = "Invalid Google ID token."
                    });
                    return result;
                }

                var user = new User
                {
                    EmailAddress = payload.Email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    Username = Guid.NewGuid().ToString()
                };

                result.Data = user;
                result.IsSuccess = true;
            }
            catch (InvalidJwtException ex)
            {
                result.Errors.Add(new ServiceError
                {
                    ErrorMessage = $"Token validation failed: {ex.GetFullErrorMessage()}"
                });
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ServiceError
                {
                    ErrorMessage = $"Unexpected error: {ex.GetFullErrorMessage()}"
                });
            }

            return result;
        }
    }
}