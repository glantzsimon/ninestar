using System.Threading.Tasks;

namespace K9.WebApplication.Services
{
    public interface IPdfService : IBaseService
    {
        byte[] HtmlToPdf(string html);
        Task<byte[]> UrlToPdf(string url);
    }
}