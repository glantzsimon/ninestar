namespace K9.WebApplication.Services
{
    public interface IMediaManagementService
    {
        string UploadToStorj(string localFilePath, string relativePath);
    }
}