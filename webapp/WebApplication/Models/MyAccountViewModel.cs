using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Models;

namespace K9.WebApplication.Models
{
    public class MyAccountViewModel
    {
        public User User { get; set; }
        public UserMembership Membership { get; set; }
    }
}