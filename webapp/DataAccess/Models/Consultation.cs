using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Enums;
using K9.Globalisation;
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(Dictionary), ListName = Strings.Names.Consultations, PluralName = Strings.Names.Consultations, Name = Strings.Names.Consultation)]
    public class Consultation : ObjectBase
    {

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ContactLabel)]
        [UIHint("Contact")]
        [Required]
        [ForeignKey("Contact")]
        public int ContactId { get; set; }
        
        [UIHint("ConsultationDuration")]
        [Required]
        [Display(ResourceType = typeof(Dictionary),
            Name = Strings.Labels.ConsultationDurationLabel)]
        public EConsultationDuration ConsultationDuration { get; set; } = EConsultationDuration.OneHour;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.CompletedOnLabel)]
        public DateTime? CompletedOn { get; set; }

        public virtual Contact Contact { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ContactLabel)]
        [LinkedColumn(LinkedTableName = "Contact", LinkedColumnName = "FullName")]
        public string ContactName { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.TotalPriceLabel)]
        [DataType(DataType.Currency)]
        public double Price => GetPrice();

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.TotalPriceLabel)]
        public string FormattedPrice => Price.ToString("C0", CultureInfo.GetCultureInfo("en-US"));

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DurationLabel)]
        public TimeSpan Duration => new TimeSpan((int)ConsultationDuration, 0, 0);

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DurationLabel)]
        public string DurationDescription =>
            ConsultationDuration.GetAttribute<EnumDescriptionAttribute>().GetDescription();

        private double GetPrice()
        {
            if (ConsultationDuration == EConsultationDuration.OneHour)
            {
                return 37;
            }

            if (ConsultationDuration == EConsultationDuration.TwoHours)
            {
                return 63;
            }

            return 0;
        }

    }
}
