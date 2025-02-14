using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Globalisation;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(Dictionary), ListName = Strings.Names.Slots, PluralName = Strings.Names.Slots, Name = Strings.Names.Slot)]
    public class UserOTP : ObjectBase
    {
        public Guid UniqueIdentifier { get; set; } = new Guid();

        [UIHint("User")]
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }
        
        [Required]
        [Range(100000, 999999)] // Ensure it's always 6 digits
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SixDigitCodeLabel)]
        public int SixDigitCode { get; set; }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.VerifiedOnLabel)]
        public DateTime? VerifiedOn { get; set; }

        public UserOTP()
        {
            SixDigitCode = GenerateSixDigitCode();
        }

        private static int GenerateSixDigitCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999); // Ensures exactly 6 digits
        }

    }
}
