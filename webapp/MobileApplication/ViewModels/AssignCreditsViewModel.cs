using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.ViewModels
{
    public class AssignCreditsViewModel
    {
        [Display(Name = "Number of Credits")]
        public int NumberOfCredits { get; set; }
        public int UserId { get; set; }
    }
}