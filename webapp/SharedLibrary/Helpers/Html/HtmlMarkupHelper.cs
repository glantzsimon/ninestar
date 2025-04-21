using Antlr.Runtime.Misc;
using K9.SharedLibrary.Extensions;
using System.IO;
using System.Text;
using System.Web.Mvc;

namespace K9.SharedLibrary.Helpers.Html
{
    public static class HtmlMarkupHelper
    {
        public static void ConvertToHtml<T>(ref T model)
        {
            Apply(model, value => ToMarkup(value));
        }

        public static void ConvertToCurly<T>(ref T model)
        {
            Apply(model, value => ToCurly(value));
        }

        private static string ToMarkup(string value) // {p} -> <p>
        {
            if (string.IsNullOrWhiteSpace(value)) return value;

            var sb = new StringBuilder();
            using (var sr = new StringReader(value))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Replace("{{", "##OPEN##").Replace("}}", "##CLOSE##");

                    // Only convert if curly braces are used
                    line = line.Replace("{", "<").Replace("}", ">");

                    line = line.Replace("##OPEN##", "{{").Replace("##CLOSE##", "}}");

                    sb.AppendLine(line);
                }
            }

            return sb.ToString();
        }

        private static string ToCurly(string value) // <p> -> {p}
        {
            if (string.IsNullOrWhiteSpace(value)) return value;

            var sb = new StringBuilder();
            using (var sr = new StringReader(value))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Replace("{{", "##OPEN##").Replace("}}", "##CLOSE##");

                    // Only convert if markup is used
                    line = line.Replace("<", "{").Replace(">", "}");

                    line = line.Replace("##OPEN##", "{{").Replace("##CLOSE##", "}}");

                    sb.AppendLine(line);
                }
            }

            return sb.ToString();
        }

        private static void Apply<T>(T model, Func<string, string> transform)
        {
            foreach (var propertyInfo in model.GetProperties())
            {
                if (propertyInfo.GetAttribute<AllowHtmlAttribute>() != null)
                {
                    var value = model.GetProperty(propertyInfo);
                    if (value != null)
                    {
                        model.SetProperty(propertyInfo, transform(value.ToString()));
                    }
                }
            }
        }
    }

}