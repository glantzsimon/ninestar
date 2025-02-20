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

        public LogService(INineStarKiBasePackage my) : base(my)
        {
        }

        public List<LogItem> GetLogItems()
        {
            var logItems = new List<LogItem>();

            foreach (var lines in GetLogFiles())
            {
                LogItem logItem = null;

                foreach (var line in lines)
                {
                    if (line.Contains(withResultsAs) ||
                        line.Contains(dataTablesDef) ||
                        line.Contains(dataTablesDef2))
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    var formattedLine = CleanLine(line);
                    var words = formattedLine.Split(' ');
                    var pieces = Regex.Split(formattedLine, separator);
                    var loggedOn = GetLoggedOn(words);

                    if (loggedOn.HasValue)
                    {
                        if (logItem != null)
                        {
                            logItems.Add(logItem);
                        }

                        logItem = new LogItem
                        {
                            LoggedOn = loggedOn.Value
                        };
                    }

                    if (logItem != null)
                    {
                        if (pieces.Length == 3)
                        {
                            logItem.ClassName = GetClassName(pieces);
                            logItem.MethodName = GetMethodName(pieces);
                            logItem.ErrorMessage = pieces.Last();
                        }
                        else
                        {
                            if (loggedOn.HasValue)
                            {
                                logItem.ErrorMessage = words.Skip(2).Aggregate((a, b) => $"{a} {b}");
                            }
                            else
                            {
                                logItem.ErrorMessage += Environment.NewLine;
                                logItem.ErrorMessage += formattedLine;
                            }
                        }
                    }
                }
            }

            return logItems;
        }

        private DateTime? GetLoggedOn(string[] words)
        {
            if (words.Length < 2)
            {
                return null;
            }

            if (DateTime.TryParse(words.FirstOrDefault(), out var date))
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
            return pieces.First().Trim().Split(' ').LastOrDefault();
        }

        private string GetMethodName(string[] pieces)
        {
            return pieces[1].Trim().Split(' ').FirstOrDefault();
        }

        private string CleanLine(string line)
        {
            return line
                .Replace(cleanUpText1, "")
                .Replace(cleanUpText2, "")
                .Replace("  ", " ")
                .Replace("  ", " ");
        }

        private IEnumerable<string[]> GetLogFiles()
        {
            return Directory.GetFiles(PathToLogFiles).ToList().Select(File.ReadAllLines);
        }

        private string PathToLogFiles => Path.Combine(HttpRuntime.AppDomainAppPath, "Logs");
    }
}