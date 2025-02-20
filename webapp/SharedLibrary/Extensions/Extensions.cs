
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Exceptions;
using K9.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace K9.SharedLibrary.Extensions
{
    public static partial class Extensions
    {
        public static string GetLocaleName(this CultureInfo cultureInfo)
        {
            var languageName = cultureInfo.TwoLetterISOLanguageName.ToLower();
            return cultureInfo.IsNeutralCulture ? languageName : $"{languageName}-{new RegionInfo(cultureInfo.LCID).TwoLetterISORegionName.ToLower()}";
        }

        public static string GetMetaLocaleName(this CultureInfo cultureInfo, string delimiter = "-")
        {
            var languageName = cultureInfo.TwoLetterISOLanguageName.ToLower();
            return cultureInfo.IsNeutralCulture ? languageName : $"{languageName}_{new RegionInfo(cultureInfo.LCID).TwoLetterISORegionName.ToUpper()}";
        }

        public static string GetLocaleLanguage(this CultureInfo cultureInfo)
        {
            return cultureInfo.IsNeutralCulture ? cultureInfo.EnglishName : cultureInfo.Parent.EnglishName;
        }

        public static string GetFullErrorMessage(this Exception ex)
        {
            var sb = new StringBuilder();
            Exception innerException = ex.InnerException;

            sb.AppendLine(ex.Message);

            while (innerException != null)
            {
                sb.AppendLine(innerException.Message);
                innerException = innerException.InnerException;
            }

            return sb.ToString();
        }

        public static bool IsDuplicateIndexError(this Exception ex)
        {
            return ex.GetFullErrorMessage().Contains("Cannot insert duplicate key row in object");
        }

        public static bool IsDeleteConflictError(this Exception ex)
        {
            return ex.GetFullErrorMessage().ToLower().Contains("delete statement conflicted with the reference constraint");
        }

        public static string GetDuplicateIndexErrorPropertyName(this Exception ex)
        {
            if (IsDuplicateIndexError(ex))
            {
                var indexRegex = new Regex(@"(?:IX_)([A-Z]\w+)");
                var fullErrorMessage = ex.GetFullErrorMessage();
                return indexRegex.IsMatch(fullErrorMessage) ? indexRegex.Match(fullErrorMessage).Groups[1].Value : "";
            }
            return string.Empty;
        }

        public static string GetValueFromResource(this Type resourceType, string key)
        {
            var propInfo = resourceType.GetProperty(key);
            if (propInfo != null)
            {
                return propInfo.GetValue(null, null).ToString();
            }
            throw new KeyNotFoundException($"The key {key} was not found.");
        }

        public static List<PropertyInfo> GetFileSourceProperties(this IObjectBase item)
        {
            return item.GetType().GetProperties().Where(p => p.PropertyType == typeof(FileSource)).ToList();
        }

        public static void InitFileSources(this IObjectBase item)
        {
            var fileSourceProperties = item.GetFileSourceProperties();
            foreach (var propertyInfo in fileSourceProperties)
            {
                var fileSource = (FileSource)item.GetProperty(propertyInfo);
                if (fileSource == null)
                {
                    fileSource = Activator.CreateInstance<FileSource>();
                    item.SetProperty(propertyInfo, fileSource);
                }

                var info = propertyInfo.GetAttribute<FileSourceInfo>();
                if (info == null)
                {
                    throw new FileSourceFilePathNotSpecifiedException();
                }
                fileSource.Filter = info.Filter;
                fileSource.PathToFiles = string.Join("/", info.PathToFiles, item.GetType().Name, item.Id.ToString());
            }
        }
    }
}

