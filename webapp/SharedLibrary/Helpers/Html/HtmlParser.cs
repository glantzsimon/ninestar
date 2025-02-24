using K9.SharedLibrary.Extensions;
using System.IO;
using System.Text;
using System.Web.Mvc;

namespace K9.SharedLibrary.Helpers.Html
{
    public static class HtmlParser
    {
        public static void ParseHtml<T>(ref T model)
        {
            ParseHtml(model);
        }

        public static T ParseHtml<T>(T model)
        {
            foreach (var propertyInfo in model.GetProperties())
            {
                if (propertyInfo.GetAttribute<AllowHtmlAttribute>() != null)
                {
                    var value = model.GetProperty(propertyInfo);
                    if (value != null)
                    {
                        model.SetProperty(propertyInfo, ParseHtml(value.ToString()));
                    }
                }
            }

            return model;
        }

        private static string ParseHtml(string value)
        {
            var sb = new StringBuilder();

            using (var sr = new StringReader(value))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    // Temporarily replace double curly braces to avoid incorrect replacement
                    line = line.Replace("{{", "##OPEN##").Replace("}}", "##CLOSE##");

                    if (line.Contains("{"))
                    {
                        // Convert { -> < and } -> >
                        line = line.Replace("{", "<").Replace("}", ">");
                    }
                    else if (line.Contains("<"))
                    {
                        // Convert < -> { and > -> }
                        line = line.Replace("<", "{").Replace(">", "}");
                    }

                    // Restore double curly braces
                    line = line.Replace("##OPEN##", "{{").Replace("##CLOSE##", "}}");

                    sb.AppendLine(line);
                }
            }

            return sb.ToString();
        }


    }
}