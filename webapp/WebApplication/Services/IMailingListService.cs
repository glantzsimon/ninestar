using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public interface IMailingListService : IBaseService
    {
        MailingList Find(int id, bool includeUsers = false);
        List<MailingList> List(bool includeUsers = false);
        List<MailingList> ListAll();
        List<User> GetUsersForMailingList(int id);
        List<ListItem> GetMailingListListItems();
    }
}