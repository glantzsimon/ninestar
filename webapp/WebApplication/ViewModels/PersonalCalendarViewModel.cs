using K9.Globalisation;
using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.ViewModels
{
    public class PersonalCalendarViewModel
    {
        [UIHint("Url")]
        [Display(ResourceType = typeof(Dictionary), Name = nameof(Dictionary.CalendarLink))]
        public string CalendarLink { get; set; } = string.Empty;
    }
}
