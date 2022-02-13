using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.ViewModels
{
    public class AssignMembershipViewModel
    {
        [UIHint("MembershipOption")]
        [Display(Name = "Subscription")]
        public int MembershipOptionId { get; set; }
        public int UserId { get; set; }
    }
}