namespace K9.WebApplication.Services
{
    public interface IPdfService : IBaseService
    {
        byte[] HtmlToPdf(string html);
        byte[] UrlToPdf(string url);
    }
}