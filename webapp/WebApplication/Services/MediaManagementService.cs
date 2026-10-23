using K9.WebApplication.Packages;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;

namespace K9.WebApplication.Services
{
    public class MediaManagementService : BaseService, IMediaManagementService
    {
        private const string UplinkPath = @"C:\Program Files\Storj\uplink.exe";
        private const string Bucket = "cdn-media";
        private const string BasePath = "Images";

        public MediaManagementService(INineStarKiBasePackage my) : base(my)
        {
        }

        public string SaveMobileImage(byte[] bytes, int userId, bool articleImage = false)
        {
            if (userId <= 0 || bytes == null || bytes.Length < 8 || bytes.Length > 2 * 1024 * 1024)
                throw new ArgumentException("invalid_image");
            var png = bytes[0] == 137 && bytes[1] == 80 && bytes[2] == 78 && bytes[3] == 71 &&
                bytes[4] == 13 && bytes[5] == 10 && bytes[6] == 26 && bytes[7] == 10;
            var jpeg = bytes[0] == 255 && bytes[1] == 216 && bytes[2] == 255;
            if (!png && !jpeg) throw new ArgumentException("unsupported_image");
            var image = new WebImage(bytes);
            if (image.Width > 4096 || image.Height > 4096) throw new ArgumentException("image_dimensions_exceeded");
            // Decode and re-encode to a fixed image type. Neither a supplied
            // extension nor an executable/polyglot upload is copied to the server.
            image.Resize(1600, 1600, preserveAspectRatio: true, preventEnlarge: true);
            var relative = MobileImageFolder(userId, articleImage) + Guid.NewGuid().ToString("N") + ".png";
            var physical = Path.Combine(HttpRuntime.AppDomainAppPath, relative.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(physical));
            File.WriteAllBytes(physical, image.GetBytes("png"));
            return relative;
        }

        public void DeleteMobileImage(string relativePath, int userId, bool articleImage = false)
        {
            if (userId <= 0 || string.IsNullOrEmpty(relativePath)) throw new ArgumentException("invalid_image_path");
            var folder = MobileImageFolder(userId, articleImage);
            if (!relativePath.StartsWith(folder, StringComparison.Ordinal) ||
                !Regex.IsMatch(relativePath.Substring(folder.Length), @"\A[a-f0-9]{32}\.png\z"))
                throw new ArgumentException("invalid_image_path");
            var physical = Path.Combine(HttpRuntime.AppDomainAppPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(physical)) File.Delete(physical);
        }

        private static string MobileImageFolder(int userId, bool articleImage) => articleImage
            ? "/Images/mobile/articles/" : "/Images/mobile/avatars/" + userId + "/";

        public string UploadToStorj(string localFilePath, string relativePath)
        {
            if (!System.IO.File.Exists(UplinkPath))
            {
                throw new FileNotFoundException("Uplink executable not found.", UplinkPath);
            }

            if (!System.IO.File.Exists(localFilePath))
            {
                throw new FileNotFoundException("Source image file not found.", localFilePath);
            }

            My.Logger.Info($"MediaManagementService => UploadToStorj => File exists: {localFilePath}");

            var safeRelativePath = relativePath.TrimStart('\\', '/').Replace("\\", "/");
            var storjPath = $"{Bucket}/{BasePath}/{safeRelativePath}";

            My.Logger.Info($"MediaManagementService => UploadToStorj => SafeRelativePath: {safeRelativePath} => StorjPath: {storjPath}");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = UplinkPath,
                    Arguments = $"cp \"{localFilePath}\" sj://{storjPath}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            My.Logger.Info($"MediaManagementService => UploadToStorj => Process starting");

            process.Start();

            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();

            process.WaitForExit(18000); // 18s timeout

            if (!process.HasExited)
            {
                My.Logger.Info($"MediaManagementService => UploadToStorj => Process has excited");

                process.Kill();
                throw new TimeoutException("Storj upload process timed out.");
            }

            if (process.ExitCode != 0)
            {
                var errorMessage = $"Storj upload failed (code {process.ExitCode}). stderr:\n{stderr}"; 
                My.Logger.Info($"MediaManagementService => UploadToStorj => {errorMessage}");
                throw new Exception(errorMessage);
            }

            var baseUrl = My.DefaultValuesConfiguration.BaseImagesPath.TrimEnd('/');
            var finalUrl = $"{baseUrl}/{safeRelativePath}";

            My.Logger.Info($"Uploaded to Storj: {finalUrl}");
            return finalUrl;
        }

    }
}
