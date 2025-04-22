using Humanizer;

namespace K9.SharedLibrary.Helpers
{
    public static class StringHelper
    {

        public static string Pluralise(this string value)
        {
            return value.Pluralize();
        }

        public static string Singularise(this string value)
        {
            return value.Singularize(false);
        }

    }
}
