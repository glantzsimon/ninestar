namespace K9.WebApplication.Services
{
    public interface IMailChimpService : IBaseService
    {
        void AddContact(string firstName, string lastName, string emailAddress);
        void AddContact(string name, string emailAddress);
        void AddAllContacts();
    }
}