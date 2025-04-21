namespace K9.WebApplication.Services
{
    public interface IMediaManagementService
    {
        bool UploadToStorj(string localFilePath, string relativePath, out string storjUrl);
    }
}