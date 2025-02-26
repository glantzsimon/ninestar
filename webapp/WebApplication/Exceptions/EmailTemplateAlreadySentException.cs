using System;

namespace K9.WebApplication.Exceptions
{
    public class EmailTemplateAlreadySentException : Exception
    {
        public EmailTemplateAlreadySentException(string message) 
            : base(message)
        {
        }
    }
}