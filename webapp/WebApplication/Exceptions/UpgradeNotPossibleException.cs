using System;

namespace K9.WebApplication.Exceptions
{
    public class UpgradeNotPossibleException : Exception
    {
        public UpgradeNotPossibleException() 
            : base(Globalisation.Dictionary.CannotSwitchMembershipError)
        {
        }
    }
}