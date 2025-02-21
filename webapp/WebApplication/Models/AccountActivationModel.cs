using System;

namespace K9.WebApplication.Models
{
    public class AccountActivationModel
    {
        public int UserId { get; set; }
        public Guid UniqueIdentifier { get; set; }
        public bool IsCodeResent { get; set; }
        public bool IsAccountAlreadyActivated { get; set; }
        
        public int Digit1 { get; set; }
        public int Digit2 { get; set; }
        public int Digit3 { get; set; }
        public int Digit4 { get; set; }
        public int Digit5 { get; set; }
        public int Digit6 { get; set; }
    }
}