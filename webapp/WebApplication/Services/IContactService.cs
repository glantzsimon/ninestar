using K9.DataAccessLayer.Models;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public interface IContactService
    {
        Contact GetOrCreateContact(string stripeCustomerId, string fullName, string emailAddress,
            string phoneNumber = "", int? userId = null); 
        Contact Find(int id);
        Contact Find(string emailAddress);
        List<Contact> ListContacts();
        void EnableMarketingEmails(string externalId, bool value = true);
        bool AreMarketingEmailsEnableForContact(int id);
    }
}