using K9.SharedLibrary.Extensions;
using System.Threading;

namespace K9.WebApplication.Services
{
    public class ShopService : IShopService
    {
        
        public string GetLocale()
        {
            var locale = Thread.CurrentThread.CurrentUICulture.GetMetaLocaleName("_");
            locale = locale == "en_US" ? "en_GB" : locale;

            switch (locale)
            {
                case "nb_NO":
                case "se_NO":
                case "nn_NO":
                case "smj_NO":
                case "sma_NO":
                    locale = "nn_NO";
                    break;

                case "fi_FI":
                case "sv_FI":
                case "se_FI":
                case "sms_FI":
                case "smn_FI":
                    locale = "fi_FI";
                    break;

                case "sv_SE":
                case "se_SE":
                case "smj_SE":
                case "sma_SE":
                    locale = "se_SE";
                    break;
            }

            return locale;
        }

        public string GetShopPrefix()
        {
            var locale = GetLocale();
            var suffix = "";
            switch (locale)
            {
                case "en_GB":
                    suffix = "co.uk";
                    break;

                case "fr_FR":
                    suffix = "fr";
                    break;
                    
                case "de_DE":
                    suffix = "de";
                    break;

                case "nl_BE":
                    suffix = "be";
                    break;

                case "fr_BE":
                    suffix = "be";
                    break;

                case "da_DK":
                    suffix = "dk";
                    break;

                case "es_ES":
                    suffix = "es";
                    break;

                case "en_IE":
                    suffix = "ie";
                    break;

                case "it_IT":
                    suffix = "it";
                    break;

                case "nn_NO":
                    suffix = "no";
                    break;

                case "pl_PL":
                    suffix = "pl";
                    break;

                case "fr_CH":
                    suffix = "ch";
                    break;

                case "de_CH":
                    suffix = "ch";
                    break;

                case "it_CH":
                    suffix = "ch";
                    break;

                case "fi_FI":
                    suffix = "fi";
                    break;

                case "se_SE":
                    suffix = "se";
                    break;

                case "de_AT":
                    suffix = "at";
                    break;

                case "en_US":
                    suffix = "com";
                    break;

                default:
                    suffix = "net";
                    break;
            }

            return $"//shop.spreadshirt.{suffix}";
        }
    }
}