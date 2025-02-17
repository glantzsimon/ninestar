using K9.Base.DataAccessLayer.Attributes;
using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Extensions;
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
    public class Consultation : TimeZoneBase
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

        [UIHint("DateTime")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ScheduledOnLabel)]
        public DateTimeOffset? ScheduledOn { get; set; }

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

        [ForeignKey("Slot")]
        public int? SlotId { get; set; }

        public virtual Slot Slot { get; set; }

        [UIHint("DateTimeOffset")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EndsOnLabel)]
        public DateTimeOffset? EndsOn
        {
            get
            {
                if (ScheduledOn.HasValue)
                {
                    return ScheduledOn.Value.Add(Duration);
                }

                return null;
            }
        }

        [UIHint("DateTimeOffset")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.StartsOnLabel)]
        public DateTimeOffset? ScheduledOnLocalTime => this.ToUserTimeZone(ScheduledOn);

        [UIHint("DateTimeOffset")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EndsOnLabel)]
        public DateTimeOffset? EndsOnLocalTime => this.ToUserTimeZone(EndsOn);

        [UIHint("DateTimeOffset")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.StartsOnLabel)]
        public DateTimeOffset? ScheduledOnMyTime => this.ToMyTimeZone(ScheduledOn);

        [UIHint("DateTimeOffset")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EndsOnLabel)]
        public DateTimeOffset? EndsOnMyTime => this.ToMyTimeZone(EndsOn);

        public string FormattedScheduledOnLocalDate => ScheduledOnLocalTime.HasValue
            ? ScheduledOnLocalTime.Value.Date.ToString(Constants.FormatConstants.AppointmentDisplayDateFormat)
            : "";

        public string FormattedScheduledOnLocalDateTime => ScheduledOnLocalTime.HasValue
            ? $"{ScheduledOnLocalTime.Value.ToString(Constants.FormatConstants.AppointmentDisplayDateFormat)} " +
              $"{ScheduledOnLocalTime.Value.ToString(Constants.FormatConstants.AppointmentDisplayTimeFormat)} - " +
              $"{EndsOnLocalTime.Value.ToString(Constants.FormatConstants.AppointmentDisplayTimeFormat)}"
            : "";

        public string FormattedScheduledOnLocalTime => ScheduledOnLocalTime.HasValue
            ? $"{ScheduledOnLocalTime.Value.ToString(Constants.FormatConstants.AppointmentDisplayTimeFormat)} - {TimeZoneDisplayText}"
            : "";

        public string FormattedEndsOnLocalDate => EndsOnLocalTime.HasValue
            ? EndsOnLocalTime.Value.Date.ToString(Constants.FormatConstants.AppointmentDisplayDateFormat)
            : "";

        public string FormattedEndsOnLocalTime => EndsOnLocalTime.HasValue
            ? $"{EndsOnLocalTime.Value.ToString(Constants.FormatConstants.AppointmentDisplayTimeFormat)} - {TimeZoneDisplayText}"
            : "";

        private double GetPrice()
        {
            if (ConsultationDuration == EConsultationDuration.OneHour)
            {
                return 72;
            }

            if (ConsultationDuration == EConsultationDuration.TwoHours)
            {
                return 111;
            }

            return 0;
        }

    }
}
