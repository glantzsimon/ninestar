using System.Text.RegularExpressions;
using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;

namespace K9.DataAccessLayer.Models
{
    [Name(ResourceType = typeof(Globalisation.Dictionary), ListName = Globalisation.Strings.Names.Tags, PluralName = Globalisation.Strings.Names.Tags, Name = Globalisation.Strings.Names.Tag)]
    public class Tag : ObjectBase
    {
        public string Slug { get; set; }
    }

    public class TagValue
    {
        public string Value { get; set; }

        public string Slugify()
        {
            if (string.IsNullOrWhiteSpace(Value)) return string.Empty;

            var slug = Value.ToLowerInvariant().Trim();
            slug = Regex.Replace(slug, @"[^\w\s-]", "");  // Remove non-word chars
            slug = Regex.Replace(slug, @"\s+", "-");      // Replace spaces with dashes
            slug = Regex.Replace(slug, "-+", "-");        // Collapse multiple dashes
            return slug;
        }
    }

}
