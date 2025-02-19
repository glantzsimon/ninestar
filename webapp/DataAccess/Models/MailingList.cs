using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;

namespace K9.DataAccessLayer.Models
{
    [Name(ResourceType = typeof(Globalisation.Dictionary), ListName = Globalisation.Strings.Names.MailingLists, PluralName = Globalisation.Strings.Names.MailingLists, Name = Globalisation.Strings.Names.MailingList)]
    public class MailingList : ObjectBase
    {
        
    }
}
