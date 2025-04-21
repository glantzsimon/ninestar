using K9.WebApplication.Packages;
using System;
using System.Diagnostics;
using System.IO;

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

            var safeRelativePath = relativePath.TrimStart('\\', '/').Replace("\\", "/");
            var storjPath = $"{Bucket}/{BasePath}/{safeRelativePath}";

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

            process.Start();

            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();

            process.WaitForExit(18000); // 18s timeout

            if (!process.HasExited)
            {
                process.Kill();
                throw new TimeoutException("Storj upload process timed out.");
            }

            if (process.ExitCode != 0)
            {
                throw new ApplicationException($"Storj upload failed (code {process.ExitCode}). stderr:\n{stderr}");
            }

            var baseUrl = My.DefaultValuesConfiguration.BaseImagesPath.TrimEnd('/');
            var finalUrl = $"{baseUrl}/{safeRelativePath}";

            My.Logger.Info($"[SUCCESS] Uploaded to Storj: {finalUrl}");
            return finalUrl;
        }

    }
}