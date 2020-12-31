using K9.DataAccessLayer.Models;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public interface IContactService
    {
        Contact GetOrCreateContact(string stripeCustomerId, string fullName, string emailAddress);
        List<Contact> ListContacts();
    }
}