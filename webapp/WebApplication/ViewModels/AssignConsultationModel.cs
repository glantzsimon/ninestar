using System.ComponentModel.DataAnnotations;
using K9.DataAccessLayer.Enums;
using K9.Globalisation;

namespace K9.WebApplication.ViewModels
{
    public class AssignConsultationModel
    {
        public int UserId { get; set; }

        [UIHint("ConsultationDuration")]
        [Required]
        [Display(ResourceType = typeof(Dictionary),
            Name = Strings.Labels.ConsultationDurationLabel)]
        public EConsultationDuration Duration { get; set; } = EConsultationDuration.OneHour;
    }
}