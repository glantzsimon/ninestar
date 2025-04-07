using System;
using System.IO;
using K9.DataAccessLayer.Models;

namespace K9.WebApplication.Config
{
    public class DefaultValuesConfiguration
    {
        public static DefaultValuesConfiguration Instance { get; set; }

        public string DefaultUserId { get; set; }
        public string WhatsAppSupportNumber { get; set; }
        public string CurrentTimeZone { get; set; }
        public string BaseImagesPath { get; set; }
        public string BaseVideosPath { get; set; }
        public string BaseEmailTemplateImagesPath { get; set; }
        public string BaseBaseEmailTemplateVideosPath { get; set; }
        public string SiteBaseUrl { get; set; }
        public string CompanyAddress { get; set; }
        public string SwephPath { get; set; }
        public int EmailQueueMaxBatchSize { get; set; } = 20;

        public SystemSetting SystemSettings { get; set; }

        public void ValidateSwephPath()
        {
            if (!Directory.Exists(SwephPath))
            {
                throw new Exception($"Invalid path to Swiss Ephemeris files: {SwephPath}");
            }
        }
    }
}