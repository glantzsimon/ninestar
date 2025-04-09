using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Globalisation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(Dictionary), ListName = Strings.Names.UserInfos, PluralName = Strings.Names.UserInfos, Name = Strings.Names.UserInfo)]
    public class UserInfo : ObjectBase
    {
        public Guid UniqueIdentifier { get; set; } = new Guid();

        [UIHint("User")]
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        
        public virtual User User { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.TimeOfBirthLabel)]
        public TimeSpan TimeOfBirth { get; set; }

        [UIHint("TimeZone")]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.BirthTimeZone)]
        public string BirthTimeZoneId { get; set; }

    }
}
