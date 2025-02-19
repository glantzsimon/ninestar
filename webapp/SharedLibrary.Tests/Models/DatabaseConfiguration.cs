namespace K9.SharedLibrary.Tests.Models
{
    public class DatabaseConfiguration
    {
        public bool IsInitialCreate { get; set; }
        
        public string SystemUserPassword { get; set; }

        public string DefaultUserPassword { get; set; }
    }
}