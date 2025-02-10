using K9.Base.DataAccessLayer.Attributes;
using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Extensions;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(Dictionary), ListName = Strings.Names.Slots, PluralName = Strings.Names.Slots, Name = Strings.Names.Slot)]
    public class Slot : TimeZoneBase
    {
        [Required]
        [UIHint("DateTime")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.StartsOnLabel)]
        public DateTimeOffset StartsOn { get; set; }

        [UIHint("ConsultationDuration")]
        [Required]
        [Display(ResourceType = typeof(Dictionary),
            Name = Strings.Labels.ConsultationDurationLabel)]
        public EConsultationDuration ConsultationDuration { get; set; } = EConsultationDuration.OneHour;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.IsTakenLabel)]
        public bool IsTaken { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DurationLabel)]
        public TimeSpan Duration => new TimeSpan((int)ConsultationDuration, 0, 0);

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DurationLabel)]
        public string DurationDescription =>
            ConsultationDuration.GetAttribute<EnumDescriptionAttribute>().GetDescription();

        [UIHint("DateTime")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EndsOnLabel)]
        public DateTimeOffset EndsOn => StartsOn.Add(Duration);

        [UIHint("DateTime")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.StartsOnLabel)]
        public DateTimeOffset? StartsOnLocalTime => this.ToUserTimeZone(StartsOn);

        #region Local Time 

        public string FormattedStartsOnLocalDate => StartsOnLocalTime.HasValue
            ? StartsOnLocalTime.Value.Date.ToString(Constants.FormatConstants.AppointmentDisplayDateFormat)
            : "";

        public string FormattedStartsOnLocalDateTime => StartsOnLocalTime.HasValue
            ? StartsOnLocalTime.Value.Date.ToString(Constants.FormatConstants.AppointmentDisplayDateTimeFormat)
            : "";

        public string FormattedStartsOnLocalTime => StartsOnLocalTime.HasValue
            ? StartsOnLocalTime.Value.ToString(Constants.FormatConstants.AppointmentDisplayTimeFormat)
            : "";

        public string FormattedEndsOnLocalDate => EndsOnLocalTime.HasValue
            ? EndsOnLocalTime.Value.Date.ToString(Constants.FormatConstants.AppointmentDisplayDateFormat)
            : "";

        public string FormattedEndsOnLocalTime => EndsOnLocalTime.HasValue
            ? EndsOnLocalTime.Value.ToString(Constants.FormatConstants.AppointmentDisplayTimeFormat)
            : "";

        #endregion

        #region My Time

        public string FormattedStartsOnMyDate => StartsOnMyTime.HasValue
            ? StartsOnMyTime.Value.Date.ToString(Constants.FormatConstants.AppointmentDisplayDateFormat)
            : "";

        public string FormattedStartsOnMyTime => StartsOnMyTime.HasValue
            ? StartsOnMyTime.Value.ToString(Constants.FormatConstants.AppointmentDisplayTimeFormat)
            : "";

        public string FormattedEndsOnMyDate => EndsOnMyTime.HasValue
            ? EndsOnMyTime.Value.Date.ToString(Constants.FormatConstants.AppointmentDisplayDateFormat)
            : "";

        public string FormattedEndsOnMyTime => EndsOnMyTime.HasValue
            ? EndsOnMyTime.Value.ToString(Constants.FormatConstants.AppointmentDisplayTimeFormat)
            : "";

        #endregion

        [UIHint("DateTime")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EndsOnLabel)]
        public DateTimeOffset? EndsOnLocalTime => this.ToUserTimeZone(EndsOn);

        [UIHint("DateTime")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.StartsOnLabel)]
        public DateTimeOffset? StartsOnMyTime => this.ToMyTimeZone(StartsOn);

        [UIHint("DateTime")]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EndsOnLabel)]
        public DateTimeOffset? EndsOnMyTime => this.ToMyTimeZone(EndsOn);

    }
}
