using K9.WebApplication.ViewModels.MobileApi;
using System;

namespace K9.WebApplication.Services
{
    public interface IMobileCheckoutService
    {
        MobileCheckoutIntent Start(int userId, MobileCheckoutRequest request);
        MobileCheckoutIntent Complete(int userId, Guid requestId);
    }

    public class MobileCheckoutException : Exception
    {
        public MobileCheckoutException(string code, int status = 409) : base(code) { Code = code; Status = status; }
        public string Code { get; }
        public int Status { get; }
    }
}
