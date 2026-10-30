using K9.WebApplication.Packages;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using K9.WebApplication.Config;

namespace K9.WebApplication.Services
{
    public class MediaService : BaseService, IMediaService
    {
        public MediaService(INineStarKiBasePackage my)
            : base(my)
        {
            _mediaBasePath = GetMediaBasePath(My.DefaultValuesConfiguration);
        }

        private readonly string _mediaBasePath;
        private bool _storjAvailable;
        private DateTime _lastCheck = DateTime.MinValue;
        private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(5);

        public static string BaseImagesPath { get; set; }
        public static string BaseVideosPath { get; set; }

        public static void InitialiseMediaPaths(DefaultValuesConfiguration configuration)
        {
            var basePath = GetMediaBasePath(configuration);
            SetMediaPaths(configuration, basePath + "/Images", basePath + "/Videos");
        }

        private static string GetMediaBasePath(DefaultValuesConfiguration configuration)
        {
#if Integration
            var basePath = configuration.SiteIntBaseUrl;
#elif DEBUG && !PRODUCTION
            var basePath = configuration.LocalMediaBasePath;
#else
            var basePath = configuration.LiveMediaBasePath;
#endif
            if (string.IsNullOrWhiteSpace(basePath))
            {
                throw new InvalidOperationException("The media base path for this environment is required.");
            }

            // SiteIntBaseUrl follows the existing scheme-less site URL convention.
            if (basePath.IndexOf("://", StringComparison.Ordinal) < 0)
            {
                basePath = "https://" + basePath;
            }

            Uri uri;
            if (!Uri.TryCreate(basePath, UriKind.Absolute, out uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                || !string.IsNullOrEmpty(uri.UserInfo) || !string.IsNullOrEmpty(uri.Query)
                || !string.IsNullOrEmpty(uri.Fragment))
            {
                throw new InvalidOperationException("The media base path must be an HTTP(S) application URL.");
            }

            return uri.AbsoluteUri.TrimEnd('/');
        }

        private static void SetMediaPaths(DefaultValuesConfiguration configuration, string imagesPath, string videosPath)
        {
            BaseImagesPath = imagesPath;
            BaseVideosPath = videosPath;
            configuration.BaseImagesPath = imagesPath;
            configuration.BaseVideosPath = videosPath;
            configuration.BaseEmailTemplateImagesPath = imagesPath;
            configuration.BaseBaseEmailTemplateVideosPath = videosPath;
        }

        public async Task CheckImageServiceHealthAsync()
        {
            if (!My.DefaultValuesConfiguration.UseHostedMedia || DateTime.UtcNow - _lastCheck < CheckInterval) return;

            try
            {
                var testUrl = $"{My.DefaultValuesConfiguration.HostedImagesBasePath.TrimEnd('/')}/company/logo.png";
                using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) })
                {
                    using (var response = await client.GetAsync(testUrl))
                    {
                        _storjAvailable = response.IsSuccessStatusCode;
                    }
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
            return My.DefaultValuesConfiguration.UseHostedMedia && _storjAvailable
                ? My.DefaultValuesConfiguration.HostedImagesBasePath
                : $"{_mediaBasePath}/Images";
        }

        private string GetBaseVideosPath()
        {
            return My.DefaultValuesConfiguration.UseHostedMedia && _storjAvailable
                ? My.DefaultValuesConfiguration.HostedVideosBasePath
                : $"{_mediaBasePath}/Videos";
        }

        public void ScheduledHealthCheck()
        {
            CheckImageServiceHealthAsync().GetAwaiter().GetResult();
            SetMediaPaths(My.DefaultValuesConfiguration, GetBaseImagesPath(), GetBaseVideosPath());
        }
    }
}