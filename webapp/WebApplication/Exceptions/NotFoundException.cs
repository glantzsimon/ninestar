using System;

namespace K9.WebApplication.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() 
            : base("Entity not found")
        {
        }
    }
}