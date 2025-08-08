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

        public static string BaseImagesPath { get; set; }
        public static string BaseVideosPath { get; set; }

        public async Task CheckImageServiceHealthAsync()
        {
            if (DateTime.UtcNow - _lastCheck < CheckInterval) return;

            var testUrl = $"{My.DefaultValuesConfiguration.HostedImagesBasePath.TrimEnd('/')}/company/logo.png";
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

        private string GetBaseImagesPath()
        {
            return _storjAvailable
                ? My.DefaultValuesConfiguration.HostedImagesBasePath
                : $"{My.DefaultValuesConfiguration.LocalMediaBasePath.TrimEnd('/')}/Images";
        }

        private string GetBaseVideosPath()
        {
            return _storjAvailable
                ? My.DefaultValuesConfiguration.HostedVideosBasePath
                : $"{My.DefaultValuesConfiguration.LocalMediaBasePath.TrimEnd('/')}/Videos";
        }

        public void ScheduledHealthCheck()
        {
            CheckImageServiceHealthAsync().GetAwaiter().GetResult();
            BaseImagesPath = GetBaseImagesPath();
            BaseVideosPath = GetBaseVideosPath();
        }
    }
}