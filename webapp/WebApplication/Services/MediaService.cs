using K9.WebApplication.Packages;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace K9.WebApplication.Services
{
    public class MediaService : BaseService, IMediaService
    {
        public MediaService(INineStarKiBasePackage my)
            : base(my)
        {
        }

        private static bool _storjAvailable = true;
        private static DateTime _lastCheck = DateTime.MinValue;
        private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(5);

        public static string BaseMediaPath { get; set; }
        public static string BaseImagesPath => $"{BaseMediaPath}/Images";
        public static string BaseVideosPath => $"{BaseMediaPath}/Videos";

        public string GetBaseMediaPath()
        {
            return _storjAvailable
                ? My.DefaultValuesConfiguration.HostedMediaBasePath
                : My.DefaultValuesConfiguration.LocalMediaBasePath;
        }

        public async Task CheckImageServiceHealthAsync()
        {
            if (DateTime.UtcNow - _lastCheck < CheckInterval) return;

            var testUrl = $"{My.DefaultValuesConfiguration.HostedMediaBasePath.TrimEnd('/')}/Images/company/logo.png";
            try
            {
                using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) })
                {
                    var response = await client.GetAsync(testUrl);
                    _storjAvailable = response.IsSuccessStatusCode;
                };
            }
            catch
            {
                _storjAvailable = false;
            }

            _lastCheck = DateTime.UtcNow;
        }

        public void ScheduledHealthCheck()
        {
            CheckImageServiceHealthAsync().GetAwaiter().GetResult();
            BaseMediaPath = GetBaseMediaPath();
        }
    }
}