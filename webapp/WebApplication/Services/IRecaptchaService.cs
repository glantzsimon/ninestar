namespace K9.WebApplication.Services
{
    public interface IRecaptchaService : IBaseService
    {
        bool Validate(string encodedResponse);
    }
}