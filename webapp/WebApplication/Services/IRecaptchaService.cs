namespace K9.WebApplication.Services
{
    public interface IRecaptchaService
    {
        bool Validate(string encodedResponse);
    }
}