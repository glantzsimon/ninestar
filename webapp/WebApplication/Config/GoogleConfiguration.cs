﻿namespace K9.WebApplication.Config
{
    public class GoogleConfiguration
    {
        public static GoogleConfiguration Instance { get; set; }

        public string TrackingId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}