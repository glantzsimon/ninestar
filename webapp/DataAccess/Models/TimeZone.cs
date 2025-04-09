using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(K9.Globalisation.Dictionary), ListName = Globalisation.Strings.Names.TimeZones, PluralName = Globalisation.Strings.Names.TimeZones, Name = Globalisation.Strings.Names.TimeZone)]
    public class TimeZone : ObjectBase
    {
        public const string StandardTimeZones = "StandardTimeZones";
        public const string ExtendedTimeZones = "ExtendedTimeZones";

        public string TimeZoneId { get; set; }
        public string WindowsTimeZoneId { get; set; }
        public string FriendlyName { get; set; }
        public string DisplayName { get; set; }
        public string Abbreviation { get; set; }
        public string UTCOffset { get; set; }
        public int? DisplayOrder { get; set; }

        public static (string DisplayName, string Abbreviation, string UTCOffset, int DisplayOrder)? GetTimeZoneInfo(string timeZoneId)
        {
            switch (timeZoneId)
            {
                case "Pacific Standard Time":
                case "America/Los_Angeles":
                    return ("Pacific Time (US & Canada)", "PDT", "UTC-07:00", 1);
                case "Mountain Standard Time":
                case "America/Denver":
                    return ("Mountain Time (US & Canada)", "MDT", "UTC-06:00", 2);
                case "Central Standard Time":
                case "America/Chicago":
                    return ("Central Time (US & Canada)", "CDT", "UTC-05:00", 3);
                case "Eastern Standard Time":
                case "America/New_York":
                    return ("Eastern Time (US & Canada)", "EDT", "UTC-04:00", 4);
                case "Atlantic Standard Time":
                case "America/Halifax":
                    return ("Atlantic Time (Canada)", "ADT", "UTC-03:00", 5);
                case "Newfoundland Standard Time":
                case "America/St_Johns":
                    return ("Newfoundland Time", "NDT", "UTC-02:30", 6);
                case "Hawaiian Standard Time":
                case "Pacific/Honolulu":
                    return ("Hawaii Time", "HST", "UTC-10:00", 7);
                case "Alaskan Standard Time":
                case "America/Anchorage":
                    return ("Alaska Time", "AKDT", "UTC-08:00", 8);
                case "US Mountain Standard Time":
                case "America/Phoenix":
                    return ("Arizona Time", "MST", "UTC-07:00", 9);
                case "GMT Standard Time":
                case "Europe/London":
                    return ("Greenwich Mean Time", "BST", "UTC+01:00", 10);
                case "W. Europe Standard Time":
                case "Europe/Berlin":
                    return ("Central European Time", "CEST", "UTC+02:00", 11);
                case "E. Europe Standard Time":
                case "Europe/Bucharest":
                    return ("Eastern European Time", "EEST", "UTC+03:00", 12);
                case "South Africa Standard Time":
                case "Africa/Johannesburg":
                    return ("South Africa Standard Time", "SAST", "UTC+02:00", 13);
                case "India Standard Time":
                case "Asia/Kolkata":
                    return ("India Standard Time", "IST", "UTC+05:30", 14);
                case "Singapore Standard Time":
                case "Asia/Singapore":
                    return ("Singapore Time", "8", "UTC+08:00", 15);
                case "China Standard Time":
                case "Asia/Shanghai":
                    return ("China Standard Time", "CST", "UTC+08:00", 16);
                case "Tokyo Standard Time":
                case "Asia/Tokyo":
                    return ("Japan Standard Time", "JST", "UTC+09:00", 17);
                case "Korea Standard Time":
                case "Asia/Seoul":
                    return ("Korea Standard Time", "KST", "UTC+09:00", 18);
                case "AUS Eastern Standard Time":
                case "Australia/Sydney":
                    return ("Australian Eastern Time", "AEST", "UTC+10:00", 19);
                case "New Zealand Standard Time":
                case "Pacific/Auckland":
                    return ("New Zealand Time", "NZST", "UTC+12:00", 20);
                case "E. South America Standard Time":
                case "America/Sao_Paulo":
                    return ("Brasilia Time", "-3", "UTC-03:00", 21);
                case "Argentina Standard Time":
                case "America/Argentina/Buenos_Aires":
                    return ("Argentina Time", "-3", "UTC-03:00", 22);
                case "Russian Standard Time":
                case "Europe/Moscow":
                    return ("Moscow Standard Time", "MSK", "UTC+03:00", 23);
                case "Arabian Standard Time":
                case "Asia/Dubai":
                    return ("Dubai Time", "4", "UTC+04:00", 24);
                default:
                    return null;
            }
        }

    }
}
