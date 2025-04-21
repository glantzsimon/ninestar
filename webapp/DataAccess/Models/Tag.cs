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
    }

}
