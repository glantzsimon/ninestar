using K9.SharedLibrary.Extensions;
using System.Reflection;

namespace K9.SharedLibrary.Helpers
{
    public static class DictionaryHelper
    {
        public static string LookUp<T>(string resourceKey) where T : Assembly
        {
            return !string.IsNullOrEmpty(resourceKey) ? typeof(T).GetValueFromResource(resourceKey) : string.Empty;
        }
    }
}
