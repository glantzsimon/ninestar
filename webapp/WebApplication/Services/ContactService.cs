using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Services
{
    public class ContactService : IContactService
    {
        private readonly IRepository<Contact> _contactsRepository;
        private readonly ILogger _logger;

        public ContactService(IRepository<Contact> contactsRepository, ILogger logger)
        {
            _contactsRepository = contactsRepository;
            _logger = logger;
        }

        public void CreateCustomer(string stripeCustomerId, string fullName, string emailAddress)
        {
            if (!string.IsNullOrEmpty(emailAddress))
            {
                try
                {
                    var existingCustomer = _contactsRepository.Find(_ => _.StripeCustomerId == stripeCustomerId || _.EmailAddress == emailAddress).FirstOrDefault();
                    if (existingCustomer == null)
                    {
                        _contactsRepository.Create(new Contact
                        {
                            StripeCustomerId = stripeCustomerId,
                            FullName = string.IsNullOrEmpty(fullName) ? emailAddress : fullName,
                            EmailAddress = emailAddress
                        });
                    }
                    else
                    {
                        var isUpdated = false;
                        if (existingCustomer.FullName != fullName)
                        {
                            existingCustomer.FullName = fullName;
                            isUpdated = true;
                        }

                        if (existingCustomer.EmailAddress != emailAddress)
                        {
                            existingCustomer.EmailAddress = emailAddress;
                            isUpdated = true;
                        }

                        if (isUpdated)
                        {
                            _contactsRepository.Update(existingCustomer);
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Error($"ContactService => CreateCustomer => {e.Message}");
                    throw;
                }
            }
        }

        public List<Contact> ListContacts()
        {
            return _contactsRepository.List().OrderBy(e => e.FullName).ToList();
        }
    }
}