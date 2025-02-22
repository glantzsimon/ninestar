using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace K9.SharedLibrary.Helpers
{
    public static class CurrencyTools
    {
        private static IDictionary<string, Tuple<string, CultureInfo>> map;

        static CurrencyTools()
        {
            map = CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .Where(c => !c.IsNeutralCulture)
                .Select(culture =>
                {
                    try
                    {
                        var regionInfo = new RegionInfo(culture.LCID);
                        return new Tuple<string, CultureInfo>(regionInfo.ISOCurrencySymbol, culture);
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(e => e != null)
                .GroupBy(e => e.Item1)
                .ToDictionary(x => x.Key, x => new Tuple<string, CultureInfo>(x.First().Item1, x.First().Item2));
        }

        public static string GetCurrencySymbolFromIso(string isoCurrencySymbol)
        {
            map.TryGetValue(isoCurrencySymbol, out var tuple);
            return tuple != null ? tuple.Item1 : isoCurrencySymbol;
        }

        public static CultureInfo GetCultureInfoFromIso(string isoCurrencySymbol)
        {
            map.TryGetValue(isoCurrencySymbol, out var tuple);
            return tuple != null ? tuple.Item2 : Thread.CurrentThread.CurrentUICulture;
        }

        public static string ToFormattedString(this double? amount)
        {
            return (amount ?? 0).ToFormattedString();
        }

        public static string ToFormattedString(this double amount)
        {
            return amount.ToString("C0", CultureInfo.GetCultureInfo("en-US"));
        }
    }
}
