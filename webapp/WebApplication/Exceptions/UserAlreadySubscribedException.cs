using System;

namespace K9.WebApplication.Exceptions
{
    public class UserAlreadySubscribedException : Exception
    {
        public UserAlreadySubscribedException() 
            : base(Globalisation.Dictionary.PurchaseMembershipErrorAlreadySubscribed)
        {
        }
    }
}