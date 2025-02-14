using DataAnnotationsExtensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.Models
{
    public class AccountActivationModel
    {
        public int UserId { get; set; }
        public Guid UniqueIdentifier { get; set; }
        public bool IsCodeResent { get; set; }

        [Max(1)]
        [Min(1)]
        [UIHint("Text")]
        public int? Digit1 { get; set; }

        [Max(1)]
        [Min(1)]
        [UIHint("Text")]
        public int? Digit2 { get; set; }

        [Max(1)]
        [Min(1)]
        [UIHint("Text")]
        public int? Digit3 { get; set; }

        [Max(1)]
        [Min(1)]
        [UIHint("Text")]
        public int? Digit4 { get; set; }

        [Max(1)]
        [Min(1)]
        [UIHint("Text")]
        public int? Digit5 { get; set; }

        [Max(1)]
        [Min(1)]
        [UIHint("Text")]
        public int? Digit6 { get; set; }
    }
}