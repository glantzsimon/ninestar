using K9.DataAccessLayer.Models;

namespace K9.WebApplication.Models
{
    public class MailingListViewModel
    {
        public string SqlQuery { get; set; }
        public MailingList MailingList  { get; set; }
    }
}