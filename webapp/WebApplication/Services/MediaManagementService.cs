using K9.WebApplication.Packages;
using System.Diagnostics;

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

        public bool UploadToStorj(string localFilePath, string relativePath, out string storjUrl)
        {
            storjUrl = null;

            if (!System.IO.File.Exists(UplinkPath))
            {
                My.Logger.Error("Uplink.exe not found.");
                return false;
            }

            if (!System.IO.File.Exists(localFilePath))
            {
                My.Logger.Error($"File not found: {localFilePath}");
                return false;
            }

            var safeRelativePath = relativePath.TrimStart('\\', '/');
            var storjPath = $"{Bucket}/{BasePath}/{safeRelativePath.Replace("\\", "/")}";

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
           
            process.WaitForExit(18000); // 10 seconds
            if (!process.HasExited)
            {
                process.Kill();
                My.Logger.Error("Storj upload process timed out.");
                return false;
            }

            if (process.ExitCode == 0)
            {
                storjUrl = $"https://link.storjshare.io/raw/{Bucket}/{BasePath}/{relativePath.Replace("\\", "/")}";
                My.Logger.Info($"[SUCCESS] Uploaded to Storj: {storjUrl}");
                return true;
            }
            else
            {
                My.Logger.Error(($"Storj upload failed:\n{stderr}"));
                return false;
            }
        }
    }
}