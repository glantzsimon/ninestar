namespace K9.WebApplication.Services
{
    public interface IMediaManagementService
    {
        string UploadToStorj(string localFilePath, string relativePath);
        string SaveMobileImage(byte[] bytes, int userId, bool articleImage = false);
        void DeleteMobileImage(string relativePath, int userId, bool articleImage = false);
    }
}
