using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace K9.WebApplication.Services
{
    public class LogService : BaseService, ILogService
    {
        private const string withResultsAs = "WITH RESULTS AS";
        private const string dataTablesDef = "&draw=";
        private const string dataTablesDef2 = "\"draw\":";
        private const string cleanUpText1 = "K9.WebApplication.Startup+<>c";
        private const string cleanUpText2 = "K9.WebApplication.Startup";
        private const string separator = "=>";

        private static readonly List<string> hangFireEntries = new List<string>
        {
            "heartbeat",
            "Removing outdated records",
            "ProcessQueue => Sending 0 emails",
            "Aggregating records in",
            "recurring job(s) processed by scheduler",
            "Hangfire",
            "ProcessQueue => Sending"
        };

        // Compiled regex to normalize multiple spaces into one.
        private static readonly Regex multipleSpacesRegex = new Regex(@"\s{2,}", RegexOptions.Compiled);

        public LogService(INineStarKiBasePackage my) : base(my)
        {
        }

        public List<LogItem> GetLogItems()
        {
            var logItems = new List<LogItem>();

            foreach (var lines in GetLogFiles())
            {
                LogItem currentItem = null;

                foreach (var line in lines)
                {
                    if (ShouldSkipLine(line))
                    {
                        continue;
                    }

                    var formattedLine = CleanLine(line);
                    var words = formattedLine.Split(' ');
                    var pieces = Regex.Split(formattedLine, separator);
                    var loggedOn = GetLoggedOn(words);

                    // A new loggedOn value signals the start of a new log item.
                    if (loggedOn.HasValue)
                    {
                        if (currentItem != null)
                        {
                            logItems.Add(currentItem);
                        }

                        currentItem = new LogItem
                        {
                            LoggedOn = loggedOn.Value,
                            ErrorMessage = string.Empty  // Initialize to prevent null concatenation
                        };
                    }

                    if (currentItem != null)
                    {
                        if (pieces.Length == 3)
                        {
                            currentItem.ClassName = GetClassName(pieces);
                            currentItem.MethodName = GetMethodName(pieces);
                            currentItem.ErrorMessage = pieces.Last();
                        }
                        else
                        {
                            // If a new log entry is starting, use the remaining words as the initial error message.
                            if (loggedOn.HasValue)
                            {
                                currentItem.ErrorMessage = string.Join(" ", words.Skip(2));
                            }
                            else
                            {
                                currentItem.ErrorMessage = (currentItem.ErrorMessage ?? string.Empty)
                                                           + Environment.NewLine + formattedLine;
                            }
                        }
                    }
                }
                
                // Add the last log item from this file if available.
                if (currentItem != null)
                {
                    logItems.Add(currentItem);
                }
            }

            return logItems;
        }

        /// <summary>
        /// Returns true if the line should be skipped.
        /// </summary>
        private bool ShouldSkipLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return true;
            }

            var lineLower = line.ToLower();

            // Skip if the line contains any of these markers.
            if (lineLower.Contains(withResultsAs) ||
                lineLower.Contains(dataTablesDef) ||
                lineLower.Contains(dataTablesDef2))
            {
                return true;
            }

            // Skip if the line contains any Hangfire-specific entry.
            if (hangFireEntries.Any(entry => lineLower.Contains(entry)))
            {
                return true;
            }

            return false;
        }

        private DateTime? GetLoggedOn(string[] words)
        {
            if (words.Length < 2)
            {
                return null;
            }

            if (DateTime.TryParse(words[0], out var date))
            {
                if (DateTime.TryParse(words[1], out var time))
                {
                    date = date.Add(time.TimeOfDay);
                }
                return date;
            }

            return null;
        }

        private string GetClassName(string[] pieces)
        {
            // Assumes the first piece contains the class name; further split by space and take the last token.
            return pieces.First().Trim().Split(' ').LastOrDefault();
        }

        private string GetMethodName(string[] pieces)
        {
            // Assumes the second piece contains the method name; further split and take the first token.
            return pieces.Length > 1 ? pieces[1].Trim().Split(' ').FirstOrDefault() : string.Empty;
        }

        private string CleanLine(string line)
        {
            // Remove unwanted text then normalize multiple spaces.
            string cleaned = line.Replace(cleanUpText1, "").Replace(cleanUpText2, "");
            return multipleSpacesRegex.Replace(cleaned, " ");
        }

        private IEnumerable<IEnumerable<string>> GetLogFiles()
        {
            if (!Directory.Exists(PathToLogFiles))
            {
                yield break;
            }

            var files = Directory.GetFiles(PathToLogFiles)
                .OrderByDescending(file => File.GetLastWriteTime(file))
                .Take(3);

            // Using File.ReadLines for lazy evaluation
            foreach (var file in files)
            {
                yield return File.ReadLines(file);
            }
        }

        private string PathToLogFiles => Path.Combine(HttpRuntime.AppDomainAppPath, "Logs");
    }
}
